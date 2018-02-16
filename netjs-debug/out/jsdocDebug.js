"use strict";
/*---------------------------------------------------------
 * Copyright (C) Microsoft Corporation. All rights reserved.
 *--------------------------------------------------------*/
Object.defineProperty(exports, "__esModule", { value: true });
const vscode_debugadapter_1 = require("vscode-debugadapter");
const path_1 = require("path");
const jsdocRuntime_1 = require("./jsdocRuntime");
class JsdocDebugSession extends vscode_debugadapter_1.LoggingDebugSession {
    /**
     * Creates a new debug adapter that is used for one debug session.
     * We configure the default implementation of a debug adapter here.
     */
    constructor() {
        super("jsdoc-debug.txt");
        this._variableHandles = new vscode_debugadapter_1.Handles();
        // this debugger uses zero-based lines and columns
        this.setDebuggerLinesStartAt1(true);
        this.setDebuggerColumnsStartAt1(true);
        this._runtime = new jsdocRuntime_1.JsdocRuntime();
        // setup event handlers
        this._runtime.on('stopOnEntry', () => {
            this.sendEvent(new vscode_debugadapter_1.StoppedEvent('entry', JsdocDebugSession.THREAD_ID));
        });
        this._runtime.on('stopOnStep', () => {
            this.sendEvent(new vscode_debugadapter_1.StoppedEvent('step', JsdocDebugSession.THREAD_ID));
        });
        this._runtime.on('stopOnBreakpoint', () => {
            this.sendEvent(new vscode_debugadapter_1.StoppedEvent('breakpoint', JsdocDebugSession.THREAD_ID));
        });
        this._runtime.on('stopOnException', () => {
            this.sendEvent(new vscode_debugadapter_1.StoppedEvent('exception', JsdocDebugSession.THREAD_ID));
        });
        this._runtime.on('breakpointValidated', (bp) => {
            this.sendEvent(new vscode_debugadapter_1.BreakpointEvent('changed', { verified: bp.verified, id: bp.id }));
        });
        this._runtime.on('output', (text, filePath, line, column) => {
            const e = new vscode_debugadapter_1.OutputEvent(`${text}\n`);
            e.body.source = this.createSource(filePath);
            e.body.line = this.convertDebuggerLineToClient(line);
            e.body.column = this.convertDebuggerColumnToClient(column);
            this.sendEvent(e);
        });
        this._runtime.on('end', () => {
            this.sendEvent(new vscode_debugadapter_1.TerminatedEvent());
        });
    }
    /**
     * The 'initialize' request is the first request called by the frontend
     * to interrogate the features the debug adapter provides.
     */
    initializeRequest(response, args) {
        // since this debug adapter can accept configuration requests like 'setBreakpoint' at any time,
        // we request them early by sending an 'initializeRequest' to the frontend.
        // The frontend will end the configuration sequence by calling 'configurationDone' request.
        this.sendEvent(new vscode_debugadapter_1.InitializedEvent());
        // build and return the capabilities of this debug adapter:
        response.body = response.body || {};
        // the adapter implements the configurationDoneRequest.
        response.body.supportsConfigurationDoneRequest = true;
        // make VS Code to use 'evaluate' when hovering over source
        response.body.supportsEvaluateForHovers = true;
        this.sendResponse(response);
    }
    launchRequest(response, args) {
        // make sure to 'Stop' the buffered logging if 'trace' is not set
        vscode_debugadapter_1.logger.setup(args.trace ? vscode_debugadapter_1.Logger.LogLevel.Verbose : vscode_debugadapter_1.Logger.LogLevel.Stop, false);
        // start the program in the runtime
        this._runtime.start(args.program, !!args.stopOnEntry);
        this.sendResponse(response);
    }
    setBreakPointsRequest(response, args) {
        const path = args.source.path;
        const clientLines = args.lines || [];
        // clear all breakpoints for this file
        this._runtime.clearBreakpoints(path);
        // set and verify breakpoint locations
        const actualBreakpoints = clientLines.map(l => {
            let { verified, line, id } = this._runtime.setBreakPoint(path, this.convertClientLineToDebugger(l));
            const bp = new vscode_debugadapter_1.Breakpoint(verified, this.convertDebuggerLineToClient(line));
            bp.id = id;
            return bp;
        });
        // send back the actual breakpoint positions
        response.body = {
            breakpoints: actualBreakpoints
        };
        this.sendResponse(response);
    }
    threadsRequest(response) {
        // runtime supports now threads so just return a default thread.
        response.body = {
            threads: [
                new vscode_debugadapter_1.Thread(JsdocDebugSession.THREAD_ID, "thread 1")
            ]
        };
        this.sendResponse(response);
    }
    stackTraceRequest(response, args) {
        const startFrame = typeof args.startFrame === 'number' ? args.startFrame : 0;
        const maxLevels = typeof args.levels === 'number' ? args.levels : 1000;
        const endFrame = startFrame + maxLevels;
        const stk = this._runtime.stack(startFrame, endFrame);
        response.body = {
            stackFrames: stk.frames.map(f => new vscode_debugadapter_1.StackFrame(f.index, f.name, this.createSource(f.file), this.convertDebuggerLineToClient(f.line))),
            totalFrames: stk.count
        };
        this.sendResponse(response);
    }
    scopesRequest(response, args) {
        const frameReference = args.frameId;
        const scopes = this._runtime.scopes(frameReference);
        response.body = {
            scopes: scopes.map(s => new vscode_debugadapter_1.Scope(s.name, this._variableHandles.create(s.name + "_" + frameReference), false))
        };
        this.sendResponse(response);
    }
    variablesRequest(response, args) {
        const variables = new Array();
        const id = this._variableHandles.get(args.variablesReference);
        if (id !== null) {
            const data = this._runtime.variables(id);
            for (var key in data) {
                var value = data[key].toString();
                if (typeof data[key] == "object") {
                    value = JSON.stringify(data[key]);
                }
                variables.push({
                    name: key,
                    type: typeof data[key],
                    value: value,
                    variablesReference: 0
                });
            }
        }
        response.body = {
            variables: variables
        };
        this.sendResponse(response);
    }
    continueRequest(response, args) {
        this._runtime.continue();
        this.sendResponse(response);
    }
    reverseContinueRequest(response, args) {
        this._runtime.continue(true);
        this.sendResponse(response);
    }
    nextRequest(response, args) {
        this._runtime.step();
        this.sendResponse(response);
    }
    stepBackRequest(response, args) {
        this._runtime.step(true);
        this.sendResponse(response);
    }
    evaluateRequest(response, args) {
        let reply = undefined;
        if (args.context === 'repl') {
            // 'evaluate' supports to create and delete breakpoints from the 'repl':
            const matches = /new +([0-9]+)/.exec(args.expression);
            if (matches && matches.length === 2) {
                const mbp = this._runtime.setBreakPoint(this._runtime.sourceFile, this.convertClientLineToDebugger(parseInt(matches[1])));
                const bp = new vscode_debugadapter_1.Breakpoint(mbp.verified, this.convertDebuggerLineToClient(mbp.line), undefined, this.createSource(this._runtime.sourceFile));
                bp.id = mbp.id;
                this.sendEvent(new vscode_debugadapter_1.BreakpointEvent('new', bp));
                reply = `breakpoint created`;
            }
            else {
                const matches = /del +([0-9]+)/.exec(args.expression);
                if (matches && matches.length === 2) {
                    const mbp = this._runtime.clearBreakPoint(this._runtime.sourceFile, this.convertClientLineToDebugger(parseInt(matches[1])));
                    if (mbp) {
                        const bp = new vscode_debugadapter_1.Breakpoint(false);
                        bp.id = mbp.id;
                        this.sendEvent(new vscode_debugadapter_1.BreakpointEvent('removed', bp));
                        reply = `breakpoint deleted`;
                    }
                }
            }
        }
        response.body = {
            result: reply ? reply : `evaluate(context: '${args.context}', '${args.expression}')`,
            variablesReference: 0
        };
        this.sendResponse(response);
    }
    //---- helpers
    createSource(filePath) {
        return new vscode_debugadapter_1.Source(path_1.basename(filePath), this.convertDebuggerPathToClient(filePath), undefined, undefined, 'jsdoc-adapter-data');
    }
}
// we don't support multiple threads, so we can use a hardcoded ID for the default thread
JsdocDebugSession.THREAD_ID = 1;
vscode_debugadapter_1.DebugSession.run(JsdocDebugSession);
//# sourceMappingURL=jsdocDebug.js.map
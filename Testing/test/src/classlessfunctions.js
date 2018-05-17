// classlessfunctions include
assert(() => include("text.txt") == undefined, "include");

//load
assert(() => load("text.txt") == "text", "load");

// out
assert(() => out("text") == undefined, "out");

// outLine
assert(() => outLine("text") == undefined, "outLine");

// import
assert(() => import("text.txt") == undefined, "import");

// redirect

//unsafe
var a = false;
assert(() => unsafe(function () {a = true}) == undefined, "unsafe");
assert(() => a == true, "unsafe result");
// async waitAll
var a = 0;
var b = 0;

function test(variable) {
    variable = parseInt(new Date().getSeconds().toString()); 
    Async.sleep(500);
    return [variable];
}
assert(() => Async.waitAll(
    () => a =test(a),
    () => b =parseInt(new Date().getSeconds().toString())
) == undefined, "Async.waitAll");
assert(() => a[0] - b < 5, "Async.waitAll result");

// waitAny
a = false;
b = false;

assert(() => Async.waitAny(
    () => a=test(a),
    () => b=test(b)
) == undefined, "Async.waitAny");
assert(() => (a[0] != false) || (b[0] != false), "Async.waitAny result");

// run
assert(() => Async.run(
	() => a = 0,
	() => b = 0
) == undefined, "Async.run");
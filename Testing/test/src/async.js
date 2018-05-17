// async waitAll
var a = 0;
var b = 0;

function test(variable) {
    variable = parseInt(new Date().getSeconds().toString() + new Date().getMilliseconds().toString()); 
    var t = 1;
    for(var i = 1; i<999; i++) {
        t = t/i + t;
        for(var i = 1; i<999; i++) {
            t += t/i;
        }
    }
    return [variable, t];
}
assert(() => Async.waitAll(
    () => a =test(a),
    () => b =parseInt(new Date().getSeconds().toString() + new Date().getMilliseconds().toString())
) == undefined, "Async.waitAll");
assert(() => a[0] - b < 10 && a[1] == 1998, "Async.waitAll result");

// waitAny
a = false;
b = false;

assert(() => Async.waitAny(
    () => a=test(a),
    () => b=test(b)
) == undefined, "Async.waitAny");
assert(() => (a[0] != false && a[1] == 1998) || (b[0] != false && b[1] == 1998), "Async.waitAny result");

// run
assert(() => Async.run(
	() => a = 0,
	() => b = 0
) == undefined, "Async.run");
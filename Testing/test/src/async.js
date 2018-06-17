// async waitAll
var a = false;
var b = false;

assert(() => Async.waitAll(
    () => a = true,
    () => b = true
) == undefined, "Async.waitAll");
assert(() => a == true && b == true, "Async.waitAll result");

// waitAny
a = false;
b = false;

assert(() => Async.waitAny(
    () => {
		Async.sleep(1000);
		a = true;
	},
    () => b = true
) == undefined, "Async.waitAny");
assert(() => a == false && b == true, "Async.waitAny result");

// run
a = false;
b = false;

assert(() => Async.run(
	() => a = true,
	() => b = true
) == undefined, "Async.run");
Async.sleep(100);
assert(() => a == true && b == true, "Async.run result");
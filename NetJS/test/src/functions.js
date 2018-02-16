var add = function(a, b){
	return a + b;
};
assert(() => add(3, 4) == 7, "Function var define");

function addFunction(a, b){
	return a + b;
}
assert(() => addFunction(4, 9) == 13, "Function function define");

function argumentTest(a, b, c){
	if(!a){
		return "not a";
	}
	if(!b){
		return "not b";
	}
	if(!c){
		return "not c";
	}
	return "all";
}

assert(() => argumentTest() == "not a", "Function arguments three missing");
assert(() => argumentTest(1) == "not b", "Function arguments one missing");
assert(() => argumentTest(1, 2) == "not c", "Function arguments two missing");
assert(() => argumentTest(1, 2, 3) == "all", "Function arguments all");
assert(() => argumentTest(1, 2, 3, 4) == "all", "Function arguments one extra");

var surround = x => "(" + x + ")";
assert(() => surround(3) == "(3)", "Arrow function single argument");

var combine = (a, b) => a + " - " + b;
assert(() => combine("alpha", "beta") == "alpha - beta", "Arrow function multiple arguments");

var substract = (a, b) => {
	return a - b;
};
assert(() => substract(4, 3) == 1, "Arrow function block");
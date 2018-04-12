assert(() => parseInt("34") == 34, "ParseInt string");
assert(() => parseInt("abc") == NaN, "ParseInt invalid string");
assert(() => parseInt("3.4") == 3, "ParseInt float string");
assert(() => parseInt(3.56) == 3, "ParseInt float");

assert(() => parseFloat("3.5") == 3.5, "ParseFloat string");
assert(() => parseFloat("abc") == NaN, "ParseFloat invalid string");

assert(() => encodeURI("https://mozilla.org/?x=шеллы") == "https://mozilla.org/?x=%D1%88%D0%B5%D0%BB%D0%BB%D1%8B", "EncodeURI");
assert(() => decodeURI("https://mozilla.org/?x=%D1%88%D0%B5%D0%BB%D0%BB%D1%8B") == "https://mozilla.org/?x=шеллы", "DecodeURI");

assert(() => encodeURIComponent("?x=шеллы") == "%3Fx%3D%D1%88%D0%B5%D0%BB%D0%BB%D1%8B", "EncodeURIComponent");
assert(() => decodeURIComponent("%3Fx%3D%D1%88%D0%B5%D0%BB%D0%BB%D1%8B") == "?x=шеллы", "DecodeURIComponent");

assert(() => eval("3 + 5") == 8, "Eval simple addition");

var factorial = function(x){
	var result = 1;
	for(var i = 1; i <= x; i++){
		result *= i;
	}
	return result;
};
assert(() => uneval(factorial) == `var factorial = function(x){
	var result = 1;
	for(var i = 1; i <= x; i++){
		result = result * i;
	}
	return result;
}`, "Uneval");
// Assign
var x = 3;
assert(() => x == 3, "Assign number");
var y = "string";
assert(() => y == "string", "Assign string");
var z = true;
assert(() => z === true, "Assign boolean");

// Equality
assert(() => 1 == 1, "Number == Number");
assert(() => 1 != 2, "Number != Number");

assert(() => "1" == 1, "String == Number");
assert(() => 1 == "1", "Number == String");
assert(() => "1" == "1", "String == String");

assert(() => 0 == false, "0 == false");
assert(() => 0 != null, "0 != null");
assert(() => 0 != undefined, "0 != undefined");

assert(() => undefined == null, "undefined == null");
assert(() => null == undefined, "null == undefined");

var obj = {x: {}};
assert(() => obj == obj, "Object == Object");
assert(() => obj != {}, "Object != Object");
assert(() => {return {} != {};}, "Object != Object");
assert(() => obj.x == obj.x, "Object.property == Object.property");
assert(() => !!obj, "Object");
assert(() => obj != true, "Object != true");

var x = 3;
assert(() => !!x, "Defined variable == true");
assert(() => !undefinedVariable, "Undefined variable == false");

assert(() => !(""), "Empty string == false");
assert(() => !!("x"), "Not empty string == true");

assert(() => 1 === 1, "Number === Number");
assert(() => 1 !== 2, "Number !== Number");

assert(() => "1" !== 1, "String !== Number");

// Numbers

assert(() => 3 + 5 == 8, "Number addition");
assert(() => 3 - 5 == -2, "Number substraction");
assert(() => 3 * 5 == 15, "Number multiplication");
assert(() => 15 / 3 == 5, "Number division");
assert(() => 4 % 3 == 1, "Number remainder");

assert(() => 5 * -1 == -5, "Number negation");
assert(() => 3 + -5 == -2, "Number add negation");
assert(() => 3 - -5 == 8, "Number substract negation");
assert(() => -(3 + 4) == -7, "Number negate group");
assert(() => -3 * -4 == 12, "Number negative times negative - " + x);

assert(() => 3 + 5 * 2 == 13, "Number multiply before add");

assert(() => 3 * (5 + 2) == 21, "Number group before multiply");
assert(() => (2 + 4) / (1 + 2) == 2, "multiple groups");
assert(() => (2 * (2 + 1)) * (((3 + 1) * 2) / 2) == 24, "Number groups in groups");

var x = 0;
assert(() => x++ == 0, "Number postfix increment");
assert(() => x == 1, "Number postfix increment");

var x = 0;
assert(() => x-- == 0, "Number postfix decrement");
assert(() => x == -1, "Number postfix decrement");

var x = 0;
assert(() => ++x == 1, "Number prefix increment");
assert(() => x == 1, "Number prefix increment");

var x = 0;
assert(() => --x == -1, "Number prefix decrement");
assert(() => x == -1, "Number prefix decrement");

x = 0;
x += 3;
assert(() => x == 3, "Number add assign");
x *= 3;
assert(() => x == 9, "Number multiply assign");
x /= 9;
assert(() => x == 1, "Number divide assign");
x -= 3;
assert(() => x == -2, "Number substract assign");
x %= 2;
assert(() => x == 0, "Number remainder assign");

assert(() => 3 < 5, "Number less than");
assert(() => !(5 < 3), "Number not less than");
assert(() => 3 <= 5, "Number less than equal");
assert(() => 5 <= 5, "Number less than equal");
assert(() => !(6 <= 5), "Number not less than equal");

assert(() => 6 > 3, "Number greater than");
assert(() => !(2 > 3), "Number not greater than");
assert(() => 5 >= 3, "Number greater than equal");
assert(() => 5 >= 5, "Number greater than equal");
assert(() => !(4 >= 5), "Number not greater than equal");

// Booleans

assert(() => !false, "Logical NOT");

assert(() => true && true, "Logical AND true && true");
assert(() => !(false && false), "Logical AND false && false");
assert(() => !(true && false), "Logical AND true && false");
assert(() => !(false && true), "Logical AND false && true");

assert(() => true || true, "Logical OR true || true");
assert(() => true || false, "Logical OR true || false");
assert(() => false || true, "Logical OR false || true");
assert(() => !(false || false), "Logical OR false || false");

// Strings

assert(() => "abc" + "def" == "abcdef", "String + String");

assert(() => "abc" + 123 == "abc123", "String + Number");
assert(() => 123 + "abc" == "123abc", "Number + String");

assert(() => "abc" + true == "abctrue", "String + Boolean");
assert(() => false + "abc" == "falseabc", "Boolean + String");

var s = "abc";
s += "def";
assert(() => s == "abcdef", "String += String");

s += 123;
assert(() => s == "abcdef123", "String += Number");

// Type of

assert(() => typeof 37 === "number", "Typeof Number");
assert(() => typeof NaN === "number", "Typeof NaN");
assert(() => typeof "" === "string", "Typeof String");
assert(() => typeof true === "boolean", "Typeof Boolean");
assert(() => typeof undefined === "undefined", "Typeof Undefined");
assert(() => typeof undeclaredVariable === "undefined", "Typeof undeclared variable");
assert(() => typeof {x: 1} === "object", "Typeof Object");
assert(() => typeof [1, 2, 3] === "object", "Typeof Array");
assert(() => typeof function(){} === "function", "Typeof Function");
assert(() => typeof Math.sin === "function", "Typeof Math.sin");
assert(() => typeof (x => x) === "function", "Typeof arrow function");

// In

var obj = {x: 3};
assert(() => "x" in obj, "Property in Object");
assert(() => !("y" in obj), "Property not in Object");

// Conditional

assert(() => (true ? "yes" : "no") == "yes", "Conditional true");
assert(() => (false ? "yes": "no") == "no", "Conditional false");

// Call + Access
var getObject = function(){
	return {x: 3};
};
assert(() => getObject().x == 3, "Call + Access dot");
assert(() => getObject()["x"] == 3, "Call + Access brackets");
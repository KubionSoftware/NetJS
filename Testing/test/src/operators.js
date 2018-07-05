// Assign
var x = 3;
Test.assert(() => x == 3, "Assign number");
var y = "string";
Test.assert(() => y == "string", "Assign string");
var z = true;
Test.assert(() => z === true, "Assign boolean");

// Equality
Test.assert(() => 1 == 1, "Number == Number");
Test.assert(() => 1 != 2, "Number != Number");

Test.assert(() => "1" == 1, "String == Number");
Test.assert(() => 1 == "1", "Number == String");
Test.assert(() => "1" == "1", "String == String");

Test.assert(() => 0 == false, "0 == false");
Test.assert(() => 0 != null, "0 != null");
Test.assert(() => 0 != undefined, "0 != undefined");

Test.assert(() => undefined == null, "undefined == null");
Test.assert(() => null != false, "null == false");

var obj = {x: {}};
Test.assert(() => obj == obj, "Object == Object");
Test.assert(() => obj != {}, "Object != Object");
Test.assert(() => {return {} != {};}, "Object != Object");
Test.assert(() => obj.x == obj.x, "Object.property == Object.property");
Test.assert(() => !!obj, "Object");
Test.assert(() => obj != true, "Object != true");

x = 3;
Test.assert(() => !!x, "Defined variable == true");
Test.assert(() => typeof undefinedVariable == "undefined", "Undefined variable == false");

Test.assert(() => !(""), "Empty string == false");
Test.assert(() => !!("x"), "Not empty string == true");

Test.assert(() => 1 === 1, "Number === Number");
Test.assert(() => 1 !== 2, "Number !== Number");

Test.assert(() => "1" !== 1, "String !== Number");

// Numbers

Test.assert(() => 3 + 5 == 8, "Number addition");
Test.assert(() => 3.2 * 2 == 6.4, "Double addition");
Test.assert(() => 3 - 5 == -2, "Number substraction");
Test.assert(() => 3 * 5 == 15, "Number multiplication");
Test.assert(() => 15 / 3 == 5, "Number division");
Test.assert(() => 4 % 3 == 1, "Number remainder");

Test.assert(() => 5 * -1 == -5, "Number negation");
Test.assert(() => 3 + -5 == -2, "Number add negation");
Test.assert(() => 3 - -5 == 8, "Number substract negation");
Test.assert(() => -(3 + 4) == -7, "Number negate group");
Test.assert(() => -3 * -4 == 12, "Number negative times negative - " + x);

Test.assert(() => 3 + 5 * 2 == 13, "Number multiply before add");

Test.assert(() => 3 * (5 + 2) == 21, "Number group before multiply");
Test.assert(() => (2 + 4) / (1 + 2) == 2, "multiple groups");
Test.assert(() => (2 * (2 + 1)) * (((3 + 1) * 2) / 2) == 24, "Number groups in groups");

x = 0;
Test.assert(() => x++ == 0, "Number postfix increment");
Test.assert(() => x == 1, "Number postfix increment");

x = 0;
Test.assert(() => x-- == 0, "Number postfix decrement");
Test.assert(() => x == -1, "Number postfix decrement");

x = 0;
Test.assert(() => ++x == 1, "Number prefix increment");
Test.assert(() => x == 1, "Number prefix increment");

x = 0;
Test.assert(() => --x == -1, "Number prefix decrement");
Test.assert(() => x == -1, "Number prefix decrement");

x = 0;
x += 3;
Test.assert(() => x == 3, "Number add assign");
x *= 3;
Test.assert(() => x == 9, "Number multiply assign");
x /= 9;
Test.assert(() => x == 1, "Number divide assign");
x -= 3;
Test.assert(() => x == -2, "Number substract assign");
x %= 2;
Test.assert(() => x == 0, "Number remainder assign");

Test.assert(() => 3 < 5, "Number less than");
Test.assert(() => !(5 < 3), "Number not less than");
Test.assert(() => 3 <= 5, "Number less than equal");
Test.assert(() => 5 <= 5, "Number less than equal");
Test.assert(() => !(6 <= 5), "Number not less than equal");

Test.assert(() => 6 > 3, "Number greater than");
Test.assert(() => !(2 > 3), "Number not greater than");
Test.assert(() => 5 >= 3, "Number greater than equal");
Test.assert(() => 5 >= 5, "Number greater than equal");
Test.assert(() => !(4 >= 5), "Number not greater than equal");

// Booleans

Test.assert(() => !false, "Logical NOT");

Test.assert(() => true && true, "Logical AND true && true");
Test.assert(() => !(false && false), "Logical AND false && false");
Test.assert(() => !(true && false), "Logical AND true && false");
Test.assert(() => !(false && true), "Logical AND false && true");

Test.assert(() => true || true, "Logical OR true || true");
Test.assert(() => true || false, "Logical OR true || false");
Test.assert(() => false || true, "Logical OR false || true");
Test.assert(() => !(false || false), "Logical OR false || false");

// Strings

Test.assert(() => "abc" + "def" == "abcdef", "String + String");

Test.assert(() => "abc" + 123 == "abc123", "String + Number");
Test.assert(() => 123 + "abc" == "123abc", "Number + String");

Test.assert(() => "abc" + true == "abctrue", "String + Boolean");
Test.assert(() => false + "abc" == "falseabc", "Boolean + String");

var s = "abc";
s += "def";
Test.assert(() => s == "abcdef", "String += String");

s += 123;
Test.assert(() => s == "abcdef123", "String += Number");

// Type of

Test.assert(() => typeof 37 === "number", "Typeof Number");
Test.assert(() => typeof NaN === "number", "Typeof NaN");
Test.assert(() => typeof "" === "string", "Typeof String");
Test.assert(() => typeof true === "boolean", "Typeof Boolean");
Test.assert(() => typeof undefined === "undefined", "Typeof Undefined");
Test.assert(() => typeof undeclaredVariable === "undefined", "Typeof undeclared variable");
Test.assert(() => typeof {x: 1} === "object", "Typeof Object");
Test.assert(() => typeof [1, 2, 3] === "object", "Typeof Array");
Test.assert(() => typeof function(){} === "function", "Typeof Function");
Test.assert(() => typeof Math.sin === "function", "Typeof Math.sin");
Test.assert(() => typeof (x => x) === "function", "Typeof arrow function");

// Instance of
Test.assert(() => function(){} instanceof Function, "Function instance of Function");
Test.assert(() => function(){} instanceof Object, "Function instance of Object");
Test.assert(() => !(function(){} instanceof Array), "Function not instance of Array");
Test.assert(() => [] instanceof Array, "[] instance of Array");
Test.assert(() => [] instanceof Object, "[] instance of Object");
Test.assert(() => !([] instanceof Function), "[] not instance of Function");

// In

obj = {x: 3};
Test.assert(() => "x" in obj, "Property in Object");
Test.assert(() => !("y" in obj), "Property not in Object");

// Conditional

Test.assert(() => (true ? "yes" : "no") == "yes", "Conditional true");
Test.assert(() => (false ? "yes": "no") == "no", "Conditional false");

// Call + Access
var getObject = function(){
	return {x: 3};
};
Test.assert(() => getObject().x == 3, "Call + Access dot");
Test.assert(() => getObject()["x"] == 3, "Call + Access brackets");
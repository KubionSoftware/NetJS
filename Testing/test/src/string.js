var s;

s = "abc";
assert(() => s.length == 3, "String.length");

assert(() => "abc".charAt(1) == "b", "String.charAt");
assert(() => "abc".charCodeAt(1) == 98, "String.charCodeAt");

s = "abc hoi def";

assert(() => s.startsWith("abc"), "String.startsWith");
assert(() => !s.startsWith("abd"), "String.startsWith negative");

assert(() => s.endsWith("def"), "String.endsWith");
assert(() => !s.endsWith("de"), "String.endsWith negative");

assert(() => s.includes("hoi"), "String.includes");
assert(() => !s.includes("hallo"), "String.includes negative");

assert(() => s.indexOf("b") == 1, "String.indexOf single");
assert(() => s.indexOf("hoi") == 4, "String.indexOf multiple");

assert(() => "aBc".toLowerCase() == "abc", "String.toLowerCase");
assert(() => "aBc".toUpperCase() == "ABC", "String.toUpperCase");

assert(() => "  abc   ".trim() == "abc", "String.trim");

assert(() => "abcdef".substr(1, 3) == "bcd", "String.substr");
assert(() => "abc".substr(0) == "abc", "String.substr without length");
assert(() => "abc".substr(0, 9) == "abc", "String.substr index > length");
assert(() => "abcdef".substr(-3) == "def", "String.substr negative start");

assert(() => "abcdef".substring(1, 3) == "bc", "String.substring");
assert(() => "abc".substring(-1, 9) == "abc", "String.substring overflow");
assert(() => "abc".substring(3, 0) == "abc", "String.substring start > end");

var parts = "a,b,c".split(",");
assert(() => parts.length == 3, "String.split");
assert(() => parts[0] == "a" && parts[1] == "b" && parts[2] == "c", "String.split");

s = "abc hallo abc hi abc".replace("abc", "xxx");
assert(() => s == "xxx hallo xxx hi xxx", "String.replace");

assert(() => "abc".repeat(3) == "abcabcabc", "String.repeat");

var x = 3;
assert(() => `x:${x}` == "x:3", "String template variable");
assert(() => `${1 + 1}` == "2", "String template expression");
assert(() => `${1 + 1} - ${1 + 2}` == "2 - 3", "String template multiple expressions");
// nesting Tags
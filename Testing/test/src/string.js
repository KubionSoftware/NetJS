var s;

s = "abc";
Test.assert(() => s.length == 3, "String.length");

Test.assert(() => "abc".charAt(1) == "b", "String.charAt");
Test.assert(() => "abc".charCodeAt(1) == 98, "String.charCodeAt");

s = "abc hoi def";

Test.assert(() => s.startsWith("abc"), "String.startsWith");
Test.assert(() => !s.startsWith("abd"), "String.startsWith negative");

Test.assert(() => s.endsWith("def"), "String.endsWith");
Test.assert(() => !s.endsWith("de"), "String.endsWith negative");

Test.assert(() => s.includes("hoi"), "String.includes");
Test.assert(() => !s.includes("hallo"), "String.includes negative");

Test.assert(() => s.indexOf("b") == 1, "String.indexOf single");
Test.assert(() => s.indexOf("hoi") == 4, "String.indexOf multiple");

Test.assert(() => "aBc".toLowerCase() == "abc", "String.toLowerCase");
Test.assert(() => "aBc".toUpperCase() == "ABC", "String.toUpperCase");

Test.assert(() => "  abc   ".trim() == "abc", "String.trim");

Test.assert(() => "abcdef".substr(1, 3) == "bcd", "String.substr");
Test.assert(() => "abc".substr(0) == "abc", "String.substr without length");
Test.assert(() => "abc".substr(0, 9) == "abc", "String.substr index > length");
Test.assert(() => "abcdef".substr(-3) == "def", "String.substr negative start");

Test.assert(() => "abcdef".substring(1, 3) == "bc", "String.substring");
Test.assert(() => "abc".substring(-1, 9) == "abc", "String.substring overflow");
Test.assert(() => "abc".substring(3, 0) == "abc", "String.substring start > end");

var parts = "a,b,c".split(",");
Test.assert(() => parts.length == 3, "String.split");
Test.assert(() => parts[0] == "a" && parts[1] == "b" && parts[2] == "c", "String.split");

s = "abc hallo abc hi abc".replace("abc", "xxx");
Test.assert(() => s == "xxx hallo abc hi abc", "String.replace");

Test.assert(() => "abc".repeat(3) == "abcabcabc", "String.repeat");

var x = 3;
Test.assert(() => `x:${x}` == "x:3", "String template variable");
Test.assert(() => `${1 + 1}` == "2", "String template expression");
Test.assert(() => `${1 + 1} - ${1 + 2}` == "2 - 3", "String template multiple expressions");
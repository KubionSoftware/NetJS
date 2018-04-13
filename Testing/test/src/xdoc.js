assert(() => XDoc.load("return") == "testvalue65", "XDoc.load");

XDoc.include("return");
assert(() => Buffer.get() == "testvalue65", "XDoc.include");
Buffer.clear();

assert(() => XDoc.load("parameters", {a: "AAA", b: "BBB"}) == "BBBAAA", "XDoc.load parameters");

XDoc.set("x", "", "", "testvalue32");
assert(() => XDoc.load("set") == "testvalue32", "XDoc.set");

XDoc.load("get");
assert(() => XDoc.get("y", "", "") == "testvalue83", "XDoc.get");
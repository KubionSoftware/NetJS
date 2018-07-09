// XDoc load
Test.assert(() => XDoc.load("return") == "testvalue65", "XDoc.load");

// include
Buffer.clear();
XDoc.include("return");
Test.assert(() => Buffer.get() == "testvalue65", "XDoc.include");
Buffer.clear();
Test.assert(() => XDoc.load("parameters", {a: "AAA", b: "BBB"}) == "BBBAAA", "XDoc.load parameters");

// set
XDoc.set("x", "", "", "testvalue32")
Test.assert(() => XDoc.load("set") == "testvalue32", "XDoc.set");

// get
XDoc.load("get");
Test.assert(() => XDoc.get("y", "", "") == "testvalue83", "XDoc.get");
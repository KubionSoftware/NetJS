// classlessfunctions include
Buffer.clear();
include("text.txt");
Test.assert(() => Buffer.get() == "text", "include");

//load
Test.assert(() => load("text.txt") == "text", "load");
out("hello");
Test.assert(() => Buffer.get() == "hello", "Buffer.get");

Buffer.clear();
Test.assert(() => Buffer.get() == "", "Buffer.clear");

Buffer.set("test");
Test.assert(() => Buffer.get() == "test", "Buffer.set");

Buffer.clear();
include("text.txt");
Test.assert(() => Buffer.get() == "text", "include");
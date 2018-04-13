out("hello");
assert(() => Buffer.get() == "hello", "Buffer.get");

Buffer.clear();
assert(() => Buffer.get() == "", "Buffer.clear");

Buffer.set("test");
assert(() => Buffer.get() == "test", "Buffer.set");

Buffer.clear();
include("text.txt");
assert(() => Buffer.get() == "text", "include");
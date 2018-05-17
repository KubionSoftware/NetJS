// encode
assert(() => Base64.encode("Hello World!") ==  "SGVsbG8gV29ybGQh", "Base64.encode");

// decode
assert(() => Base64.decode("SGVsbG8gV29ybGQh") ==  "Hello World!", "Base64.decode");
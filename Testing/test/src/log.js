// Log write
assert(() => Log.write("Hello-World!") == undefined, "Log.write");
assert(() => IO.readText("log.txt").endsWith("Hello-World!\n"), "Log.write result");
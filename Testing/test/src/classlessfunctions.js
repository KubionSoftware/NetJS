// classlessfunctions include
assert(() => include("text.txt") == undefined, "include");
assert(() => load("text.txt") == "text", "load");
assert(() => out("text") == undefined, "out");
assert(() => outLine("text") == undefined, "outLine");
assert(() => import("text.txt") == undefined, "import");
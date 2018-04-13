assert(() => /abc/.test("abc"), "Regex literal simple valid test");
assert(() => !(/def/.test("abc")), "Regex literal simple invalid test");

assert(() => new RegExp("abc").test("abc"), "Regex constructor simple valid test");
assert(() => !(new RegExp("def").test("abc")), "Regex constructor simple invalid test");

assert(() => /^[a-z\\]*$/.test("abc\\"), "Regex advanced + escape valid test");
assert(() => !(/^[a-z\\]*$/.test("abc/")), "Regex advanced + escape invalid test");

var parts = "abc%def#ghi".split(/[^a-z]/);
assert(() => parts.length == 3, "Regex split length");
assert(() => parts[0] == "abc" && parts[1] == "def" && parts[2] == "ghi", "Regex split parts");

assert(() => "abc%def#ghi".replace(/[a-z]+/, "_") == "_%def#ghi", "Regex replace");
assert(() => "abc%def#ghi".replace(/[a-z]+/g, "_") == "_%_#_", "Regex replace + global");

var matches = "abc%def#ghi".match(/[a-z]+/g);
assert(() => matches.length == 3, "Regex match length");
assert(() => matches[0] == "abc" && matches[1] == "def" && matches[2] == "ghi", "Regex match matches");
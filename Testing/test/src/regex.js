Test.assert(() => /abc/.test("abc"), "Regex literal simple valid test");
Test.assert(() => !(/def/.test("abc")), "Regex literal simple invalid test");

Test.assert(() => new RegExp("abc").test("abc"), "Regex constructor simple valid test");
Test.assert(() => !(new RegExp("def").test("abc")), "Regex constructor simple invalid test");

Test.assert(() => /^[a-z\\]*$/.test("abc\\"), "Regex advanced + escape valid test");
Test.assert(() => !(/^[a-z\\]*$/.test("abc/")), "Regex advanced + escape invalid test");

var parts = "abc%def#ghi".split(/[^a-z]/);
Test.assert(() => parts.length == 3, "Regex split length");
Test.assert(() => parts[0] == "abc" && parts[1] == "def" && parts[2] == "ghi", "Regex split parts");

Test.assert(() => "abc%def#ghi".replace(/[a-z]+/, "_") == "_%def#ghi", "Regex replace");
Test.assert(() => "abc%def#ghi".replace(/[a-z]+/g, "_") == "_%_#_", "Regex replace + global");

var matches = "abc%def#ghi".match(/[a-z]+/g);
Test.assert(() => matches.length == 3, "Regex match length");
Test.assert(() => matches[0] == "abc" && matches[1] == "def" && matches[2] == "ghi", "Regex match matches");
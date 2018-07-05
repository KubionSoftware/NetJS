// Session set
Test.assert(() => Session.set("Hello", "World!") === true, "Session.set");

// get
Test.assert(() => Session.get("Hello") == "World!", "Session.get");

// getAll
Test.assert(() => Session.getAll()["Hello"] == "World!", "Session.getAll");

// remove
Test.assert(() => Session.remove("Hello") === true, "Session.remove");
Test.assert(() => Session.get("Hello") === null, "Session.get for remove check");

// clear
Session.set("Hello", "World!");
Test.assert(() => Session.clear() === true, "Session.clear");
Test.assert(() => Session.get("Hello") === null, "Session.get for clear check");

// multiple data types
Session.set("Object", {first:"Hello", second:"World!"});
Test.assert(() => Session.get("Object").first == "Hello" && Session.get("Object").second == "World!", "Session.get for Objects");
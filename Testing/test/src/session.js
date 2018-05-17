// Session set and get
assert(() => Session.set("Hello", "World!") == undefined, "Session.set");
assert(() => Session.get("Hello") == "World!", "Session.set");

// getAll
assert(() => Session.getAll()["Hello"] == "World!", "Session.getAll");

// remove
assert(() => Session.remove("Hello") == undefined, "Session.remove");
assert(() => Session.get("Hello") == undefined, "Session.get for remove check");

// clear
Session.set("Hello", "World!");
assert(() => Session.clear() == undefined, "Session.clear");
assert(() => Session.get("Hello") == undefined, "Session.get for clear check");
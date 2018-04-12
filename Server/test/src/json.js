var s = JSON.stringify({x: {y: 3, z: true}});
assert(() => s == '{"x":{"z":true,"y":3}}', "JSON.stringify - " + s);

var data = JSON.parse('{"obj":{"b":3,"c":true},"string":"abc","number":4,"bool":true,"array":[1,2,3]}');
assert(() => data.obj.b == 3, "JSON.parse object");
assert(() => data.string == "abc", "JSON.parse string");
assert(() => data.number == 4, "JSON.parse number");
assert(() => data.bool == true, "JSON.parse bool");
assert(() => data.array.length == 3, "JSON.parse array length");
assert(() => data.array[1] == 2, "JSON.parse array item");
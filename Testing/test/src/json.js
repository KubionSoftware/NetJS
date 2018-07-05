var s = JSON.stringify({x: {y: 3, z: true}});
Test.assert(() => s == '{"x":{"y":3,"z":true}}', "JSON.stringify");

var data = JSON.parse('{"obj":{"b":3,"c":true},"string":"abc","number":4,"bool":true,"array":[1,2,3]}');
Test.assert(() => data.obj.b == 3, "JSON.parse object");
Test.assert(() => data.string == "abc", "JSON.parse string");
Test.assert(() => data.number == 4, "JSON.parse number");
Test.assert(() => data.bool == true, "JSON.parse bool");
Test.assert(() => data.array.length == 3, "JSON.parse array length");
Test.assert(() => data.array[1] == 2, "JSON.parse array item");
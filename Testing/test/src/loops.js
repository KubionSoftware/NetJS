var s = "";
for(var i = 0; i < 5; i++){
	s += i;
}
assert(() => s == "01234", "For i from 0 to 5 loop");

var items = ["carrot", "basket", "stick"];
s = "";
for(var item of items){
	s += item;
}
assert(() => s == "carrotbasketstick", "For item of items loop");

var obj = {
	x: 1,
	y: 5,
	z: 4
};
s = "";
var total = 0;
for(var key in obj){
	s += key;
	total += obj[key];
}
assert(() => s == "xyz", "For key in object loop (keys)");
assert(() => total == 10, "For key in object loop (values)");

s = "";
while(s.length < 5){
	s += "x";
}
assert(() => s == "xxxxx", "While < 5 loop");

s = "";
while(true){
	s += "x";
	if(s.length == 5) break;
}
assert(() => s == "xxxxx", "While break");
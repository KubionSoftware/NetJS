var characters = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f"];

var generate = function(length){
	var s = "";
	for(var i = 0; i < length; i++){
		var index = parseInt(Math.random() * characters.length);
		s += characters[index];
	}
	return s;
};

var start = new Date();
for(var i = 0; i < 100; i++){
	<div>generate(64)</div>
}
new Date().getTime() - start.getTime();
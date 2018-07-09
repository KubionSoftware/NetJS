require("buffer.js");
require("math.js");
require("string.js");
require("json.js");
require("functions.js");
require("loops.js");
require("if.js");
require("array.js");
require("operators.js");
require("html.js");
require("global.js");
require("regex.js");
require("date.js");
require("xdoc.js");
require("class.js");
require("http.js");
require("base64.js");
require("session.js");
require("include.js");

require("io.js");
require("sql.js");

async function asyncTests() {
	try {
		await testIO();
	} catch (e) {
		Test.assert(() => false, "IO - " + e);
	}

	try {
		await testSQL();
	} catch (e) {
		Test.assert(() => false, "SQL - " + e);
	}
}

asyncTests().then(() => {
	Test.time(() => {
		var ret = [], tmp, num = 500, i = 1024;
	
		for ( var j1 = 0; j1 < i * 15; j1++ ) {
			ret = [];
			ret.length = i;
		}
	
		for ( var j2 = 0; j2 < i * 10; j2++ ) {
			ret = new Array(i);
		}
	
		ret = [];
		for ( var j3 = 0; j3 < i; j3++ ) {
			ret.unshift(j3);
		}
	
		ret = [];
		for ( var j4 = 0; j4 < i; j4++ ) {
			ret.splice(0,0,j4);
		}
	
		var a = ret.slice();
		for ( var j5 = 0; j5 < i; j5++ ) {
			tmp = a.shift();
		}
	
		var b = ret.slice();
		for ( var j6 = 0; j6 < i; j6++ ) {
			tmp = b.splice(0,1);
		}
	
		ret = [];
		for ( var j7 = 0; j7 < i * 25; j7++ ) {
			ret.push(j7);
		}
	
		var c = ret.slice();
		for ( var j8 = 0; j8 < i * 25; j8++ ) {
			tmp = c.pop();
		}
	}, "Array speed test (200ms - 500ms)");

	end(Buffer.get());
});
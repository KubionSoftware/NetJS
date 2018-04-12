function span(x){
	<span>x</span>
}

assert(() => span(3) == "<span>3</span>", "Html simple");

function id(x){
	<span id="#x#"></span>
}
assert(() => id("some-id") == "<span id=\"some-id\"></span>", "Html inline variable");

function condition(context){
	<span #if(context == "kcc"){
		return "class='kcc'"
	}else if(context == "admin"){
		return "id='admin'"
	}#></span>
}
assert(() => condition("kcc") == "<span class='kcc'></span>", "Html inline if");
assert(() => condition("admin") == "<span id='admin'></span>", "Html inline if");
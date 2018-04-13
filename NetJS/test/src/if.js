if(false) {
	assert(() => false, "If false");
}else{
	assert(() => true, "If false");
}

if(true) {
	assert(() => true, "If true");
}else{
	assert(() => false, "If true");
}

var ageToText = function(age){
	if(age < 1){
		return "baby";
	}else if(age < 4){
		return "peuter";
	}else if(age < 6){
		return "kleuter";
	}else if(age < 12){
		return "kind";
	}else if(age < 18){
		return "puber";
	}else if(age < 21){
		return "jong volwassen";
	}else if(age < 65){
		return "volwassen";
	}else{
		return "bejaard";
	}
};
assert(() => ageToText(0.2) == "baby", "If elseif else");
assert(() => ageToText(3) == "peuter", "If elseif else");
assert(() => ageToText(11) == "kind", "If elseif else");
assert(() => ageToText(50) == "volwassen", "If elseif else");
assert(() => ageToText(88) == "bejaard", "If elseif else");

var logic = function(a, b){
	if(a){
		if(b){
			return "a and b";
		}else{
			return "a not b";
		}
	}else{
		if(b){
			return "not a and b";
		}else{
			return "not a and not b";
		}
	}
};
assert(() => logic(true, true) == "a and b", "If in if");
assert(() => logic(true, false) == "a not b", "If in if");
assert(() => logic(false, true) == "not a and b", "If in if");
assert(() => logic(false, false) == "not a and not b", "If in if");

var getString = function(value){
	switch(value){
		case 1:
			return "one";
		case 2:
			return "two";
		case 3:
			return "three";
		default:
			return "unknown";
	}
};
assert(() => getString(1) == "one", "Switch first");
assert(() => getString(3) == "three", "Switch last");
assert(() => getString(4) == "unknown", "Switch default");
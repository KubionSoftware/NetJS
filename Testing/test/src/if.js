if(false) {
	Test.assert(() => false, "If false");
}else{
	Test.assert(() => true, "If false");
}

if(true) {
	Test.assert(() => true, "If true");
}else{
	Test.assert(() => false, "If true");
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
Test.assert(() => ageToText(0.2) == "baby", "If elseif else");
Test.assert(() => ageToText(3) == "peuter", "If elseif else");
Test.assert(() => ageToText(11) == "kind", "If elseif else");
Test.assert(() => ageToText(50) == "volwassen", "If elseif else");
Test.assert(() => ageToText(88) == "bejaard", "If elseif else");

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
Test.assert(() => logic(true, true) == "a and b", "If in if");
Test.assert(() => logic(true, false) == "a not b", "If in if");
Test.assert(() => logic(false, true) == "not a and b", "If in if");
Test.assert(() => logic(false, false) == "not a and not b", "If in if");

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
Test.assert(() => getString(1) == "one", "Switch first");
Test.assert(() => getString(3) == "three", "Switch last");
Test.assert(() => getString(4) == "unknown", "Switch default");

Test.assert(() => (true ? "yes" : "no") == "yes", "Conditional operator true");
Test.assert(() => (false ? "yes" : "no") == "no", "Conditional operator false");
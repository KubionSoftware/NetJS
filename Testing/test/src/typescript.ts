var add = function(a: number, b: number){
	return a + b;
};

try{
	void add(3, 4);
	assert(() => true, "Typescript valid argument types succeeded");
}catch{
	assert(() => false, "Typescript valid argument types failed");
}

try{
	void add(3, "4");
	assert(() => false, "Typescript invalid argument types succeeded");
}catch{
	assert(() => true, "Typescript invalid argument types failed");
}

try{
	var name: string = "Bouke";
	assert(() => true, "Typescript valid variable declaration succeeded");
}catch{
	assert(() => false, "Typescript valid variable declaration failed");
}

try{
	var name: string = 3;
	assert(() => false, "Typescript invalid variable declaration succeeded");
}catch{
	assert(() => true, "Typescript invalid variable declaration failed");
}

try{
	var name: string = "Bouke";
	name = "Tom";
	assert(() => true, "Typescript valid variable assignment succeeded");
}catch{
	assert(() => false, "Typescript valid variable assignment failed");
}

try{
	var name: string = "Bouke";
	name = 3;
	assert(() => false, "Typescript invalid variable assignment succeeded");
}catch{
	assert(() => true, "Typescript invalid variable assignment failed");
}

try{
	var substract = function(a, b): number {
		return a - b;
	};

	void substract(3, 4);
	assert(() => true, "Typescript valid function return succeeded");
}catch{
	assert(() => false, "Typescript valid function return failed");
}

try{
	var substract = function(a, b): number {
		return "a - b";
	};

	void substract(3, 4);
	assert(() => false, "Typescript invalid function return succeeded");
}catch{
	assert(() => true, "Typescript invalid function return failed");
}

try{
	var numbers: number[] = [3, 4, 5];
	assert(() => true, "Typescript valid array succeeded");
}catch{
	assert(() => false, "Typescript valid array failed");
}

try{
	var numbers: number[] = [3, "4", 5];
	assert(() => false, "Typescript invalid array succeeded");
}catch{
	assert(() => true, "Typescript invalid array failed");
}

interface Box{
	length: number;
	width: number;
	height: number;
}

try{
	var box: Box = {
		length: 3,
		width: 5,
		height: 4
	};
	assert(() => true, "Typescript valid object interface succeeded");
}catch{
	assert(() => false, "Typescript valid object interface failed");
}

try{
	var box: Box = {
		length: "3",
		width: 5,
		height: 4
	};
	assert(() => false, "Typescript invalid object interface succeeded");
}catch{
	assert(() => true, "Typescript invalid object interface failed");
}

try{
	var box: Box = {
		lngt: 3,
		width: 5,
		height: 4
	};
	assert(() => false, "Typescript invalid object interface succeeded");
}catch{
	assert(() => true, "Typescript invalid object interface failed");
}

try{
	var x: any = true;
	x = 3;
	x = "text";
	assert(() => true, "Typescript any succeeded");

	var n: number = 3;
	assert(() => true, "Typescript number succeeded");

	var s: string = "text";
	assert(() => true, "Typescript string succeeded");

	var b: boolean = true;
	assert(() => true, "Typescript boolean succeeded");

	var o: object = {x: 3};
	assert(() => true, "Typescript object succeeded");

	var d: Date = new Date();
	assert(() => true, "Typescript date succeeded");

	var a: any[] = [3, "text", true];
	assert(() => true, "Typescript array any succeeded");

	var numbers: number[] = [3, 4, 5];
	assert(() => true, "Typescript array number succeeded");

	var strings: string[] = ["a", "b", "c"];
	assert(() => true, "Typescript array string succeeded");

	var booleans: boolean[] = [true, false, true];
	assert(() => true, "Typescript array boolean succeeded");

	var objects: object[] = [{}, {}, {}];
	assert(() => true, "Typescript array object succeeded");

	var dates: Date[] = [new Date(), new Date(), new Date()];
	assert(() => true, "Typescript array date succeeded");
} catch (e) {
	assert(() => false, "Typescript type failed: " + e);
}
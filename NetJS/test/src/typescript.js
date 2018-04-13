var add = function(a: number, b: number){
	return a + b;
};

try{
	void add(3, 4);
	assert(() => true, "Typescript valid argument types");
}catch{
	assert(() => false, "Typescript valid argument types");
}

try{
	void add(3, "4");
	assert(() => false, "Typescript invalid argument types");
}catch{
	assert(() => true, "Typescript invalid argument types");
}

try{
	var name: string = "Bouke";
	assert(() => true, "Typescript valid variable assignment");
}catch{
	assert(() => false, "Typescript valid variable assignment");
}

try{
	var name: string = 3;
	assert(() => false, "Typescript invalid variable assignment");
}catch{
	assert(() => true, "Typescript invalid variable assignment");
}

try{
	var substract = function(a, b): number {
		return a - b;
	};

	void substract(3, 4);
	assert(() => true, "Typescript valid function return");
}catch{
	assert(() => false, "Typescript valid function return");
}

try{
	var substract = function(a, b): number {
		return "a - b";
	};

	void substract(3, 4);
	assert(() => false, "Typescript invalid function return");
}catch{
	assert(() => true, "Typescript invalid function return");
}

try{
	var numbers: number[] = [3, 4, 5];
	assert(() => true, "Typescript valid array");
}catch{
	assert(() => false, "Typescript valid array");
}

try{
	var numbers: number[] = [3, "4", 5];
	assert(() => false, "Typescript invalid array");
}catch{
	assert(() => true, "Typescript invalid array");
}

try{
	var x: any = true;
	x = 3;
	x = "text";
	assert(() => true, "Typescript any");

	var n: number = 3;
	assert(() => true, "Typescript number");

	var s: string = "text";
	assert(() => true, "Typescript string");

	var b: boolean = true;
	assert(() => true, "Typescript boolean");

	var o: object = {x: 3};
	assert(() => true, "Typescript object");

	var d: Date = new Date();
	assert(() => true, "Typescript date");

	var a: any[] = [3, "text", true];
	assert(() => true, "Typescript array any");

	var numbers: number[] = [3, 4, 5];
	assert(() => true, "Typescript array number");

	var strings: string[] = ["a", "b", "c"];
	assert(() => true, "Typescript array string");

	var booleans: boolean[] = [true, false, true];
	assert(() => true, "Typescript array boolean");

	var objects: object[] = [{}, {}, {}];
	assert(() => true, "Typescript array object");

	var dates: Date[] = [new Date(), new Date(), new Date()];
	assert(() => true, "Typescript array date");
} catch (e) {
	assert(() => false, "Typescript type: " + e);
}
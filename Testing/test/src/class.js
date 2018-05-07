class Animal {
	speak(){
		return "Bleeh";
	}

	static compute(){
		return "No robot";
	}
}

class Dog extends Animal {
	constructor(name){
		this.name = name;
	}

	greet(){
		return "Hello " + this.name;
	}

	speak(){
		return "Woef";
	}
}

var animal = new Animal();
assert(() => animal.speak() == "Bleeh", "Class method");
assert(() => Animal.compute() == "No robot", "Class static method");

var dog = new Dog("Bello");
assert(() => dog.greet() == "Hello Bello", "Class constructor + this access");
assert(() => dog.speak() == "Woef", "Class method override");
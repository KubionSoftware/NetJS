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
		super();
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
Test.assert(() => animal.speak() == "Bleeh", "Class method");
Test.assert(() => Animal.compute() == "No robot", "Class static method");

var dog = new Dog("Bello");
Test.assert(() => dog.greet() == "Hello Bello", "Class constructor + this access");
Test.assert(() => dog.speak() == "Woef", "Class method override");
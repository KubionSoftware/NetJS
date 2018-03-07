var numbers = [3, 4, 2, 5];
assert(() => numbers.length == 4, "Array.length");

var array = new Array(10);
assert(() => array.length == 10, "Array constructor function");

assert(() => numbers[0] == 3, "Array get");
assert(() => numbers[9] == undefined, "Array get > length");
assert(() => numbers[-2] == undefined, "Array get < 0");
numbers[0] = 6;
assert(() => numbers[0] == 6, "Array assign");
numbers[0] = 3;
assert(() => numbers[0] == 3, "Array assign");

var total = 0;
numbers.forEach(n => total += n);
assert(() => total == 14, "Array.forEach");

numbers.push(7);
assert(() => numbers.length == 5, "Array.push length");
assert(() => numbers[numbers.length - 1] == 7, "Array.push item");

var last = numbers.pop();
assert(() => numbers.length == 4, "Array.pop length");
assert(() => last == 7, "Array.pop item");

var first = numbers.shift();
assert(() => numbers.length == 3, "Array.shift length");
assert(() => first == 3, "Array.shift item");
assert(() => numbers[0] == 4, "Array.shift first item");

numbers.unshift(3);
assert(() => numbers.length == 4, "Array.unshift length");
assert(() => numbers[0] == 3, "Array.unshift item");

var fruit = ["apple", "orange", "banana", "strawberry", "melon"];
assert(() => fruit.indexOf("banana") == 2, "Array.indexOf in array");
assert(() => fruit.indexOf("grapes") == -1, "Array.indexOf not in array");

fruit.splice(2, 0, "grapes");
assert(() => fruit.length == 6, "Array.splice add length");
assert(() => fruit[2] == "grapes", "Array.splice item");

fruit.splice(2, 1);
assert(() => fruit.length == 5, "Array.splice remove length");

var sub = fruit.slice(2, 4);
assert(() => fruit.length == 5, "Array.slice not modified");
assert(() => sub.length == 2, "Array.slice length");
assert(() => sub[0] == "banana", "Array.slice item");

var numbers = [1, 2, 3];

var squareRoots = numbers.map(n => n * n);
assert(() => squareRoots.length == 3, "Array.map length");
assert(() => squareRoots[1] == 4, "Array.map item");

var total = numbers.reduce((acc, val) => acc + val, 0);
assert(() => total == 6, "Array.reduce");

var numbers = [1, 2, 3, 4, 5, 6];
var evens = numbers.filter(n => n % 2 == 0);
assert(() => evens.length == 3, "Array.filter length");
assert(() => evens[1] == 4, "Array.filter item");

var numbers = [5, 3, 8, 4, 2];
numbers.sort((a, b) => a - b);
assert(() => numbers.length == 5, "Array.sort length");
assert(() => numbers[0] == 2, "Array.sort first item");
assert(() => numbers[4] == 8, "Array.sort last item");

var numbers = [3, 2, 5];
var s = numbers.join(",");
assert(() => s == "3,2,5", "Array.join");

var numbers = [3, 5, 2];
assert(() => numbers.some(n => n > 5) == false, "Array.some false");
assert(() => numbers.some(n => n > 4) == true, "Array.some true");

assert(() => numbers.every(n => n < 6) == true, "Array.every true");
assert(() => numbers.every(n => n < 3) == false, "Array.every false");

assert(() => numbers.find(n => n > 4) == 5, "Array.find");
assert(() => numbers.findIndex(n => n > 4) == 1, "Array.findIndex");

assert(() => numbers.includes(5) == true, "Array.includes true");
assert(() => numbers.includes(6) == false, "Array.includes false");

numbers.reverse();
assert(() => numbers[0] == 2 && numbers[1] == 5 && numbers[2] == 3, "Array.reverse odd length");

var numbers = [2, 3, 4, 5].reverse();
assert(() => numbers[0] == 5 && numbers[1] == 4 && numbers[2] == 3 && numbers[3] == 2, "Array.reverse even length");
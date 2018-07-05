var numbers = [3, 4, 2, 5];
Test.assert(() => numbers.length == 4, "Array.length");

var array = new Array(10);
Test.assert(() => array.length == 10, "Array constructor function");

Test.assert(() => numbers[0] == 3, "Array get");
Test.assert(() => numbers[9] == undefined, "Array get > length");
Test.assert(() => numbers[-2] == undefined, "Array get < 0");
numbers[0] = 6;
Test.assert(() => numbers[0] == 6, "Array assign");
numbers[0] = 3;
Test.assert(() => numbers[0] == 3, "Array assign");

var total = 0;
numbers.forEach(n => total += n);
Test.assert(() => total == 14, "Array.forEach");

numbers.push(7);
Test.assert(() => numbers.length == 5, "Array.push length");
Test.assert(() => numbers[numbers.length - 1] == 7, "Array.push item");

var last = numbers.pop();
Test.assert(() => numbers.length == 4, "Array.pop length");
Test.assert(() => last == 7, "Array.pop item");

var first = numbers.shift();
Test.assert(() => numbers.length == 3, "Array.shift length");
Test.assert(() => first == 3, "Array.shift item");
Test.assert(() => numbers[0] == 4, "Array.shift first item");

numbers.unshift(3);
Test.assert(() => numbers.length == 4, "Array.unshift length");
Test.assert(() => numbers[0] == 3, "Array.unshift item");

var fruit = ["apple", "orange", "banana", "strawberry", "melon"];
Test.assert(() => fruit.indexOf("banana") == 2, "Array.indexOf in array");
Test.assert(() => fruit.indexOf("grapes") == -1, "Array.indexOf not in array");

fruit.splice(2, 0, "grapes");
Test.assert(() => fruit.length == 6, "Array.splice add length");
Test.assert(() => fruit[2] == "grapes", "Array.splice item");

fruit.splice(2, 1);
Test.assert(() => fruit.length == 5, "Array.splice remove length");

var sub = fruit.slice(2, 4);
Test.assert(() => fruit.length == 5, "Array.slice not modified");
Test.assert(() => sub.length == 2, "Array.slice length");
Test.assert(() => sub[0] == "banana", "Array.slice item");

numbers = [1, 2, 3];

var squareRoots = numbers.map(n => n * n);
Test.assert(() => squareRoots.length == 3, "Array.map length");
Test.assert(() => squareRoots[1] == 4, "Array.map item");

total = numbers.reduce((acc, val) => acc + val, 0);
Test.assert(() => total == 6, "Array.reduce");

numbers = [1, 2, 3, 4, 5, 6];
var evens = numbers.filter(n => n % 2 == 0);
Test.assert(() => evens.length == 3, "Array.filter length");
Test.assert(() => evens[1] == 4, "Array.filter item");

numbers = [5, 3, 8, 4, 2];
numbers.sort((a, b) => a - b);
Test.assert(() => numbers.length == 5, "Array.sort length");
Test.assert(() => numbers[0] == 2, "Array.sort first item");
Test.assert(() => numbers[4] == 8, "Array.sort last item");

numbers = [3, 2, 5];
var s = numbers.join(",");
Test.assert(() => s == "3,2,5", "Array.join");

numbers = [3, 5, 2];
Test.assert(() => numbers.some(n => n > 5) == false, "Array.some false");
Test.assert(() => numbers.some(n => n > 4) == true, "Array.some true");

Test.assert(() => numbers.every(n => n < 6) == true, "Array.every true");
Test.assert(() => numbers.every(n => n < 3) == false, "Array.every false");

Test.assert(() => numbers.find(n => n > 4) == 5, "Array.find");
Test.assert(() => numbers.findIndex(n => n > 4) == 1, "Array.findIndex");

Test.assert(() => numbers.includes(5) == true, "Array.includes true");
Test.assert(() => numbers.includes(6) == false, "Array.includes false");

numbers.reverse();
Test.assert(() => numbers[0] == 2 && numbers[1] == 5 && numbers[2] == 3, "Array.reverse odd length");

numbers = [2, 3, 4, 5].reverse();
Test.assert(() => numbers[0] == 5 && numbers[1] == 4 && numbers[2] == 3 && numbers[3] == 2, "Array.reverse even length");
var date = new Date(2018, 1, 13, 12, 41, 38, 420);

assert(() => date.getFullYear() == 2018, "Date getFullYear");
assert(() => date.getMonth() == 1, "Date getMonth");
assert(() => date.getDate() == 13, "Date getDate");
assert(() => date.getDay() == 2, "Date getDay");
assert(() => date.getHours() == 12, "Date getHours");
assert(() => date.getMinutes() == 41, "Date getMinutes");
assert(() => date.getSeconds() == 38, "Date getSeconds");
assert(() => date.getMilliseconds() == 420, "Date getMilliseconds");

void date.setFullYear(2016);
assert(() => date.getFullYear() == 2016, "Date setFullYear");
void date.setMonth(3);
assert(() => date.getMonth() == 3, "Date setMonth");
void date.setDate(24);
assert(() => date.getDate() == 24, "Date setDate");
void date.setHours(17);
assert(() => date.getHours() == 17, "Date setHours");
void date.setMinutes(58);
assert(() => date.getMinutes() == 58, "Date setMinutes");
void date.setSeconds(15);
assert(() => date.getSeconds() == 15, "Date setSeconds");
void date.setMilliseconds(784);
assert(() => date.getMilliseconds() == 784, "Date setMilliseconds");
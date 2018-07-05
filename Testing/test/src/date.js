var date = new Date(2018, 1, 13, 12, 41, 38, 420);

Test.assert(() => date.getFullYear() == 2018, "Date getFullYear");
Test.assert(() => date.getMonth() == 1, "Date getMonth");
Test.assert(() => date.getDate() == 13, "Date getDate");
Test.assert(() => date.getDay() == 2, "Date getDay");
Test.assert(() => date.getHours() == 12, "Date getHours");
Test.assert(() => date.getMinutes() == 41, "Date getMinutes");
Test.assert(() => date.getSeconds() == 38, "Date getSeconds");
Test.assert(() => date.getMilliseconds() == 420, "Date getMilliseconds");

void date.setFullYear(2016);
Test.assert(() => date.getFullYear() == 2016, "Date setFullYear");
void date.setMonth(3);
Test.assert(() => date.getMonth() == 3, "Date setMonth");
void date.setDate(24);
Test.assert(() => date.getDate() == 24, "Date setDate");
void date.setHours(17);
Test.assert(() => date.getHours() == 17, "Date setHours");
void date.setMinutes(58);
Test.assert(() => date.getMinutes() == 58, "Date setMinutes");
void date.setSeconds(15);
Test.assert(() => date.getSeconds() == 15, "Date setSeconds");
void date.setMilliseconds(784);
Test.assert(() => date.getMilliseconds() == 784, "Date setMilliseconds");
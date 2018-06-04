using NetJS.Core.Javascript;
using System;
using System.Globalization;

namespace NetJS.Core.API {
    class Date {

        public static Constant constructor(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            if(arguments.Length == 1) {
                if(arguments[0] is Javascript.String) {
                    var dateString = Tool.GetArgument<Javascript.String>(arguments, 0, "Date constructor").Value;
                    return new Javascript.Date(DateTime.Parse(dateString));
                }else if(arguments[0] is Javascript.Number) {
                    var milliseconds = Tool.GetArgument<Javascript.Number>(arguments, 0, "Date constructor").Value;
                    return new Javascript.Date(Convert.UnixMillisecondsToDateTime(milliseconds));
                }
            }else if(arguments.Length >= 2) {
                var year = (int)Tool.GetArgument<Javascript.Number>(arguments, 0, "Date constructor").Value;
                var month = (int)Tool.GetArgument<Javascript.Number>(arguments, 1, "Date constructor").Value;
                var day = arguments.Length > 2 ? (int)Tool.GetArgument<Javascript.Number>(arguments, 2, "Date constructor").Value : 0;
                var hours = arguments.Length > 3 ? (int)Tool.GetArgument<Javascript.Number>(arguments, 3, "Date constructor").Value : 0;
                var minutes = arguments.Length > 4 ? (int)Tool.GetArgument<Javascript.Number>(arguments, 4, "Date constructor").Value : 0;
                var seconds = arguments.Length > 5 ? (int)Tool.GetArgument<Javascript.Number>(arguments, 5, "Date constructor").Value : 0;
                var milliseconds = arguments.Length > 6 ? (int)Tool.GetArgument<Javascript.Number>(arguments, 6, "Date constructor").Value : 0;

                var date = new DateTime(year, month + 1, day, hours, minutes, seconds, milliseconds);
                return new Javascript.Date(date);
            }

            var now = DateTime.Now;
            return new Javascript.Date(now);
        }

        public static Constant toString(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            return new Javascript.String(toDateString(_this, arguments, lex).ToString() + " " + toTimeString(_this, arguments, lex).ToString());
        }

        public static Constant toISOString(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            return new Javascript.String(date.Value.ToUniversalTime().ToString("o"));
        }

        public static Constant toDateString(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            return new Javascript.String(date.Value.ToString("ddd MMM dd yyyy"));
        }

        public static Constant toTimeString(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            var timeZoneName = TimeZoneInfo.Local.StandardName;
            var timeZoneTime = date.Value.ToString("zzz").Replace(":", "");
            return new Javascript.String(date.Value.ToString("HH:mm:ss") + " GMT" + timeZoneTime + " (" + timeZoneName + ")");
        }

        public static Constant toUTCString(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            return new Javascript.String(date.Value.ToString("ddd MMM dd yyyy HH:mm:ss") + " GMT");
        }

        public static Constant getFullYear(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            return new Javascript.Number(date.Value.Year);
        }

        public static Constant getMonth(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            return new Javascript.Number(date.Value.Month - 1);
        }

        public static Constant getDate(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this; ;
            return new Javascript.Number(date.Value.Day);
        }

        public static Constant getDay(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            return new Javascript.Number((int)date.Value.DayOfWeek);
        }

        public static Constant getHours(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            return new Javascript.Number(date.Value.Hour);
        }

        public static Constant getMinutes(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            return new Javascript.Number(date.Value.Minute);
        }

        public static Constant getSeconds(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            return new Javascript.Number(date.Value.Second);
        }

        public static Constant getMilliseconds(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            return new Javascript.Number(date.Value.Millisecond);
        }

        public static Constant getTime(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            return new Javascript.Number(Convert.DateTimeToUnixMilliseconds(date.Value));
        }

        public static Constant setFullYear(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            date.Value = date.Value.AddYears((int)Tool.GetArgument<Javascript.Number>(arguments, 0, "Date.setFullYear").Value - date.Value.Year);
            return getTime(_this, new Constant[] { }, lex);
        }

        public static Constant setMonth(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            date.Value = date.Value.AddMonths(((int)Tool.GetArgument<Javascript.Number>(arguments, 0, "Date.setMonth").Value + 1) - date.Value.Month);
            return getTime(_this, new Constant[] { }, lex);
        }

        public static Constant setDate(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this; ;
            date.Value = date.Value.AddDays((int)Tool.GetArgument<Javascript.Number>(arguments, 0, "Date.setDate").Value - date.Value.Day);
            return getTime(_this, new Constant[] { }, lex);
        }

        public static Constant setHours(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            date.Value = date.Value.AddHours((int)Tool.GetArgument<Javascript.Number>(arguments, 0, "Date.setHours").Value - date.Value.Hour);
            return getTime(_this, new Constant[] { }, lex);
        }

        public static Constant setMinutes(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            date.Value = date.Value.AddMinutes((int)Tool.GetArgument<Javascript.Number>(arguments, 0, "Date.setMinutes").Value - date.Value.Minute);
            return getTime(_this, new Constant[] { }, lex);
        }

        public static Constant setSeconds(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            date.Value = date.Value.AddSeconds((int)Tool.GetArgument<Javascript.Number>(arguments, 0, "Date.setSeconds").Value - date.Value.Second);
            return getTime(_this, new Constant[] { }, lex);
        }

        public static Constant setMilliseconds(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            date.Value = date.Value.AddMilliseconds((int)Tool.GetArgument<Javascript.Number>(arguments, 0, "Date.setMilliseconds").Value - date.Value.Millisecond);
            return getTime(_this, new Constant[] { }, lex);
        }

        public static Constant setTime(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var date = (Javascript.Date)_this;
            date.Value = Convert.UnixMillisecondsToDateTime((int)Tool.GetArgument<Javascript.Number>(arguments, 0, "Date.setTime").Value);
            return getTime(_this, new Constant[] { }, lex);
        }
    }
}
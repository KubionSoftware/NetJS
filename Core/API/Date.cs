using NetJS.Core;
using System;
using System.Globalization;

namespace NetJS.Core.API {
    class DateAPI {

        public const string Primitive = "[[PrimitiveValue]]";

        public static Constant constructor(Constant _this, Constant[] arguments, Agent agent) {
            DateTime date;

            if(arguments.Length == 1) {
                if(arguments[0] is String) {
                    var dateString = Tool.GetArgument<String>(arguments, 0, "Date constructor").Value;
                    date = DateTime.Parse(dateString);
                }else if(arguments[0] is Number) {
                    var milliseconds = Tool.GetArgument<Number>(arguments, 0, "Date constructor").Value;
                    date = Convert.UnixMillisecondsToDateTime(milliseconds);
                } else {
                    date = DateTime.Now;
                }
            }else if(arguments.Length >= 2) {
                var year = (int)Tool.GetArgument<Number>(arguments, 0, "Date constructor").Value;
                var month = (int)Tool.GetArgument<Number>(arguments, 1, "Date constructor").Value;
                var day = arguments.Length > 2 ? (int)Tool.GetArgument<Number>(arguments, 2, "Date constructor").Value : 0;
                var hours = arguments.Length > 3 ? (int)Tool.GetArgument<Number>(arguments, 3, "Date constructor").Value : 0;
                var minutes = arguments.Length > 4 ? (int)Tool.GetArgument<Number>(arguments, 4, "Date constructor").Value : 0;
                var seconds = arguments.Length > 5 ? (int)Tool.GetArgument<Number>(arguments, 5, "Date constructor").Value : 0;
                var milliseconds = arguments.Length > 6 ? (int)Tool.GetArgument<Number>(arguments, 6, "Date constructor").Value : 0;

                date = new DateTime(year, month + 1, day, hours, minutes, seconds, milliseconds);
            } else {
                date = DateTime.Now;
            }

            var o = Tool.Construct("Date", agent);
            o.Set(Primitive, new Foreign(date), agent);
            return o;
        }

        private static DateTime GetDate(Constant _this, Agent agent) {
            return (DateTime)((_this as Object).Get(Primitive, agent) as Foreign).Value;
        }

        private static void SetDate(Constant _this, DateTime date, Agent agent) {
            ((_this as Object).Get(Primitive, agent) as Foreign).Value = date;
        }

        public static Constant toString(Constant _this, Constant[] arguments, Agent agent) {
            return new String(toDateString(_this, arguments, agent).ToString() + " " + toTimeString(_this, arguments, agent).ToString());
        }

        public static Constant toISOString(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            return new String(date.ToUniversalTime().ToString("o"));
        }

        public static Constant toDateString(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            return new String(date.ToString("ddd MMM dd yyyy"));
        }

        public static Constant toTimeString(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            var timeZoneName = TimeZoneInfo.Local.StandardName;
            var timeZoneTime = date.ToString("zzz").Replace(":", "");
            return new String(date.ToString("HH:mm:ss") + " GMT" + timeZoneTime + " (" + timeZoneName + ")");
        }

        public static Constant toUTCString(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            return new String(date.ToString("ddd MMM dd yyyy HH:mm:ss") + " GMT");
        }

        public static Constant getFullYear(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            return new Number(date.Year);
        }

        public static Constant getMonth(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            return new Number(date.Month - 1);
        }

        public static Constant getDate(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            return new Number(date.Day);
        }

        public static Constant getDay(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            return new Number((int)date.DayOfWeek);
        }

        public static Constant getHours(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            return new Number(date.Hour);
        }

        public static Constant getMinutes(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            return new Number(date.Minute);
        }

        public static Constant getSeconds(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            return new Number(date.Second);
        }

        public static Constant getMilliseconds(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            return new Number(date.Millisecond);
        }

        public static Constant getTime(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            return new Number(Convert.DateTimeToUnixMilliseconds(date));
        }

        public static Constant setFullYear(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            var year = (int)Tool.GetArgument<Number>(arguments, 0, "Date.setFullYear").Value;
            SetDate(_this, date.AddYears(year - date.Year), agent);
            return getTime(_this, new Constant[] { }, agent);
        }

        public static Constant setMonth(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            var month = (int)Tool.GetArgument<Number>(arguments, 0, "Date.setMonth").Value + 1;
            SetDate(_this, date.AddMonths(month - date.Month), agent);
            return getTime(_this, new Constant[] { }, agent);
        }

        public static Constant setDate(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            var d = (int)Tool.GetArgument<Number>(arguments, 0, "Date.setDate").Value;
            SetDate(_this, date.AddDays(d - date.Day), agent);
            return getTime(_this, new Constant[] { }, agent);
        }

        public static Constant setHours(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            var hour = (int)Tool.GetArgument<Number>(arguments, 0, "Date.setHours").Value;
            SetDate(_this, date.AddHours(hour - date.Hour), agent);
            return getTime(_this, new Constant[] { }, agent);
        }

        public static Constant setMinutes(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            var minute = (int)Tool.GetArgument<Number>(arguments, 0, "Date.setMinutes").Value;
            SetDate(_this, date.AddMinutes(minute - date.Minute), agent);
            return getTime(_this, new Constant[] { }, agent);
        }

        public static Constant setSeconds(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            var second = (int)Tool.GetArgument<Number>(arguments, 0, "Date.setSeconds").Value;
            SetDate(_this, date.AddSeconds(second - date.Second), agent);
            return getTime(_this, new Constant[] { }, agent);
        }

        public static Constant setMilliseconds(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            var ms = (int)Tool.GetArgument<Number>(arguments, 0, "Date.setMilliseconds").Value;
            SetDate(_this, date.AddMilliseconds(ms - date.Millisecond), agent);
            return getTime(_this, new Constant[] { }, agent);
        }

        public static Constant setTime(Constant _this, Constant[] arguments, Agent agent) {
            var date = GetDate(_this, agent);
            var time = (int)Tool.GetArgument<Number>(arguments, 0, "Date.setTime").Value;
            SetDate(_this, Convert.UnixMillisecondsToDateTime(time), agent);
            return getTime(_this, new Constant[] { }, agent);
        }
    }
}
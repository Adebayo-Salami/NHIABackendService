using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHIABackendService.Core.Extensions
{
    public static class DateExtensions
    {
        public static DateTime GetDateUtcNow(this DateTime now)
        {
            return DateTime.UtcNow;
        }

        public static DateTime ToInvariantDateTime(this string value, string format, out bool succeeded)
        {
            var dtfi = DateTimeFormatInfo.InvariantInfo;
            var result = DateTime.TryParseExact(value, format, dtfi, DateTimeStyles.None, out var newValue);
            succeeded = result;
            return newValue;
        }

        public static string ToDateString(this DateTime date, string format)
        {
            return date.ToString(format, DateTimeFormatInfo.InvariantInfo);
        }
    }
}

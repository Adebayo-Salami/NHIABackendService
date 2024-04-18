using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHIABackendService.Core.Timing
{
    public class UtcClockProvider : IClockProvider
    {
        internal UtcClockProvider()
        {
        }

        public DateTime Now => DateTime.UtcNow;

        public DateTimeKind Kind => DateTimeKind.Utc;

        public bool SupportsMultipleTimezone => true;

        public DateTime Normalize(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified) return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

            if (dateTime.Kind == DateTimeKind.Local) return dateTime.ToUniversalTime();

            return dateTime;
        }
    }
}

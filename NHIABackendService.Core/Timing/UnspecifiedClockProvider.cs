using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHIABackendService.Core.Timing
{
    public class UnspecifiedClockProvider : IClockProvider
    {
        internal UnspecifiedClockProvider()
        {
        }

        public DateTime Now => DateTime.Now;

        public DateTimeKind Kind => DateTimeKind.Unspecified;

        public bool SupportsMultipleTimezone => false;

        public DateTime Normalize(DateTime dateTime)
        {
            return dateTime;
        }
    }
}

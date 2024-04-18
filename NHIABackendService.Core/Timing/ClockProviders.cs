﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHIABackendService.Core.Timing
{
    public static class ClockProviders
    {
        public static UnspecifiedClockProvider Unspecified { get; } = new UnspecifiedClockProvider();

        public static LocalClockProvider Local { get; } = new LocalClockProvider();

        public static UtcClockProvider Utc { get; } = new UtcClockProvider();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHIABackendService.Core.ViewModels.Enums
{
    public enum FilterOperation
    {
        CONTAINS = 1,

        STARTSWITH = 2,

        ENDSWITH = 3,

        GREATERTHAN = 4,

        LESSTHAN = 5,

        EQUALS = 6,

        NOTEQUALS = 7,

        GREATERTHANOREQUALS = 8,

        LESSTHANOREQUALS = 9,

        ASCENDING = 10,

        Descending = 11,

        DATEBEFORE = 12,

        DATEAFTER = 13,
    }
}

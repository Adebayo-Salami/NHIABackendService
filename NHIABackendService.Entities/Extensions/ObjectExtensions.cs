﻿using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

#nullable disable

namespace NHIABackendService.Entities.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ObjectExtensions
    {
        public static T As<T>(this object obj) where T : class
        {
            return (T)obj;
        }

        public static T To<T>(this object obj) where T : struct
        {
            if (typeof(T) == typeof(Guid))
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj.ToString());

            return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }

        public static bool IsIn<T>(this T item, params T[] list)
        {
            return list.Contains(item);
        }
    }
}

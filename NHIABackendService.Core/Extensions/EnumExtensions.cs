﻿using System;
using System.ComponentModel;

#nullable disable

namespace NHIABackendService.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (name != null)
            {
                var field = type.GetField(name);
                if (field != null)
                {
                    var attr =
                        Attribute.GetCustomAttribute(field,
                            typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                        return attr.Description;
                    return value.ToString();
                }
            }

            return null;
        }

        public static T GetValueFromString<T>(this Enum value, string str) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");

            Enum.TryParse(value.GetType(), str, out var result);
            return (T)result;
        }
    }
}

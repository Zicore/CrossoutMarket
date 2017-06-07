using System;

namespace Crossout.Model
{
    public static class TypeConverter
    {
        /// <summary>
        /// Konvertiert Wert in Angegebenen Ziel Wert unter Beachtung von DBNull Werten
        /// </summary>
        /// <typeparam name="T">Ziel Typ</typeparam>
        /// <param name="obj">Wert</param>
        /// <param name="formatProvider">formatProvider</param>
        /// <returns>Gibt konvertierten Wert zurück</returns>
        public static T ConvertTo<T>(this Object obj, IFormatProvider formatProvider = null)
        {
            Type t = typeof(T);

            if (t.IsGenericType
                && (t.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                if (obj == null)
                {
                    return (T)(object)null;
                }
                else
                {
                    if (Convert.IsDBNull(obj))
                    {
                        return default(T);
                    }
                    else
                    {
                        if (formatProvider == null)
                        {
                            return (T) Convert.ChangeType(obj, Nullable.GetUnderlyingType(t));
                        }
                        else
                        {
                            return (T)Convert.ChangeType(obj, Nullable.GetUnderlyingType(t), formatProvider);
                        }
                    }
                }
            }
            else
            {
                if (Convert.IsDBNull(obj))
                {
                    return default(T);
                }
                else
                {
                    if (formatProvider == null)
                    {
                        return (T)Convert.ChangeType(obj, t);
                    }
                    else
                    {
                        return (T)Convert.ChangeType(obj, t, formatProvider);
                    }
                }
            }
        }

        public static string FilterNull(this string value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            return value;
        }

        public static string FilterNull(this string value, string defaultValue)
        {
            if (value == null)
            {
                return defaultValue;
            }
            return value;
        }

        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        /// <summary>
        /// Konvertiert Wert in Angegebenen Ziel Wert unter Beachtung von DBNull Werten
        /// </summary>
        /// <typeparam name="T">Ziel Typ</typeparam>
        /// <param name="obj">Wert</param>
        /// <param name="type">Target type</param>
        /// <returns>Gibt konvertierten Wert zurück</returns>
        public static object ChangeType(Object obj, Type type)
        {
            if (type.IsGenericType
                && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                if (obj == null)
                {
                    return (object)null;
                }
                else
                {
                    if (Convert.IsDBNull(obj))
                    {
                        return GetDefault(type);
                    }
                    else
                    {
                        return Convert.ChangeType(obj, Nullable.GetUnderlyingType(type));
                    }
                }
            }
            else
            {
                if (Convert.IsDBNull(obj))
                {
                    return GetDefault(type);
                }
                else
                {
                    return Convert.ChangeType(obj, type);
                }
            }
        }
    }
}

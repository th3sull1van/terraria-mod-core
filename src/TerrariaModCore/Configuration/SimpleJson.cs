using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace TerrariaModCore.Configuration
{
    /// <summary>
    /// Robust, lightweight JSON serialization and deserialization engine with 0 external dependencies.
    /// Handles primitives, booleans, floating-points, nested objects, lists, and dictionaries.
    /// </summary>
    public static class SimpleJson
    {
        public static string Serialize(object obj, bool pretty = true)
        {
            var sb = new StringBuilder();
            SerializeValue(obj, sb, pretty ? 0 : -1);
            return sb.ToString();
        }

        public static T Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return default;
            int index = 0;
            object parsed = ParseValue(json, ref index);
            return (T)ConvertValue(parsed, typeof(T));
        }

        public static object Deserialize(string json, Type targetType)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            int index = 0;
            object parsed = ParseValue(json, ref index);
            return ConvertValue(parsed, targetType);
        }

        #region Serialization

        private static void SerializeValue(object value, StringBuilder sb, int indent)
        {
            if (value == null)
            {
                sb.Append("null");
                return;
            }

            if (value is bool b)
            {
                sb.Append(b ? "true" : "false");
                return;
            }

            if (value is string s)
            {
                sb.Append('"');
                foreach (char c in s)
                {
                    switch (c)
                    {
                        case '"': sb.Append("\\\""); break;
                        case '\\': sb.Append("\\\\"); break;
                        case '\b': sb.Append("\\b"); break;
                        case '\f': sb.Append("\\f"); break;
                        case '\n': sb.Append("\\n"); break;
                        case '\r': sb.Append("\\r"); break;
                        case '\t': sb.Append("\\t"); break;
                        default: sb.Append(c); break;
                    }
                }
                sb.Append('"');
                return;
            }

            if (value is sbyte || value is byte || value is short || value is ushort ||
                value is int || value is uint || value is long || value is ulong)
            {
                sb.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
                return;
            }

            if (value is float f)
            {
                sb.Append(f.ToString("G9", CultureInfo.InvariantCulture));
                return;
            }

            if (value is double d)
            {
                sb.Append(d.ToString("G17", CultureInfo.InvariantCulture));
                return;
            }

            if (value is decimal dec)
            {
                sb.Append(dec.ToString(CultureInfo.InvariantCulture));
                return;
            }

            if (value is Enum)
            {
                sb.Append('"').Append(value.ToString()).Append('"');
                return;
            }

            if (value is IDictionary dict)
            {
                SerializeDictionary(dict, sb, indent);
                return;
            }

            if (value is IEnumerable enumerable)
            {
                SerializeArray(enumerable, sb, indent);
                return;
            }

            SerializeObject(value, sb, indent);
        }

        private static void SerializeDictionary(IDictionary dict, StringBuilder sb, int indent)
        {
            sb.Append('{');
            bool pretty = indent >= 0;
            int nextIndent = pretty ? indent + 2 : -1;
            bool first = true;

            foreach (DictionaryEntry entry in dict)
            {
                if (!first) sb.Append(',');
                if (pretty) { sb.AppendLine(); sb.Append(' ', nextIndent); }
                sb.Append('"').Append(entry.Key).Append("\": ");
                SerializeValue(entry.Value, sb, nextIndent);
                first = false;
            }

            if (pretty && !first) { sb.AppendLine(); sb.Append(' ', indent); }
            sb.Append('}');
        }

        private static void SerializeArray(IEnumerable enumerable, StringBuilder sb, int indent)
        {
            sb.Append('[');
            bool pretty = indent >= 0;
            int nextIndent = pretty ? indent + 2 : -1;
            bool first = true;

            foreach (var item in enumerable)
            {
                if (!first) sb.Append(',');
                if (pretty) { sb.AppendLine(); sb.Append(' ', nextIndent); }
                SerializeValue(item, sb, nextIndent);
                first = false;
            }

            if (pretty && !first) { sb.AppendLine(); sb.Append(' ', indent); }
            sb.Append(']');
        }

        private static void SerializeObject(object obj, StringBuilder sb, int indent)
        {
            sb.Append('{');
            bool pretty = indent >= 0;
            int nextIndent = pretty ? indent + 2 : -1;
            bool first = true;

            Type type = obj.GetType();
            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var p in props)
            {
                if (!p.CanRead || p.GetIndexParameters().Length > 0) continue;
                object val = p.GetValue(obj, null);
                if (!first) sb.Append(',');
                if (pretty) { sb.AppendLine(); sb.Append(' ', nextIndent); }
                sb.Append('"').Append(p.Name).Append("\": ");
                SerializeValue(val, sb, nextIndent);
                first = false;
            }

            foreach (var f in fields)
            {
                object val = f.GetValue(obj);
                if (!first) sb.Append(',');
                if (pretty) { sb.AppendLine(); sb.Append(' ', nextIndent); }
                sb.Append('"').Append(f.Name).Append("\": ");
                SerializeValue(val, sb, nextIndent);
                first = false;
            }

            if (pretty && !first) { sb.AppendLine(); sb.Append(' ', indent); }
            sb.Append('}');
        }

        #endregion

        #region Deserialization Parsing

        private static object ParseValue(string json, ref int index)
        {
            SkipWhitespace(json, ref index);
            if (index >= json.Length) return null;

            char c = json[index];
            if (c == '{') return ParseObject(json, ref index);
            if (c == '[') return ParseArray(json, ref index);
            if (c == '"') return ParseString(json, ref index);
            if (c == 't' || c == 'f') return ParseBool(json, ref index);
            if (c == 'n') return ParseNull(json, ref index);
            if (char.IsDigit(c) || c == '-') return ParseNumber(json, ref index);

            throw new FormatException($"Unexpected character '{c}' at position {index} in JSON: {json}");
        }

        private static void SkipWhitespace(string json, ref int index)
        {
            while (index < json.Length && char.IsWhiteSpace(json[index])) index++;
        }

        private static Dictionary<string, object> ParseObject(string json, ref int index)
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            index++; // skip '{'

            while (true)
            {
                SkipWhitespace(json, ref index);
                if (index >= json.Length) throw new FormatException("Unterminated JSON object");
                if (json[index] == '}') { index++; break; }

                string key = ParseString(json, ref index);
                SkipWhitespace(json, ref index);
                if (index >= json.Length || json[index] != ':') throw new FormatException($"Expected ':' at position {index}");
                index++; // skip ':'

                object val = ParseValue(json, ref index);
                dict[key] = val;

                SkipWhitespace(json, ref index);
                if (index < json.Length && json[index] == ',') { index++; continue; }
                if (index < json.Length && json[index] == '}') { index++; break; }
            }

            return dict;
        }

        private static List<object> ParseArray(string json, ref int index)
        {
            var list = new List<object>();
            index++; // skip '['

            while (true)
            {
                SkipWhitespace(json, ref index);
                if (index >= json.Length) throw new FormatException("Unterminated JSON array");
                if (json[index] == ']') { index++; break; }

                object val = ParseValue(json, ref index);
                list.Add(val);

                SkipWhitespace(json, ref index);
                if (index < json.Length && json[index] == ',') { index++; continue; }
                if (index < json.Length && json[index] == ']') { index++; break; }
            }

            return list;
        }

        private static string ParseString(string json, ref int index)
        {
            index++; // skip opening quote
            var sb = new StringBuilder();
            while (index < json.Length)
            {
                char c = json[index++];
                if (c == '"') return sb.ToString();
                if (c == '\\')
                {
                    if (index >= json.Length) throw new FormatException("Invalid escape sequence in JSON string");
                    char esc = json[index++];
                    switch (esc)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'u':
                            if (index + 4 <= json.Length)
                            {
                                string hex = json.Substring(index, 4);
                                sb.Append((char)Convert.ToInt32(hex, 16));
                                index += 4;
                            }
                            break;
                        default: sb.Append(esc); break;
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            throw new FormatException("Unterminated string in JSON");
        }

        private static bool ParseBool(string json, ref int index)
        {
            if (json.Length >= index + 4 && json.Substring(index, 4).Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                index += 4;
                return true;
            }
            if (json.Length >= index + 5 && json.Substring(index, 5).Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                index += 5;
                return false;
            }
            throw new FormatException($"Invalid boolean at position {index}");
        }

        private static object ParseNull(string json, ref int index)
        {
            if (json.Length >= index + 4 && json.Substring(index, 4).Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                index += 4;
                return null;
            }
            throw new FormatException($"Invalid null token at position {index}");
        }

        private static object ParseNumber(string json, ref int index)
        {
            int start = index;
            bool isFloat = false;
            if (json[index] == '-') index++;
            while (index < json.Length && (char.IsDigit(json[index]) || json[index] == '.' || json[index] == 'e' || json[index] == 'E' || json[index] == '+' || json[index] == '-'))
            {
                if (json[index] == '.' || json[index] == 'e' || json[index] == 'E') isFloat = true;
                index++;
            }

            string numStr = json.Substring(start, index - start);
            if (isFloat)
            {
                if (double.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double d)) return d;
            }
            else
            {
                if (long.TryParse(numStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out long l))
                {
                    if (l >= int.MinValue && l <= int.MaxValue) return (int)l;
                    return l;
                }
            }
            return numStr;
        }

        #endregion

        #region Conversion to Strongly Typed Object

        private static object ConvertValue(object value, Type targetType)
        {
            if (value == null) return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            if (targetType.IsAssignableFrom(value.GetType())) return value;

            if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, value.ToString(), true);
            }

            if (targetType == typeof(bool)) return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
            if (targetType == typeof(byte)) return Convert.ToByte(value, CultureInfo.InvariantCulture);
            if (targetType == typeof(sbyte)) return Convert.ToSByte(value, CultureInfo.InvariantCulture);
            if (targetType == typeof(short)) return Convert.ToInt16(value, CultureInfo.InvariantCulture);
            if (targetType == typeof(ushort)) return Convert.ToUInt16(value, CultureInfo.InvariantCulture);
            if (targetType == typeof(int)) return Convert.ToInt32(value, CultureInfo.InvariantCulture);
            if (targetType == typeof(uint)) return Convert.ToUInt32(value, CultureInfo.InvariantCulture);
            if (targetType == typeof(long)) return Convert.ToInt64(value, CultureInfo.InvariantCulture);
            if (targetType == typeof(ulong)) return Convert.ToUInt64(value, CultureInfo.InvariantCulture);
            if (targetType == typeof(float)) return Convert.ToSingle(value, CultureInfo.InvariantCulture);
            if (targetType == typeof(double)) return Convert.ToDouble(value, CultureInfo.InvariantCulture);
            if (targetType == typeof(decimal)) return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
            if (targetType == typeof(string)) return value.ToString();

            // List<T>
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type elemType = targetType.GetGenericArguments()[0];
                var listInstance = (IList)Activator.CreateInstance(targetType);
                if (value is IEnumerable enumVal)
                {
                    foreach (var item in enumVal)
                    {
                        listInstance.Add(ConvertValue(item, elemType));
                    }
                }
                return listInstance;
            }

            // Dictionary<string, T>
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type keyType = targetType.GetGenericArguments()[0];
                Type valType = targetType.GetGenericArguments()[1];
                var dictInstance = (IDictionary)Activator.CreateInstance(targetType);
                if (value is IDictionary dictVal)
                {
                    foreach (DictionaryEntry de in dictVal)
                    {
                        dictInstance.Add(ConvertValue(de.Key, keyType), ConvertValue(de.Value, valType));
                    }
                }
                return dictInstance;
            }

            // POCO Object from Dictionary
            if (value is Dictionary<string, object> dict)
            {
                object instance = Activator.CreateInstance(targetType);
                PropertyInfo[] props = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.Instance);

                foreach (var p in props)
                {
                    if (!p.CanWrite) continue;
                    if (dict.TryGetValue(p.Name, out object rawPropVal))
                    {
                        p.SetValue(instance, ConvertValue(rawPropVal, p.PropertyType), null);
                    }
                }

                foreach (var f in fields)
                {
                    if (dict.TryGetValue(f.Name, out object rawFieldVal))
                    {
                        f.SetValue(instance, ConvertValue(rawFieldVal, f.FieldType));
                    }
                }

                return instance;
            }

            return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }

        #endregion
    }
}

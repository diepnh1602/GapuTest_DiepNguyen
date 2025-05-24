using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

public static class JsonHelper
{
    #region Public Methods

    public static string Serialize(object obj)
    {
        return SerializeValue(obj);
    }

    public static T Deserialize<T>(string json)
    {
        int index = 0;
        object parsed = JsonParser.ParseValue(json, ref index);
        return (T)ConvertValue(parsed, typeof(T));
    }

    #endregion

    #region Serialization Internals

    private static string SerializeValue(object value)
    {
        if (value == null) return "null";

        if (value is string s) return QuoteString(s);
        if (value is bool b) return b ? "true" : "false";
        if (value.GetType().IsEnum) return QuoteString(value.ToString());
        if (value is DateTime dt) return QuoteString(dt.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture));
        if (IsNumeric(value)) return Convert.ToString(value, CultureInfo.InvariantCulture);
        if (value is IDictionary dict) return SerializeDictionary(dict);
        if (value is Array arr && arr.Rank == 2) return Serialize2DArray(arr);
        if (value is IEnumerable list) return SerializeArray(list);

        return SerializeObject(value);
    }

    private static string Serialize2DArray(Array array)
    {
        var sb = new StringBuilder();
        sb.Append('[');
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        for (int i = 0; i < rows; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append('[');
            for (int j = 0; j < cols; j++)
            {
                if (j > 0) sb.Append(',');
                sb.Append(SerializeValue(array.GetValue(i, j)));
            }
            sb.Append(']');
        }
        sb.Append(']');
        return sb.ToString();
    }

    private static string SerializeObject(object obj)
    {
        var sb = new StringBuilder();
        sb.Append('{');
        bool first = true;
        foreach (var member in obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance))
        {
            object val = null;
            string name = null;
            if (member is PropertyInfo pi && pi.CanRead)
            {
                name = pi.Name;
                val = pi.GetValue(obj);
            }
            else if (member is FieldInfo fi)
            {
                name = fi.Name;
                val = fi.GetValue(obj);
            }
            if (name != null)
            {
                if (!first) sb.Append(',');
                sb.Append(QuoteString(name)).Append(':').Append(SerializeValue(val));
                first = false;
            }
        }
        sb.Append('}');
        return sb.ToString();
    }

    private static string SerializeDictionary(IDictionary dict)
    {
        var sb = new StringBuilder();
        sb.Append('{');
        bool first = true;
        foreach (DictionaryEntry entry in dict)
        {
            if (!first) sb.Append(',');
            sb.Append(QuoteString(entry.Key.ToString())).Append(':').Append(SerializeValue(entry.Value));
            first = false;
        }
        sb.Append('}');
        return sb.ToString();
    }

    private static string SerializeArray(IEnumerable list)
    {
        var sb = new StringBuilder();
        sb.Append('[');
        bool first = true;
        foreach (var item in list)
        {
            if (!first) sb.Append(',');
            sb.Append(SerializeValue(item));
            first = false;
        }
        sb.Append(']');
        return sb.ToString();
    }

    private static string QuoteString(string s)
    {
        var sb = new StringBuilder();
        sb.Append('"');
        foreach (char c in s)
        {
            switch (c)
            {
                case '\\': sb.Append("\\\\"); break;
                case '"': sb.Append("\\\""); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                default:
                    if (char.IsControl(c))
                        sb.AppendFormat("\\u{0:X4}", (int)c);
                    else
                        sb.Append(c);
                    break;
            }
        }
        sb.Append('"');
        return sb.ToString();
    }

    private static bool IsNumeric(object value)
    {
        return value is byte || value is sbyte || value is short || value is ushort ||
               value is int || value is uint || value is long || value is ulong ||
               value is float || value is double || value is decimal;
    }

    #endregion

    #region Deserialization Internals

    private static object ConvertValue(object parsed, Type targetType)
    {
        if (parsed == null) return null;

        var parsedType = parsed.GetType();

        if (targetType.IsAssignableFrom(parsedType)) return parsed;

        var nullableType = Nullable.GetUnderlyingType(targetType);
        if (nullableType != null)
            return ConvertValue(parsed, nullableType);

        if (IsDictionaryType(targetType, out Type keyType, out Type valueType))
        {
            var dict = (IDictionary)Activator.CreateInstance(targetType);
            if (parsed is Dictionary<string, object> parsedDict)
            {
                foreach (var kv in parsedDict)
                {
                    object convKey = keyType.IsEnum ? Enum.Parse(keyType, kv.Key, true) : Convert.ChangeType(kv.Key, keyType, CultureInfo.InvariantCulture);
                    var convVal = ConvertValue(kv.Value, valueType);
                    dict.Add(convKey, convVal);
                }
            }
            return dict;
        }

        if (targetType.IsEnum)
        {
            if (parsed is string enumStr)
                return Enum.Parse(targetType, enumStr, true);
            if (IsNumeric(parsed))
                return Enum.ToObject(targetType, parsed);
            throw new InvalidCastException($"Cannot convert type {parsedType} to enum {targetType}");
        }

        if (targetType == typeof(Guid)) return Guid.Parse(parsed.ToString());
        if (targetType == typeof(DateTime)) return DateTime.Parse(parsed.ToString(), null, DateTimeStyles.RoundtripKind);
        if (targetType == typeof(string)) return parsed.ToString();

        if (IsPrimitiveType(targetType))
        {
            if (parsed is IConvertible)
                return Convert.ChangeType(parsed, targetType, CultureInfo.InvariantCulture);
            throw new InvalidCastException($"Cannot convert {parsedType} to {targetType}");
        }

        if (IsEnumerableType(targetType, out Type itemType))
        {
            if (targetType.IsArray && targetType.GetArrayRank() == 2 && parsed is List<object> outerList)
            {
                var elementType = targetType.GetElementType();
                int rows = outerList.Count;
                int cols = (outerList[0] as List<object>)?.Count ?? 0;

                var array = Array.CreateInstance(elementType, rows, cols);
                for (int i = 0; i < rows; i++)
                {
                    var inner = outerList[i] as List<object>;
                    for (int j = 0; j < cols; j++)
                    {
                        var val = ConvertValue(inner[j], elementType);
                        array.SetValue(val, i, j);
                    }
                }
                return array;
            }

            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
            if (parsed is List<object> parsedList)
            {
                foreach (var item in parsedList)
                    list.Add(ConvertValue(item, itemType));
            }
            if (targetType.IsArray)
            {
                var array = Array.CreateInstance(itemType, list.Count);
                list.CopyTo(array, 0);
                return array;
            }
            return list;
        }

        var result = Activator.CreateInstance(targetType);
        if (parsed is Dictionary<string, object> objDict)
        {
            foreach (var member in targetType.GetMembers(BindingFlags.Public | BindingFlags.Instance))
            {
                string name = null;
                Type memType = null;
                Action<object> setter = null;

                if (member is PropertyInfo pi && pi.CanWrite)
                {
                    name = pi.Name;
                    memType = pi.PropertyType;
                    setter = v => pi.SetValue(result, v);
                }
                else if (member is FieldInfo fi)
                {
                    name = fi.Name;
                    memType = fi.FieldType;
                    setter = v => fi.SetValue(result, v);
                }

                if (name != null && objDict.TryGetValue(name, out var rawVal))
                {
                    var conv = ConvertValue(rawVal, memType);
                    setter(conv);
                }
            }
        }
        return result;
    }

    private static bool IsPrimitiveType(Type t)
    {
        return t.IsPrimitive || t == typeof(decimal);
    }

    private static bool IsDictionaryType(Type t, out Type keyType, out Type valueType)
    {
        keyType = null;
        valueType = null;
        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            var args = t.GetGenericArguments();
            keyType = args[0];
            valueType = args[1];
            return true;
        }
        return false;
    }

    private static bool IsEnumerableType(Type t, out Type itemType)
    {
        itemType = null;
        if (t.IsArray)
        {
            itemType = t.GetElementType();
            return true;
        }
        if (t.IsGenericType && typeof(IEnumerable).IsAssignableFrom(t))
        {
            itemType = t.GetGenericArguments()[0];
            return true;
        }
        return false;
    }

    #endregion
}

public static class JsonParser
{
    public static object ParseValue(string json, ref int index)
    {
        SkipWhitespace(json, ref index);
        if (index >= json.Length) throw new Exception("Unexpected end of JSON string.");

        char c = json[index];
        if (c == '{') return ParseObject(json, ref index);
        if (c == '[') return ParseArray(json, ref index);
        if (c == '"') return ParseString(json, ref index);
        if (char.IsDigit(c) || c == '-') return ParseNumber(json, ref index);
        if (json.Substring(index).StartsWith("true")) { index += 4; return true; }
        if (json.Substring(index).StartsWith("false")) { index += 5; return false; }
        if (json.Substring(index).StartsWith("null")) { index += 4; return null; }

        throw new Exception($"Unexpected character at position {index}: {c}");
    }

    private static object ParseObject(string json, ref int index)
    {
        var dict = new Dictionary<string, object>();
        index++; // skip '{'
        while (true)
        {
            SkipWhitespace(json, ref index);
            if (json[index] == '}') { index++; break; }
            var key = ParseString(json, ref index);
            SkipWhitespace(json, ref index);
            index++; // skip ':'
            var value = ParseValue(json, ref index);
            dict[key] = value;
            SkipWhitespace(json, ref index);
            if (json[index] == ',') index++;
            else if (json[index] == '}') { index++; break; }
        }
        return dict;
    }

    private static object ParseArray(string json, ref int index)
    {
        var list = new List<object>();
        index++; // skip '['
        while (true)
        {
            SkipWhitespace(json, ref index);
            if (json[index] == ']') { index++; break; }
            list.Add(ParseValue(json, ref index));
            SkipWhitespace(json, ref index);
            if (json[index] == ',') index++;
            else if (json[index] == ']') { index++; break; }
        }
        return list;
    }

    private static string ParseString(string json, ref int index)
    {
        var sb = new StringBuilder();
        index++; // skip opening '"'
        while (true)
        {
            if (index >= json.Length) throw new Exception("Unexpected end of string.");
            char c = json[index++];
            if (c == '"') break;
            if (c == '\\')
            {
                c = json[index++];
                if (c == '"') sb.Append('"');
                else if (c == '\\') sb.Append('\\');
                else if (c == '/') sb.Append('/');
                else if (c == 'b') sb.Append('\b');
                else if (c == 'f') sb.Append('\f');
                else if (c == 'n') sb.Append('\n');
                else if (c == 'r') sb.Append('\r');
                else if (c == 't') sb.Append('\t');
                else if (c == 'u')
                {
                    var hex = json.Substring(index, 4);
                    sb.Append((char)Convert.ToInt32(hex, 16));
                    index += 4;
                }
            }
            else sb.Append(c);
        }
        return sb.ToString();
    }

    private static object ParseNumber(string json, ref int index)
    {
        int start = index;
        while (index < json.Length && (char.IsDigit(json[index]) || "-+eE.".Contains(json[index])))
            index++;
        string numStr = json.Substring(start, index - start);
        if (numStr.Contains(".") || numStr.Contains("e") || numStr.Contains("E"))
            return double.Parse(numStr, CultureInfo.InvariantCulture);
        if (long.TryParse(numStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longVal))
            return longVal;
        return double.Parse(numStr, CultureInfo.InvariantCulture);
    }

    private static void SkipWhitespace(string json, ref int index)
    {
        while (index < json.Length && char.IsWhiteSpace(json[index])) index++;
    }
}
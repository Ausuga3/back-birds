using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BackBird.Api.src.Bird.Modules.Birds.Infrastructure.Json
{
    public class JsonStringEnumMemberConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter)Activator.CreateInstance(
                typeof(JsonStringEnumMemberConverterInner<>).MakeGenericType(typeToConvert))!;
        }

        private class JsonStringEnumMemberConverterInner<T> : JsonConverter<T> where T : struct, Enum
        {
            private readonly Dictionary<T, string> _enumToString = new();
            private readonly Dictionary<string, T> _stringToEnum = new();

            public JsonStringEnumMemberConverterInner()
            {
                var type = typeof(T);
                var values = Enum.GetValues<T>();

                foreach (var value in values)
                {
                    var enumMember = type.GetMember(value.ToString())[0];
                    var attr = enumMember.GetCustomAttribute<EnumMemberAttribute>();
                    var stringValue = attr?.Value ?? value.ToString();
                    
                    _enumToString.Add(value, stringValue);
                    _stringToEnum.Add(stringValue, value);
                }
            }

            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var stringValue = reader.GetString();
                if (stringValue != null && _stringToEnum.TryGetValue(stringValue, out var enumValue))
                {
                    return enumValue;
                }

                throw new JsonException($"Unable to convert \"{stringValue}\" to enum \"{typeof(T)}\"");
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(_enumToString[value]);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using static Core.component.QuaternionConverter;
using Core.graphics.light;
using Core.models;

namespace Core.component
{
    public class QuaternionConverter : JsonConverter<Quaternion>
    {
        public class Conv
        {
            public static float ReadAsFloat(ref Utf8JsonReader reader)
            {
                if (reader.TokenType == JsonTokenType.Number)
                    return reader.GetSingle();
                if (reader.TokenType == JsonTokenType.String && float.TryParse(reader.GetString(), out var result))
                    return result;

                throw new JsonException("Expected float or numeric string.");
            }
        }

        public override Quaternion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            float x = 0, y = 0, z = 0, w = 1;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string? prop = reader.GetString();
                    reader.Read();

                    switch (prop)
                    {
                        case "X": x = Conv.ReadAsFloat(ref reader); break;
                        case "Y": y = Conv.ReadAsFloat(ref reader); break;
                        case "Z": z = Conv.ReadAsFloat(ref reader); break;
                        case "W": w = Conv.ReadAsFloat(ref reader); break;
                    }
                }
            }

            return new Quaternion(x, y, z, w);
        }

        public override void Write(Utf8JsonWriter writer, Quaternion value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("X", value.X);
            writer.WriteNumber("Y", value.Y);
            writer.WriteNumber("Z", value.Z);
            writer.WriteNumber("W", value.W);
            writer.WriteEndObject();
        }
    }
    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            float x = 0, y = 0, z = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string? prop = reader.GetString();
                    reader.Read();

                    switch (prop)
                    {
                        case "X": x = Conv.ReadAsFloat(ref reader); break;
                        case "Y": y = Conv.ReadAsFloat(ref reader); break;
                        case "Z": z = Conv.ReadAsFloat(ref reader); break;
                    }
                }
            }

            return new Vector3(x, y, z);
        }

        public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("X", value.X);
            writer.WriteNumber("Y", value.Y);
            writer.WriteNumber("Z", value.Z);
            writer.WriteEndObject();
        }
    }


    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            float x = 0, y = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string? prop = reader.GetString();
                    reader.Read();

                    switch (prop)
                    {
                        case "X": x = Conv.ReadAsFloat(ref reader); break;
                        case "Y": y = Conv.ReadAsFloat(ref reader); break;

                    }
                }
            }

            return new Vector2(x, y);
        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("X", value.X);
            writer.WriteNumber("Y", value.Y);
            writer.WriteEndObject();
        }
    }
    public class BoolConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
                return reader.GetBoolean();

            if (reader.TokenType == JsonTokenType.String && bool.TryParse(reader.GetString(), out var result))
                return result;

            throw new JsonException("Expected boolean or string representing a boolean.");
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }

    public class FloatConverter : JsonConverter<float>
    {
        public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Number => reader.GetSingle(),
                JsonTokenType.String when float.TryParse(reader.GetString(), out var value) => value,
                _ => throw new JsonException("Expected float or numeric string.")
            };
        }

        public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
    public class UInt32ListConverter : JsonConverter<List<uint>>
    {
        public override List<uint> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var list = new List<uint>();

            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Expected start of array");

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                if (reader.TokenType == JsonTokenType.Number)
                {
                    list.Add(reader.GetUInt32());
                }
                else if (reader.TokenType == JsonTokenType.String)
                {
                    var str = reader.GetString();
                    if (uint.TryParse(str, out var result))
                        list.Add(result);
                    else
                        throw new JsonException($"Cannot parse '{str}' as uint.");
                }
                else
                {
                    throw new JsonException("Unexpected token type for uint value.");
                }
            }

            return list;
        }

        public override void Write(Utf8JsonWriter writer, List<uint> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var v in value)
                writer.WriteNumberValue(v);
            writer.WriteEndArray();
        }
    }

    public class LightTypeConverter : JsonConverter<LightType>
    {
        public override LightType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return (LightType)reader.GetInt32();
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();

                if (int.TryParse(str, out var intVal))
                    return (LightType)intVal;

                if (Enum.TryParse<LightType>(str, true, out var enumVal))
                    return enumVal;
            }

            throw new JsonException($"Cannot convert to LightType");
        }

        public override void Write(Utf8JsonWriter writer, LightType value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue((int)value);
        }
    }
    public class AssetTypeConverter : JsonConverter<AssetType>
    {
        public override AssetType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return (AssetType)reader.GetInt32();
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();

                // Dacă e număr în string
                if (int.TryParse(str, out var intVal))
                    return (AssetType)intVal;

                // Dacă e nume valid de enum
                if (Enum.TryParse<AssetType>(str, ignoreCase: true, out var result))
                    return result;
            }

            throw new JsonException("Invalid value for AssetType.");
        }

        public override void Write(Utf8JsonWriter writer, AssetType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    public class LightBaseConverter : JsonConverter<LightBase>
    {
        public override LightBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (!root.TryGetProperty("$type", out var typeProp))
                throw new JsonException("Missing $type discriminator");

            var typeName = typeProp.GetString();
            JsonSerializerOptions opt = new(options);
            opt.Converters.Add(new FlexibleConverterFactory());

            Console.WriteLine("LightBaseConverter");
            return typeName switch
            {
                "LightPoint" => JsonSerializer.Deserialize<LightPoint>(root.GetRawText(), opt)!,
                "LightDirectional" => JsonSerializer.Deserialize<LightDirectional>(root.GetRawText(), opt)!,
                "LightSpot" => JsonSerializer.Deserialize<LightSpot>(root.GetRawText(), opt)!,
                _ => throw new JsonException($"Unknown Light type '{typeName}'")
            };
        }

        public override void Write(Utf8JsonWriter writer, LightBase value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
    }

    public class FlexibleConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(float) ||
                   typeToConvert == typeof(bool) ||
                   typeToConvert == typeof(int) ||
                   typeToConvert == typeof(uint);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(FlexibleConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
    }

    public class FlexibleConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number || reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
                return JsonSerializer.Deserialize<T>(ref reader, options);

            if (reader.TokenType == JsonTokenType.String && typeof(T).IsPrimitive)
            {
                var str = reader.GetString();
                return (T)Convert.ChangeType(str, typeof(T));
            }

            throw new JsonException($"Cannot convert token to {typeof(T)}");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }



}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.scene
{
    public class ComponentConverter : JsonConverter<IComponent>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(IComponent).IsAssignableFrom(typeToConvert);
        }

        public override IComponent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (!root.TryGetProperty("Type", out var typeProp))
                throw new Exception("Missing Type property");

            var typeName = typeProp.GetString()!;
            var type = Type.GetType(typeName)!;
            return (IComponent)root.Deserialize(type, options)!;
        }

        public override void Write(Utf8JsonWriter writer, IComponent value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Type", value.GetType().AssemblyQualifiedName);
            foreach (var prop in value.GetType().GetProperties())
            {
                var val = prop.GetValue(value);
                writer.WritePropertyName(prop.Name);
                JsonSerializer.Serialize(writer, val, options);
            }
            writer.WriteEndObject();
        }
    }
}

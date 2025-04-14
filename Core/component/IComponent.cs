using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.component
{
    public interface IComponent
    {
    }


    public class ComponentDictionaryConverter : JsonConverter<Dictionary<string, IComponent>>
    {
        public override Dictionary<string, IComponent> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(ref reader, options)!;
            var result = new Dictionary<string, IComponent>();

            foreach (var kvp in dict)
            {
                IComponent? comp = kvp.Key switch
                {
                    nameof(TransformComponent) => kvp.Value.Deserialize<TransformComponent>(options),
                    nameof(CameraComponent) => kvp.Value.Deserialize<CameraComponent>(options),
                    nameof(MeshComponent) => kvp.Value.Deserialize<MeshComponent>(options),
                    nameof(ShaderComponent) => kvp.Value.Deserialize<ShaderComponent>(options),
                    nameof(CameraControllerComponent) => kvp.Value.Deserialize<CameraControllerComponent>(options),
                    _ => null
                };

                if (comp != null)
                    result[kvp.Key] = comp;
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, IComponent> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var kvp in value)
            {
                writer.WritePropertyName(kvp.Key);
                JsonSerializer.Serialize(writer, kvp.Value, kvp.Value.GetType(), options);
            }
            writer.WriteEndObject();
        }
    }
}

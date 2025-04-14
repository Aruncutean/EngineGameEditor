using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Core.component;

namespace Core.entity
{
    public class Entity
    {
        public string Name { get; set; } = "Unnamed";

        [JsonConverter(typeof(ComponentDictionaryConverter))]
        public Dictionary<string, IComponent> Components { get; set; } = new();

        public void AddComponent<T>(T component) where T : IComponent
            => Components[typeof(T).Name] = component;

        public T? GetComponent<T>() where T : class, IComponent
        {
            Components.TryGetValue(typeof(T).Name, out var comp);
            return comp as T;
        }

        public bool HasComponent<T>() where T : IComponent
            => Components.ContainsKey(typeof(T).Name);
    }
}

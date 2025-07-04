﻿using Core.component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.models
{
    public class AssetItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

    
        [JsonConverter(typeof(AssetTypeConverter))]
        [JsonPropertyName("type")]
        public AssetType Type { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;

        [JsonPropertyName("baseDirector")]
        public string BaseDirector { get; set; } = string.Empty;

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public override string ToString()
        {
            return Name;
        }

    }

    public enum AssetType
    {
        Mesh, Texture, Folder, Material, Script

    }

}

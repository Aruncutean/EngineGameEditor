#pragma once

#include <string>
#include <unordered_map>
#include <memory>
#include <glad/glad.h>
#include "Texture.hpp"


namespace Material {

	class TextureManager {
	public:
		static GLuint Get(const std::string& id) {
			if (id.empty()) {
				return 0;
			}

			auto& instance = Instance();
			auto it = instance._textures.find(id);
			if (it != instance._textures.end()) {
				return it->second;
			}
			std::optional<Model::ProjectData> projectData = AppContext::Instance().GetCurrentProject();
			if (!projectData) {
				return 0;
			}

			auto assetCollection = Service::AssetsService::Instance().getAssets();

			auto itAsset = std::find_if(assetCollection.begin(), assetCollection.end(),
				[&id](const Model::AssetItem& asset) {
					return asset.id == id;
				});

			if (itAsset != assetCollection.end()) {
				std::filesystem::path fullPath = std::filesystem::path(projectData->path) / itAsset->path;
				Texture texture;
				GLuint textureID = texture.LoadTexture(fullPath.string());
				instance._textures[id] = textureID;
				return textureID;
			}

			return 0;
		}

	private:
		TextureManager() = default;

		static TextureManager& Instance() {
			static TextureManager instance;
			return instance;
		}

		std::unordered_map<std::string, GLuint> _textures;
	};

}

#pragma once  
#include "ProjectData.h"  
#include "SceneData.h"  
#include <optional>  
#include <memory>  
#include <string>  
#include <iostream>  
#include "World.h"  
#include <glm/glm.hpp>  
#include "Entity.hpp"
#include "AssetItem.h"

namespace Service {
	class EditorService {
	public:
		static EditorService& Instance() {
			static EditorService instance;
			return instance;
		}

		glm::mat4 getView() {
			return view;
		}
		glm::mat4 getProjection() {
			return projection;
		}

		void setView(glm::mat4 v) {
			view = v;
		}
		void setProjection(glm::mat4 p) {
			projection = p;
		}

		Entity::Entity* GetSelectedEntity() {
			return entity;
		}

		void setSelectedEntity(Entity::Entity* e) {
			entity = e;
		}

		Entity::Entity* getEditorCamera() {
			return editorCamera;
		}

		void setEditorCamera(Entity::Entity* e) {
			editorCamera = e;
		}

		Model::AssetItem* getMaterialSelected() {
			return material;
		}

		void setMaterialSelected(Model::AssetItem* m) {
			material = new Model::AssetItem(*m);;
		}


	private:
		EditorService() : view(glm::mat4(1.0f)), projection(glm::mat4(1.0f)) {}
		~EditorService() = default;

		EditorService(const EditorService&) = delete;
		EditorService& operator=(const EditorService&) = delete;

		glm::mat4 view;
		glm::mat4 projection;
		Entity::Entity* entity = nullptr;
		Entity::Entity* editorCamera = nullptr;
		Model::AssetItem* material = nullptr;
	};
}

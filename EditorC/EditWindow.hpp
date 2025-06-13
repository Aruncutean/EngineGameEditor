#pragma once
#include <imgui.h>
#include <string>
#include <vector>
#include <functional>
#include <memory>
#include "Component.h"
#include "Window.h"
#include "MainWindow.hpp"
#include "EditorService.hpp"

class EditWindow : public Window, public std::enable_shared_from_this<EditWindow>
{
public:
	EditWindow(MainWindow* host) : host(host) {
		host->IsBockingSpace = true;
	}

	~EditWindow() {

	}

	void Render() override {
		ImGui::Begin("Selected Object Info", nullptr, ImGuiWindowFlags_AlwaysAutoResize);

		if (auto selectedEntity = Service::EditorService::Instance().GetSelectedEntity()) {
			if (selectedEntity->HasComponent<Component::TransformComponent>()) {
				auto transform = selectedEntity->GetComponent<Component::TransformComponent>();

				ImGui::Text("Object: %s", selectedEntity->Name.c_str());

				ImGui::Separator();
				ImGui::Text("Transform");

				ImGui::DragFloat3("Position", glm::value_ptr(transform->position), 0.1f);
				ImGui::DragFloat3("Rotation", glm::value_ptr(transform->rotation), 0.1f);
				ImGui::DragFloat3("Scale", glm::value_ptr(transform->scale), 0.1f, 0.01f, 100.0f);

				if (selectedEntity->HasComponent<Component::MaterialComponent>()) {
					auto materialComp = selectedEntity->GetComponent<Component::MaterialComponent>();

					ImGui::Separator();
					ImGui::Text("Material");

					std::vector<Model::AssetItem> allAssets = Service::AssetsService::Instance().getAssets();
					std::vector<std::string> materialPaths;
					std::vector<Model::AssetItem> materialAssets;

					materialPaths.push_back("Default");
					Model::AssetItem defaultAsset;
					defaultAsset.id = "default";
					defaultAsset.name = "Default";
					materialAssets.push_back(defaultAsset);


					for (const auto& asset : allAssets) {
						if (asset.type == Model::AssetType::Material) {
							materialPaths.push_back(asset.name);
							materialAssets.push_back(asset);
						}
					}

					int currentIndex = 0;
					for (int i = 0; i < materialAssets.size(); ++i) {
						if (materialAssets[i].id == materialComp->MaterialID) {
							currentIndex = i;
							break;
						}
					}


					if (ImGui::Combo("Material", &currentIndex,
						[](void* data, int idx, const char** out_text) {
							auto& paths = *static_cast<std::vector<std::string>*>(data);
							*out_text = paths[idx].c_str();
							return true;
						}, static_cast<void*>(&materialPaths), static_cast<int>(materialPaths.size()))) {

						materialComp->MaterialID = materialAssets[currentIndex].id;
					}
				}
			}

			if (selectedEntity->HasComponent<Component::LightComponent>()) {
				auto lightComp = selectedEntity->GetComponent<Component::LightComponent>();

				ImGui::Separator();
				ImGui::Text("Light");

				const char* lightTypes[] = { "Point", "Directional", "Spot" };
				int currentType = static_cast<int>(lightComp->Type);

				if (ImGui::Combo("Light Type", &currentType, lightTypes, IM_ARRAYSIZE(lightTypes))) {
					lightComp->Type = static_cast<Light::LightType>(currentType);
					switch (lightComp->Type) {
					case Light::LightType::Point:
						lightComp->LightBase = std::make_shared<Light::LightPoint>();
						break;
					case Light::LightType::Directional:
						lightComp->LightBase = std::make_shared<Light::LightDirectional>();
						break;
					case Light::LightType::Spot:
						lightComp->LightBase = std::make_shared<Light::LightSpot>();
						break;
					}
				}


				if (lightComp->LightBase) {
					ImGui::SliderFloat("Intensity", &reinterpret_cast<Light::LightPoint*>(lightComp->LightBase.get())->intensity, 0.0f, 10.0f);

					glm::vec3& color = reinterpret_cast<Light::LightPoint*>(lightComp->LightBase.get())->color;
					ImGui::ColorEdit3("Color", glm::value_ptr(color));


					if (auto point = std::dynamic_pointer_cast<Light::LightPoint>(lightComp->LightBase)) {
						ImGui::SliderFloat("Range", &point->range, 0.0f, 100.0f);
					}


					if (auto dir = std::dynamic_pointer_cast<Light::LightDirectional>(lightComp->LightBase)) {
						ImGui::DragFloat3("Direction", glm::value_ptr(dir->direction), 0.1f);
					}


					if (auto spot = std::dynamic_pointer_cast<Light::LightSpot>(lightComp->LightBase)) {
						ImGui::DragFloat3("Direction", glm::value_ptr(spot->direction), 0.1f);
						ImGui::SliderFloat("Range", &spot->range, 0.0f, 100.0f);
						ImGui::SliderFloat("Cutoff", &spot->cutoff, 0.0f, 90.0f);
					}
				}
			}

		}
		ImGui::End();

		ImGui::Begin("Scene Hierarchy");

		auto entities = AppContext::Instance().GetCurrentWorldPath().GetEntities();

		for (size_t i = 0; i < entities.size(); ++i) {
			auto entity = entities[i];
			std::string label = "Entity " + std::to_string(i);

			bool isSelected = (Service::EditorService::Instance().GetSelectedEntity() == entity.get());

			if (ImGui::Selectable(label.c_str(), isSelected)) {
				Service::EditorService::Instance().setSelectedEntity(entity.get());

			}
		}
		ImGui::End();
		ImGui::Begin("Camera Info");
		if (auto camera = Service::EditorService::Instance().getEditorCamera()) {
			if (camera->HasComponent<Component::TransformComponent>() &&
				camera->HasComponent<Component::CameraComponent>()) {

				auto transform = camera->GetComponent<Component::TransformComponent>();
				auto cameraC = camera->GetComponent<Component::CameraComponent>();

				// Transform (editable)
				ImGui::Text("Transform");
				ImGui::DragFloat3("Position", glm::value_ptr(transform->position), 0.1f);
				ImGui::DragFloat3("Rotation", glm::value_ptr(transform->rotation), 0.1f);
				ImGui::DragFloat3("Scale", glm::value_ptr(transform->scale), 0.1f);

				ImGui::Separator();

				// Camera (editable)
				ImGui::Text("Camera");
				ImGui::DragFloat("FOV", &cameraC->fieldOfView, 0.1f, 1.0f, 180.0f);
				ImGui::DragFloat("Near Clip", &cameraC->nearClip, 0.01f, 0.001f, 10.0f);
				ImGui::DragFloat("Far Clip", &cameraC->farClip, 1.0f, 10.0f, 10000.0f);

				ImGui::Separator();

				// Vectors (display only)
				glm::vec3 front = cameraC->front;
				glm::vec3 up = cameraC->up;

				ImGui::Text("Direction Vectors");
				ImGui::Text("Front: %.2f %.2f %.2f", front.x, front.y, front.z);
				ImGui::Text("Up: %.2f %.2f %.2f", up.x, up.y, up.z);

			}


		}

		ImGui::End();
	}




private:
	MainWindow* host = nullptr;
};

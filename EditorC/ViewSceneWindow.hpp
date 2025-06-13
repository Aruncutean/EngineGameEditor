#pragma once
#include <imgui.h>
#include <string>
#include <vector>
#include <functional>
#include <memory>
#include "Component.h"
#include "Window.h"
#include "MainWindow.hpp"
#include "WorldManager.hpp"
#include "WindowService.hpp"
#include "AppContext.h"
#include <ImGuizmo.h>
#include "EditorService.hpp"
class ViewSceneWindow : public Window, public std::enable_shared_from_this<ViewSceneWindow>
{
public:
	ViewSceneWindow(MainWindow* host) : host(host) {
		host->IsBockingSpace = true;

		std::optional<Model::ProjectData> projectData = AppContext::Instance().GetCurrentProject();


		_worldSystem = new WorldManager(projectData->path);
		_worldSystem->isEditMode = true;
		_worldSystem->renderInFrameBuffer = true;
		_worldSystem->Init();
		_worldSystem->LoadWorld(AppContext::Instance().GetCurrentScene().path);

	}

	~ViewSceneWindow() {

	}

	void Render() override {
		;
		_worldSystem->Update(WindowService::getInstance()->getDeltaTime());


		bool isOpen = true;
		UI::Window("#Scene Scene", isOpen, [&]()
			{


				ImVec2 size = ImGui::GetContentRegionAvail();
				if (isGuizmoClicked == false) {
					Input::Instance().isActive = ImGui::IsWindowFocused(ImGuiFocusedFlags_RootAndChildWindows);
				}

				if ((int)size.x != framebufferWidth || (int)size.y != framebufferHeight)
				{

					WindowService::getInstance()->setHeight((int)size.y);
					WindowService::getInstance()->setWidth((int)size.x);
					framebufferHeight = (int)size.y;
					framebufferWidth = (int)size.x;
					_worldSystem->Resize((int)size.x, (int)size.y);
				}
				_worldSystem->Render(WindowService::getInstance()->getDeltaTime());


				_worldSystem->frameBuffer->BlitToTexture();


				if (_worldSystem->frameBuffer != nullptr)
				{
					ImGui::Image(_worldSystem->frameBuffer->_texture, size, ImVec2(0, 1),
						ImVec2(1, 0));
				}

				ImGuizmo::SetDrawlist();

				ImVec2 windowPos = ImGui::GetWindowPos();
				ImVec2 regionMin = ImGui::GetWindowContentRegionMin();
				ImVec2 regionMax = ImGui::GetWindowContentRegionMax();

				ImVec2 min = ImVec2(windowPos.x + regionMin.x, windowPos.y + regionMin.y);
				ImVec2 size1 = ImVec2(regionMax.x - regionMin.x, regionMax.y - regionMin.y);

				ImGuizmo::SetRect(min.x, min.y, size1.x, size1.y);

				glm::mat4 identity = glm::mat4(1.0f);

				if (ImGui::BeginPopupContextWindow("scene_context", ImGuiPopupFlags_MouseButtonRight)) {
					if (ImGui::MenuItem("Add Light")) {

						auto lightEntity = new Entity::Entity();
						lightEntity->Name = "Light";

						lightEntity->AddComponent<Component::TransformComponent>();
						lightEntity->AddComponent<Component::LightComponent>();

						auto lightComp = lightEntity->GetComponent<Component::LightComponent>();
						lightComp->Type = Light::LightType::Point;
						lightComp->LightBase = std::make_shared<Light::LightPoint>();

						auto transform = lightEntity->GetComponent<Component::TransformComponent>();
						transform->position = glm::vec3(0.0f, 5.0f, 0.0f);

						AppContext::Instance().GetCurrentWorldPath().AddEntity(std::shared_ptr<Entity::Entity>(lightEntity));
					}
					if (ImGui::MenuItem("Add Direct Light")) {

						auto lightEntity = new Entity::Entity();
						lightEntity->Name = "Light Direct";

						lightEntity->AddComponent<Component::TransformComponent>();
						lightEntity->AddComponent<Component::LightComponent>();

						auto lightComp = lightEntity->GetComponent<Component::LightComponent>();
						lightComp->Type = Light::LightType::Point;
						lightComp->LightBase = std::make_shared<Light::LightDirectional>();

						auto transform = lightEntity->GetComponent<Component::TransformComponent>();
						transform->position = glm::vec3(0.0f, 5.0f, 0.0f);

						AppContext::Instance().GetCurrentWorldPath().AddEntity(std::shared_ptr<Entity::Entity>(lightEntity));
					}
					ImGui::EndPopup();
				}



				if (auto selectedEntity = Service::EditorService::Instance().GetSelectedEntity()) {
					if (selectedEntity->HasComponent<Component::TransformComponent>()) {
						auto transform = selectedEntity->GetComponent<Component::TransformComponent>();

						glm::mat4 model = transform->GetTransform();


						if (Input::KeyT()) currentOp = ImGuizmo::TRANSLATE;
						if (Input::KeyR()) currentOp = ImGuizmo::ROTATE;
						if (Input::KeyS()) currentOp = ImGuizmo::SCALE;
						glm::mat4 view = Service::EditorService::Instance().getView();

						ImGuizmo::Manipulate(
							glm::value_ptr(view),
							glm::value_ptr(Service::EditorService::Instance().getProjection()),
							currentOp,
							ImGuizmo::LOCAL,
							glm::value_ptr(model)
						);

						if (ImGuizmo::IsUsing()) {
							Input::Instance().isActive = false;
							isGuizmoClicked = true;
							glm::vec3 translation, scale;
							glm::quat rotationQuat;
							ImGuizmo::DecomposeMatrixToComponents(glm::value_ptr(model),
								glm::value_ptr(translation),
								glm::value_ptr(rotationQuat),
								glm::value_ptr(scale));

							transform->position = translation;
							transform->rotation = glm::eulerAngles(rotationQuat);;
							transform->scale = scale;
						}
						else {
							isGuizmoClicked = false;
						}
					}
				}
			});
	}




private:
	MainWindow* host = nullptr;
	WorldManager* _worldSystem = nullptr;
	int framebufferWidth;
	int framebufferHeight;
	bool isGuizmoClicked = false;
	ImGuizmo::OPERATION currentOp = ImGuizmo::TRANSLATE;
};

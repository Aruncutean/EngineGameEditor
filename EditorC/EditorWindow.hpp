
#pragma once
#include <imgui.h>
#include <string>
#include <vector>
#include <functional>
#include <memory>
#include "Component.h"
#include "Window.h"
#include "MainWindow.hpp"
#include "AssetsWindow.hpp"
#include "ViewSceneWindow.hpp"
#include "EditWindow.hpp"
#include "MaterialEditorWindow.hpp"
#include "ComponentFactory.h"
#include "ProjectService.hpp"

class EditorWindow : public Window, public std::enable_shared_from_this<EditorWindow>
{
public:
	EditorWindow(MainWindow* host) : host(host) {
		host->IsBockingSpace = true;


		RegisterAllComponents();

		assetsWindow = new AssetsWindow(host);
		viewSceneWindow = new ViewSceneWindow(host);
		editWindow = new EditWindow(host);
		materialEditorWindow = new MaterialEditorWindow();
	}

	void RegisterAllComponents() {
		using namespace Component;
		auto& factory = ComponentFactory::Instance();
		factory.Register<TransformComponent>("TransformComponent");
		factory.Register<MaterialComponent>("MaterialComponent");
		factory.Register<MeshComponent>("MeshComponent");
		factory.Register<CameraComponent>("CameraComponent");
		factory.Register<LightComponent>("LightComponent");
	}

	~EditorWindow() {

	}

	void Render() override {
		if (ImGui::BeginMainMenuBar())
		{
			if (ImGui::BeginMenu("File"))
			{
				if (ImGui::MenuItem("New Scene", "Ctrl+N")) {
					// Acțiune pentru scenă nouă
				}
				if (ImGui::MenuItem("Open...", "Ctrl+O")) {
					// Acțiune pentru deschidere fișier
				}
				if (ImGui::MenuItem("Save", "Ctrl+S")) {
					std::string path = AppContext::Instance().GetCurrentScene().path;

					SceneSerializer::SaveScene(path, AppContext::Instance().GetCurrentWorldPath());
				}
				if (ImGui::MenuItem("Exit")) {
					// Închide aplicația
				}
				ImGui::EndMenu();
			}

			if (ImGui::BeginMenu("Edit"))
			{
				if (ImGui::MenuItem("Undo", "Ctrl+Z")) {}
				if (ImGui::MenuItem("Redo", "Ctrl+Y", false, false)) {} // false = dezactivat
				ImGui::EndMenu();
			}

			if (ImGui::BeginMenu("View"))
			{
				if (ImGui::MenuItem("Show Grid")) {}
				if (ImGui::MenuItem("Toggle Fullscreen")) {}
				ImGui::EndMenu();
			}

			if (ImGui::BeginMenu("Build"))
			{
				if (ImGui::MenuItem("Run")) {
					Service::ProjectService projectService;
					projectService.RunProject();
				}

				ImGui::EndMenu();
			}


			ImGui::EndMainMenuBar();
		}


		assetsWindow->Render();
		viewSceneWindow->Render();
		editWindow->Render();
		materialEditorWindow->Render();

	}

private:
	MainWindow* host = nullptr;

	AssetsWindow* assetsWindow = nullptr;
	ViewSceneWindow* viewSceneWindow = nullptr;
	EditWindow* editWindow = nullptr;
	MaterialEditorWindow* materialEditorWindow = nullptr;
};

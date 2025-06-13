#pragma once

#include <memory>
#include <glad/glad.h>
#include <glm/glm.hpp>
#include "FrameBuffer.hpp"
#include "Entity.hpp"
#include "World.h"
#include "SceneInfo.h"
#include "RenderSystem.hpp"
#include "CameraControllerSystem.hpp"
#include "ShaderManager.hpp"
#include "SceneSerializer.h"
#include "TransformComponent.hpp"
#include "CameraComponent.hpp"
#include "CameraControllerComponent.hpp"
#include "WindowService.hpp"


class WorldManager {
public:
	WorldManager(const std::string& path)
		: projectPath(path)
	{
		Init();
	}

	void Init() {
		ShaderManager::LoadShaders();

		cameraEntity = std::make_shared<Entity::Entity>();
		cameraEntity->AddComponent<Component::TransformComponent>();
		cameraEntity->AddComponent<Component::CameraComponent>();
		cameraEntity->AddComponent<Component::CameraControllerComponent>();

		cameraEntity->GetComponent<Component::TransformComponent>()->position = glm::vec3(0, 5, 10);
		cameraEntity->GetComponent<Component::CameraComponent>()->up = glm::vec3(0, 1, 0);
		renderSystem = std::make_unique<System::RenderSystem>();
		renderSystem->cameraEntity = cameraEntity;

		Service::EditorService::Instance().setEditorCamera(cameraEntity.get());

		cameraController = std::make_unique<System::CameraControllerSystem>(cameraEntity);

		frameBuffer = std::make_unique<FrameBuffer>(
			WindowService::getInstance()->getWidth(),
			WindowService::getInstance()->getHeight()
		);

		glEnable(GL_DEPTH_TEST);
		glEnable(GL_MULTISAMPLE);
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
	}

	void Resize(int width, int height) {
		if (frameBuffer)
			frameBuffer->Resize(width, height);
	}

	void LoadWorld(const Model::SceneInfo& info) {
		std::string path = projectPath + "/scenes/" + info.path;
		currentWorld = std::make_shared<World>(SceneSerializer::LoadScene(path));
		AppContext::Instance().SetCurrentWorld(*currentWorld);
	}

	void LoadWorld(const std::string& path) {
		currentWorld = std::make_shared<World>(SceneSerializer::LoadScene(path));
		AppContext::Instance().SetCurrentWorld(*currentWorld);
	}

	void LoadWorld(std::shared_ptr<World> world) {
		currentWorld = world;
		AppContext::Instance().SetCurrentWorld(*currentWorld);
	}

	void Update(float deltaTime) {
		if (cameraController && currentWorld)
			cameraController->Update(deltaTime);
	}

	void Render(float deltaTime) {
		auto renderScene = [&]() {
			glClearColor(0.247f, 0.247f, 0.247f, 1.0f);
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

			if (currentWorld) {

				auto currentWorldPath = AppContext::Instance().GetCurrentWorldPath();
				if (renderSystem)
					renderSystem->Render(&currentWorldPath, WindowService::getInstance()->getWidth(),
						WindowService::getInstance()->getHeight());
			}
			};

		if (renderInFrameBuffer && frameBuffer) {
			frameBuffer->Render(renderScene);
			frameBuffer->BlitToTexture();
		}
		else {
			renderScene();
		}
	}

	void Dispose() {
		frameBuffer.reset();
		renderSystem.reset();
		cameraController.reset();
	}

	bool renderInFrameBuffer = false;
	bool isEditMode = false;


	std::shared_ptr<World> currentWorld;
	std::shared_ptr<Entity::Entity> cameraEntity;
	std::unique_ptr<System::RenderSystem> renderSystem;
	std::unique_ptr<System::CameraControllerSystem> cameraController;

	std::unique_ptr<FrameBuffer> frameBuffer;
	std::string projectPath;
};

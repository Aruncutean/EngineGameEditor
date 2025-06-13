#pragma once
#include "ProjectData.h" 
#include "SceneData.h"
#include <optional>
#include <memory>
#include <string>
#include <iostream>
#include "World.h"

class AppContext {
public:
	static AppContext& Instance() {
		static AppContext instance;
		return instance;
	}


	void SetCurrentProject(const Model::ProjectData& project) {
		currentProject = project;
	}

	bool HasCurrentProject() const {
		return currentProject.has_value();
	}

	const Model::ProjectData& GetCurrentProject() const {
		if (!currentProject)
			throw std::runtime_error("No project loaded.");
		return *currentProject;
	}

	void ClearProject() {
		currentProject.reset();
	}

	const Model::SceneData& GetCurrentScene() const {
		if (!curentScene)
			throw std::runtime_error("No project loaded.");
		return *curentScene;
	}

	void SetCurrentScene(const Model::SceneData& sceneData) {
		curentScene = sceneData;
	}

	bool HasCurrentScene() const {
		return curentScene.has_value();
	}

	const Model::SceneData& GetCurrentScenePath() const {
		if (!curentScene)
			throw std::runtime_error("No scene loaded.");
		return *curentScene;
	}

	void ClearScene() {
		curentScene.reset();
	}

	void SetCurrentWorld(const World& sceneData) {
		currentWorld = sceneData;
	}

	World& GetCurrentWorldPath() {
		if (!currentWorld)
			throw std::runtime_error("No scene loaded.");
		return *currentWorld;
	}


private:
	AppContext() = default;
	~AppContext() = default;

	AppContext(const AppContext&) = delete;
	AppContext& operator=(const AppContext&) = delete;

	std::optional<Model::ProjectData> currentProject;
	std::optional<Model::SceneData> curentScene;
	std::optional<World> currentWorld;
};

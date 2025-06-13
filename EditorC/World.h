#pragma once

#include <string>
#include <vector>
#include <memory>
#include <chrono>
#include "Entity.hpp"

class World {
public:

	World()
	{
		createdAt = std::chrono::system_clock::now();
		lastUpdated = createdAt;
	}

	void AddEntity(const std::shared_ptr<Entity::Entity>& entity)
	{
		entities.push_back(entity);
		lastUpdated = std::chrono::system_clock::now();
	}

	std::vector<std::shared_ptr<Entity::Entity>> GetEntities() const
	{
		return entities;
	}

	const std::string& GetName() const
	{
		return name;
	}

	const std::string& GetPath() const
	{
		return path;
	}

	std::chrono::system_clock::time_point GetCreatedAt() const
	{
		return createdAt;
	}

	std::chrono::system_clock::time_point GetLastUpdated() const
	{
		return lastUpdated;
	}

	void SetName(const std::string& n)
	{
		name = n;
	}

	void SetPath(const std::string& p)
	{
		path = p;
	}

private:
	std::vector<std::shared_ptr<Entity::Entity>> entities;
	std::string name;
	std::string path;
	std::chrono::system_clock::time_point createdAt;
	std::chrono::system_clock::time_point lastUpdated;
};

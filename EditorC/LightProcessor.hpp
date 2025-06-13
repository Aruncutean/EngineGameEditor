#pragma once

#include <vector>
#include <memory>
#include "World.h"
#include "Entity.hpp"
#include "LightComponent.hpp"

namespace Process {

	class LightProcessor {
	public:
		std::vector<std::shared_ptr<Entity::Entity>> GetLights(const World& scene) {
			std::vector<std::shared_ptr<Entity::Entity>> lights;

			for (const auto& entity : scene.GetEntities()) {
				if (entity->HasComponent<Component::LightComponent>()) {
					lights.push_back(entity);
				}
			}

			return lights;
		}
	};

}

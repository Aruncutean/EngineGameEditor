#pragma once  
#include <vector>  
#include <memory>  
#include "World.h"  
#include "Entity.hpp"  
#include "TransformComponent.hpp"  
#include "CameraComponent.hpp"  
#include "MeshComponent.hpp"  
#include "MaterialComponent.hpp"  
#include "LightComponent.hpp"  
#include "ShaderBinder.hpp"  
#include "MaterialManager.hpp"  
#include "ShaderManager.hpp"
#include "GLMesh.hpp"  
#include "ColorUtils.h"  
#include "TextureManager.hpp"  
#include "ShaderProgram.hpp"
#include "Attenuation.h"  
#include "Light.h"  
#include "Material.h"

class RenderableProcessor {
public:
	std::vector<std::shared_ptr<Entity::Entity>> GetRenderables(const World& scene) {
		std::vector<std::shared_ptr<Entity::Entity>> result;
		for (auto& e : scene.GetEntities()) {
			if (e->HasComponent<Component::MeshComponent>() &&
				e->HasComponent<Component::MaterialComponent>() &&
				!e->HasComponent<Component::LightComponent>()) {
				result.push_back(e);
			}
		}
		return result;
	}

	void RenderEntity(std::shared_ptr<Entity::Entity> entity,
		const std::vector<std::shared_ptr<Entity::Entity>>& lights,
		const glm::mat4& view, const glm::mat4& projection,
		const Component::TransformComponent& cameraTransform) {
		auto transform = entity->GetComponent<Component::TransformComponent>();
		auto meshComp = entity->GetComponent<Component::MeshComponent>();
		auto matComp = entity->GetComponent<Component::MaterialComponent>();

		if (!transform || !meshComp || !matComp) return;

		auto material = Material::MaterialManager::Get(matComp->MaterialID);
		if (!material) return;

		ShaderProgram shaderProgram(ShaderManager::Get(material->getShader()));
		shaderProgram.Use();
		ShaderBinder binder(shaderProgram.GetProgramId());

		glm::mat4 model = glm::translate(glm::mat4(1.0f), transform->position);
		model = glm::rotate(model, transform->rotation.x, glm::vec3(1, 0, 0));
		model = glm::rotate(model, transform->rotation.y, glm::vec3(0, 1, 0));
		model = glm::rotate(model, transform->rotation.z, glm::vec3(0, 0, 1));
		model = glm::scale(model, transform->scale);

		binder.setMat4("model", model);
		binder.setMat4("view", view);
		binder.setMat4("projection", projection);

		if (auto phong = std::dynamic_pointer_cast<Material::MaterialPhong>(material)) {
			binder.setVec3("viewPos", cameraTransform.position);
			binder.setInt("material.ambientMap", 0);
			binder.setInt("material.diffuseMap", 1);
			binder.setInt("material.specularMap", 2);

			// Set ambient  
			if (!phong->ambient.isTexture) {
				binder.setVec3("material.ambientColor", ColorUtils::HexToVec3(phong->ambient.color));
				binder.setInt("material.useAmbientMap", 0);
			}
			else {
				binder.setInt("material.useAmbientMap", 1);
				glActiveTexture(GL_TEXTURE0);
				glBindTexture(GL_TEXTURE_2D, Material::TextureManager::Get(phong->ambient.texturePath));

			}

			// Diffuse  
			if (!phong->diffuse.isTexture) {
				binder.setVec3("material.diffuseColor", ColorUtils::HexToVec3(phong->diffuse.color));
				binder.setInt("material.useDiffuseMap", 0);
			}
			else {
				binder.setInt("material.useDiffuseMap", 1);
				glActiveTexture(GL_TEXTURE1);
				glBindTexture(GL_TEXTURE_2D, Material::TextureManager::Get(phong->diffuse.texturePath));
			}

			// Specular  
			if (!phong->specular.isTexture) {
				binder.setVec3("material.specularColor", ColorUtils::HexToVec3(phong->specular.color));
				binder.setInt("material.useSpecularMap", 0);
			}
			else {
				binder.setInt("material.useSpecularMap", 1);
				glActiveTexture(GL_TEXTURE2);
				glBindTexture(GL_TEXTURE_2D, Material::TextureManager::Get(phong->specular.texturePath));
			}

			binder.setFloat("material.shininess", phong->shininess);

			int i = 0;
			for (const auto& light : lights) {
				auto lightComp = light->GetComponent<Component::LightComponent>();
				auto lightTransform = light->GetComponent<Component::TransformComponent>();

				if (!lightComp || !lightTransform) continue;

				if (auto dir = std::dynamic_pointer_cast<Light::LightDirectional>(lightComp->LightBase)) {
					glm::vec3 dirVec = glm::normalize(transform->position - lightTransform->position);
					binder.setVec3("dirLight.direction", dirVec);
					binder.setVec3("dirLight.color", dir->color);
					binder.setFloat("dirLight.intensity", dir->intensity);
				}
				else if (auto pt = std::dynamic_pointer_cast<Light::LightPoint>(lightComp->LightBase)) {
					float constant, linear, quadratic;
					Attenuation::FromRange(pt->range, constant, linear, quadratic);

					std::string prefix = "pointLights[" + std::to_string(i) + "]";
					binder.setVec3(prefix + ".position", lightTransform->position);
					binder.setVec3(prefix + ".color", pt->color);
					binder.setFloat(prefix + ".intensity", pt->intensity);
					binder.setFloat(prefix + ".constant", constant);
					binder.setFloat(prefix + ".linear", linear);
					binder.setFloat(prefix + ".quadratic", quadratic);
					i++;
				}
			}

			binder.setInt("numPointLights", i);
		}

		if (!meshComp->glMesh)
			meshComp->LoadMesh();

		meshComp->glMesh->Render();
	}
};

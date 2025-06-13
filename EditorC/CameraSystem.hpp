#pragma once
#pragma once

#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include "TransformComponent.hpp"
#include "CameraComponent.hpp"

namespace System {

	class CameraSystem {
	public:
		glm::mat4 GetViewMatrix(const Component::TransformComponent& transform, const Component::CameraComponent& cameraComponent) const {
			return glm::lookAt(transform.position, transform.position + cameraComponent.front, cameraComponent.up);
		}

		glm::mat4 GetProjectionMatrix(const Component::CameraComponent& cam, float aspectRatio) const {
			float fovRadians = glm::radians(cam.fieldOfView);
			return glm::perspective(fovRadians, aspectRatio, cam.nearClip, cam.farClip);
		}
	};

}

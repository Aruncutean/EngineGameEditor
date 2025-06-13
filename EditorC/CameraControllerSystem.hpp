#pragma once  

#include <glm/glm.hpp>  
#include <glm/gtc/matrix_transform.hpp>  
#include <memory>  
#include "CameraComponent.hpp"  
#include "TransformComponent.hpp"  
#include "Entity.hpp"  
#include "Input.hpp"  

namespace System {

	class CameraControllerSystem {
	private:
		glm::vec2 lastMousePos{};
		bool firstMove = true;
		float yaw = -90.0f;
		float pitch = 0.0f;

		bool mousePressed = false;
		float moveSpeed = 5.0f;
		float lookSensitivity = 0.1f;

		std::shared_ptr<Component::CameraComponent> camera;
		std::shared_ptr<Component::TransformComponent> transform;

	public:
		explicit CameraControllerSystem(std::shared_ptr<Entity::Entity> cameraEntity) {
			transform = cameraEntity->GetComponent<Component::TransformComponent>();
			camera = cameraEntity->GetComponent<Component::CameraComponent>();

			Input::Instance().MouseClicked = [this](Input::MouseButton mouse) {
				if (mouse == Input::MouseButton::Left) {
					mousePressed = true;
				}
				};

			Input::Instance().MouseReleased = [this](Input::MouseButton mouse) {
				if (mouse == Input::MouseButton::Left) {
					mousePressed = false;
				}
				};

			Input::Instance().MouseMove = [this](glm::vec2 pos) {
				OnMouseMove(pos);
				};
		}

		void OnMouseMove(const glm::vec2& newPos) {
			if (firstMove) {
				lastMousePos = newPos;
				firstMove = false;
			}

			float xoffset = newPos.x - lastMousePos.x;
			float yoffset = lastMousePos.y - newPos.y;
			lastMousePos = newPos;

			if (!mousePressed) return;

			xoffset *= lookSensitivity;
			yoffset *= lookSensitivity;

			yaw += xoffset;
			pitch += yoffset;

			pitch = std::clamp(pitch, -89.0f, 89.0f);

			glm::vec3 front;
			front.x = cos(glm::radians(yaw)) * cos(glm::radians(pitch));
			front.y = sin(glm::radians(pitch));
			front.z = sin(glm::radians(yaw)) * cos(glm::radians(pitch));

			camera->front = glm::normalize(front);
		}

		void Update(float deltaTime) {
			float velocity = moveSpeed * deltaTime;

			if (Input::KeyW() == true)
				transform->position += camera->front * velocity;

			if (Input::KeyS() == true)
				transform->position -= camera->front * velocity;

			glm::vec3 right = glm::normalize(glm::cross(camera->front, camera->up));

			if (Input::KeyA() == true)
				transform->position -= right * velocity;

			if (Input::KeyD() == true)
				transform->position += right * velocity;
		}

		void SetMoveSpeed(float speed) { moveSpeed = speed; }
		void SetSensitivity(float sens) { lookSensitivity = sens; }
	};

}

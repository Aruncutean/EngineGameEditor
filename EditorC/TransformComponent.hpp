#pragma once
#include "IComponent.hpp"
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <boost/property_tree/ptree.hpp>

namespace Component {

	class TransformComponent : public ComponentBase
	{
	public:

		glm::vec3 position = glm::vec3(0.0f);
		glm::vec3 rotation = glm::vec3(0.0f);
		glm::vec3 scale = glm::vec3(1.0f);

		TransformComponent() = default;

		TransformComponent(const boost::property_tree::ptree& pt) {
			position = glm::vec3(
				pt.get<float>("Position.X", 0),
				pt.get<float>("Position.Y", 0),
				pt.get<float>("Position.Z", 0)
			);
			rotation = glm::vec3(
				pt.get<float>("Rotation.X", 0),
				pt.get<float>("Rotation.Y", 0),
				pt.get<float>("Rotation.Z", 0)
			);
			scale = glm::vec3(
				pt.get<float>("Scale.X", 1),
				pt.get<float>("Scale.Y", 1),
				pt.get<float>("Scale.Z", 1)
			);
		}

		glm::mat4 GetTransform() const {
			glm::mat4 trans = glm::mat4(1.0f);
			trans = glm::translate(trans, position);
			trans = glm::rotate(trans, glm::radians(rotation.x), glm::vec3(1, 0, 0));
			trans = glm::rotate(trans, glm::radians(rotation.y), glm::vec3(0, 1, 0));
			trans = glm::rotate(trans, glm::radians(rotation.z), glm::vec3(0, 0, 1));
			trans = glm::scale(trans, scale);
			return trans;
		}

		boost::property_tree::ptree ToPtree() const override {
			boost::property_tree::ptree pt;
			pt.put("Position.X", position.x);
			pt.put("Position.Y", position.y);
			pt.put("Position.Z", position.z);

			pt.put("Rotation.X", rotation.x);
			pt.put("Rotation.Y", rotation.y);
			pt.put("Rotation.Z", rotation.z);

			pt.put("Scale.X", scale.x);
			pt.put("Scale.Y", scale.y);
			pt.put("Scale.Z", scale.z);

			return pt;
		}

		void FromPtree(const boost::property_tree::ptree& pt) override {
			position.x = pt.get<float>("Position.X", 0);
			position.y = pt.get<float>("Position.Y", 0);
			position.z = pt.get<float>("Position.Z", 0);

			rotation.x = pt.get<float>("Rotation.X", 0);
			rotation.y = pt.get<float>("Rotation.Y", 0);
			rotation.z = pt.get<float>("Rotation.Z", 0);

			scale.x = pt.get<float>("Scale.X", 1);
			scale.y = pt.get<float>("Scale.Y", 1);
			scale.z = pt.get<float>("Scale.Z", 1);
		}

		std::string GetTypeName()  const override {
			return "TransformComponent";
		}
	};


}
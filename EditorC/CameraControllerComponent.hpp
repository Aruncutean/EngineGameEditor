#pragma once


#include <glm/glm.hpp>
#include "IComponent.hpp"

namespace Component {

	class CameraControllerComponent : public ComponentBase {
	public:
		float MoveSpeed = 5.0f;
		float LookSensitivity = 0.1f;

		CameraControllerComponent() = default;




		std::string GetTypeName() const override {
			return "CameraControllerComponent";
		}

		boost::property_tree::ptree ToPtree() const override {
			boost::property_tree::ptree pt;
			pt.put("MoveSpeed", MoveSpeed);
			pt.put("LookSensitivity", LookSensitivity);

			return pt;
		}

		void FromPtree(const boost::property_tree::ptree& pt) override {
			MoveSpeed = pt.get<float>("MoveSpeed", 5);
			LookSensitivity = pt.get<float>("LookSensitivity", 0.1f);
		}

	};

}

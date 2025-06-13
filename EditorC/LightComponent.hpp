#pragma once  

#include "IComponent.hpp"  
#include "Light.h"  



namespace Component {
	class LightComponent : public ComponentBase {
	public:
		Light::LightType Type = Light::LightType::Point;

		std::shared_ptr<Light::LightBase> LightBase;

		std::string GetTypeName() const override {
			return "LightComponent";
		}

		boost::property_tree::ptree ToPtree() const override {
			boost::property_tree::ptree pt;

			pt.put("Type", static_cast<int>(Type));

			if (LightBase) {
				pt.add_child("LightBase", LightBase->ToPtree());
			}

			return pt;
		}

		void FromPtree(const boost::property_tree::ptree& pt) override {
			Type = static_cast<Light::LightType>(pt.get<int>("Type", 0));

			if (auto lightData = pt.get_child_optional("LightBase")) {
				LightBase = Light::LightFromPtree(*lightData);
			}
		}

	};
}
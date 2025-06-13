#pragma once
#include "IComponent.hpp"

namespace Component {

	class MaterialComponent : public ComponentBase
	{
	public:
		std::string MaterialID;

		MaterialComponent() = default;

		std::string GetTypeName() const override {
			return "MaterialComponent";
		}
		boost::property_tree::ptree ToPtree() const override {
			boost::property_tree::ptree pt;
			pt.put("MaterialID", MaterialID);
			return pt;
		}

		void FromPtree(const boost::property_tree::ptree& pt) override {
			MaterialID = pt.get<std::string>("MaterialID", "");
		}
	};
}
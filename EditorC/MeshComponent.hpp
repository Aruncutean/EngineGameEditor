#pragma once

#include "IComponent.hpp"
#include <string>
#include <memory>
#include <iostream>
#include "MeshData.h"
#include "GLMesh.hpp"
#include "MeshRegistry.hpp"

namespace Component {

	class MeshComponent : public ComponentBase
	{
	public:
		std::string meshPath;

		// Not serializable
		std::shared_ptr<Model::MeshData> runtimeMesh;
		std::unique_ptr<GLMesh> glMesh = nullptr;

		MeshComponent() = default;

		void LoadMesh() {

			auto project = AppContext::Instance().GetCurrentProject();

			runtimeMesh = MeshRegistry::GetMesh(project.path + "/" + meshPath + ".mesh.json");
			if (runtimeMesh) {
				glMesh = std::make_unique<GLMesh>(*runtimeMesh);
			}
			else {
				std::cerr << "Failed to load mesh: " << meshPath << std::endl;
			}
		}
		std::string GetTypeName() const override {
			return "MeshComponent";
		}

		boost::property_tree::ptree ToPtree() const override {
			boost::property_tree::ptree pt;
			pt.put("MeshPath", meshPath);
			return pt;
		}

		void FromPtree(const boost::property_tree::ptree& pt) override {
			meshPath = pt.get<std::string>("MeshPath", "");
			LoadMesh();
		}



	};


}
#pragma once
#include "IComponent.hpp"
#include <string>
#include <glm/glm.hpp>
#include "bullet/btBulletDynamicsCommon.h"
#include <boost/property_tree/ptree.hpp>

namespace Component {

	class PhysicsComponent : public ComponentBase
	{
	public:
		float mass = 1.0f;
		bool isKinematic = false;
		btRigidBody* rigidBody = nullptr;

		btCollisionShape* shape = nullptr;
		btDefaultMotionState* motionState = nullptr;

		~PhysicsComponent() {
			delete rigidBody;
			delete motionState;
			delete shape;
		}

		PhysicsComponent() = default;

		std::string GetTypeName() const override {
			return "PhysicsComponent ";
		}
		boost::property_tree::ptree ToPtree() const override {
			boost::property_tree::ptree pt;

			return pt;
		}

		void FromPtree(const boost::property_tree::ptree& pt) override {

		}
	};
}
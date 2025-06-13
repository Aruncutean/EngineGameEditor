#pragma once
#include <bullet/btBulletDynamicsCommon.h>
#include <functional>
#include <vector>

class PhysicsSystem {
public:
	btBroadphaseInterface* broadphase;
	btDefaultCollisionConfiguration* collisionConfiguration;
	btCollisionDispatcher* dispatcher;
	btSequentialImpulseConstraintSolver* solver;
	btDiscreteDynamicsWorld* dynamicsWorld;

	using CollisionCallback = std::function<void(btRigidBody* a, btRigidBody* b)>;
	std::vector<CollisionCallback> collisionCallbacks;

	PhysicsSystem() {
		broadphase = new btDbvtBroadphase();
		collisionConfiguration = new btDefaultCollisionConfiguration();
		dispatcher = new btCollisionDispatcher(collisionConfiguration);
		solver = new btSequentialImpulseConstraintSolver;
		dynamicsWorld = new btDiscreteDynamicsWorld(dispatcher, broadphase, solver, collisionConfiguration);
		dynamicsWorld->setGravity(btVector3(0, -9.81f, 0));
	}

	void AddCallback(CollisionCallback cb) {
		collisionCallbacks.push_back(cb);
	}

	void Step(float deltaTime) {
		dynamicsWorld->stepSimulation(deltaTime);

		int numManifolds = dynamicsWorld->getDispatcher()->getNumManifolds();
		for (int i = 0; i < numManifolds; ++i) {
			btPersistentManifold* contactManifold = dynamicsWorld->getDispatcher()->getManifoldByIndexInternal(i);
			btRigidBody* bodyA = (btRigidBody*)contactManifold->getBody0();
			btRigidBody* bodyB = (btRigidBody*)contactManifold->getBody1();

			if (contactManifold->getNumContacts() > 0) {
				for (auto& cb : collisionCallbacks) {
					cb(bodyA, bodyB);
				}
			}
		}
	}

	~PhysicsSystem() {
		delete dynamicsWorld;
		delete solver;
		delete dispatcher;
		delete collisionConfiguration;
		delete broadphase;
	}
};

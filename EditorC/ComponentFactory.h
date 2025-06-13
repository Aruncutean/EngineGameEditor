#pragma once
#include <string>
#include <unordered_map>
#include <functional>
#include <memory>
#include <stdexcept>
#include "IComponent.hpp"

namespace Component {

	class ComponentFactory {
	public:
		using CreatorFunc = std::function<std::shared_ptr<ComponentBase>()>;

		static ComponentFactory& Instance() {
			static ComponentFactory instance;
			return instance;
		}

		template<typename T>
		void Register(const std::string& typeName) {
			creators[typeName] = []() {
				return std::make_shared<T>();
				};
		}

		std::shared_ptr<ComponentBase> Create(const std::string& typeName) const {
			auto it = creators.find(typeName);
			if (it != creators.end()) {
				return it->second();
			}
			throw std::runtime_error("Unknown component type: " + typeName);
		}

		bool IsRegistered(const std::string& typeName) const {
			return creators.find(typeName) != creators.end();
		}

	private:
		std::unordered_map<std::string, CreatorFunc> creators;
	};

} // namespace Component

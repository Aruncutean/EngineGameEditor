#include "MeshDataSerializer.hpp"
#include <fstream>

namespace Service {

	class MeshService
	{
	public:

		static void Save(const Model::MeshData& mesh, const std::string& path) {
			boost::property_tree::ptree root = Service::MeshDataSerializer::Serialize(mesh);
			boost::property_tree::write_json(path, root);
		}

		static Model::MeshData Load(const std::string& path) {
			boost::property_tree::ptree root;
			boost::property_tree::read_json(path, root);
			return Service::MeshDataSerializer::Deserialize(root);
		}

	private:

	};



}


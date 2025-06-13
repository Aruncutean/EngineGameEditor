#include "Material.h"


std::shared_ptr<Material::MaterialBase> Material::MaterialBase::fromPtree(const boost::property_tree::ptree& pt) {
	std::string type = pt.get<std::string>("$type");

	std::shared_ptr<Material::MaterialBase> mat;

	if (type == "phong") {
		auto phong = std::make_shared<Material::MaterialPhong>();
		phong->shininess = pt.get<float>("Shininess", 32.0f);
		phong->ambient = ColorOrTexture::fromPtree(pt.get_child("Ambient"));
		phong->diffuse = ColorOrTexture::fromPtree(pt.get_child("Diffuse"));
		phong->specular = ColorOrTexture::fromPtree(pt.get_child("Specular"));
		mat = phong;
	}
	else if (type == "pbr") {
		auto pbr = std::make_shared<Material::MaterialPBR>();
		pbr->albedoMap = pt.get<std::string>("albedoMap", "");
		pbr->metallic = pt.get<float>("metallic", 0.5f);
		pbr->roughness = pt.get<float>("roughness", 0.5f);
		pbr->emissive[0] = pt.get<float>("emissive.x", 0);
		pbr->emissive[1] = pt.get<float>("emissive.y", 0);
		pbr->emissive[2] = pt.get<float>("emissive.z", 0);
		mat = pbr;
	}
	else if (type == "default") {
		auto def = std::make_shared<Material::MaterialDefault>();
		def->shininess = pt.get<float>("shininess", 32.0f);
		def->diffuseColor[0] = pt.get<float>("diffuseColor.x", 1);
		def->diffuseColor[1] = pt.get<float>("diffuseColor.y", 1);
		def->diffuseColor[2] = pt.get<float>("diffuseColor.z", 1);
		def->specularColor[0] = pt.get<float>("specularColor.x", 1);
		def->specularColor[1] = pt.get<float>("specularColor.y", 1);
		def->specularColor[2] = pt.get<float>("specularColor.z", 1);
		mat = def;
	}
	else {
		throw std::runtime_error("Unknown material type: " + type);
	}

	// Setează câmpurile comune
	mat->id = pt.get<std::string>("Id", "");
	mat->name = pt.get<std::string>("Name", "");
	mat->path = pt.get<std::string>("Path", "");

	return mat;
}

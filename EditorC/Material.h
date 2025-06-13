#pragma once
#include <string>
#include <memory>
#include <boost/property_tree/ptree.hpp>
#include "ShaderTypes.h"

struct ColorOrTexture {
	std::string color = "#ffffff";
	std::string texturePath;
	bool isTexture = false;

	boost::property_tree::ptree toPtree() const {
		boost::property_tree::ptree pt;
		pt.put("Color", color);
		pt.put("TexturePath", texturePath);
		pt.put("IsTexture", isTexture);
		return pt;
	}

	static ColorOrTexture fromPtree(const boost::property_tree::ptree& pt) {
		ColorOrTexture c;
		c.color = pt.get<std::string>("Color", "#ffffff");
		c.texturePath = pt.get<std::string>("TexturePath", "");
		c.isTexture = pt.get<bool>("IsTexture", false);
		return c;
	}
};

namespace Material {

	class MaterialBase {
	public:
		std::string id;
		std::string name;
		std::string path;

		virtual ShaderType getShader() const = 0;
		virtual boost::property_tree::ptree toPtree() const = 0;
		~MaterialBase() = default;

		static std::shared_ptr<MaterialBase> fromPtree(const boost::property_tree::ptree& pt);

	};

	class MaterialPhong : public MaterialBase {
	public:
		ColorOrTexture ambient;
		ColorOrTexture diffuse;
		ColorOrTexture specular;
		float shininess = 32.0f;

		MaterialPhong() = default;

		MaterialPhong(const boost::property_tree::ptree& pt) {
			shininess = pt.get<float>("Shininess", 32.0f);
			ambient = ColorOrTexture::fromPtree(pt.get_child("Ambient"));
			diffuse = ColorOrTexture::fromPtree(pt.get_child("Diffuse"));
			specular = ColorOrTexture::fromPtree(pt.get_child("Specular"));
		}

		ShaderType getShader() const override { return ShaderType::Phong; }

		boost::property_tree::ptree toPtree() const override {
			boost::property_tree::ptree pt;
			pt.put("$type", "phong");
			pt.put("Shininess", shininess);
			pt.add_child("Ambient", ambient.toPtree());
			pt.add_child("Diffuse", diffuse.toPtree());
			pt.add_child("Specular", specular.toPtree());
			pt.put("Id", id);
			pt.put("Name", name);
			pt.put("Path", path);
			return pt;
		}
	};

	class MaterialPBR : public MaterialBase {
	public:
		std::string albedoMap;
		float metallic = 0.5f;
		float roughness = 0.5f;
		float emissive[3] = { 0, 0, 0 };

		MaterialPBR() = default;

		MaterialPBR(const boost::property_tree::ptree& pt) {
			albedoMap = pt.get<std::string>("albedoMap", "");
			metallic = pt.get<float>("metallic", 0.5f);
			roughness = pt.get<float>("roughness", 0.5f);
			emissive[0] = pt.get<float>("emissive.x", 0.0f);
			emissive[1] = pt.get<float>("emissive.y", 0.0f);
			emissive[2] = pt.get<float>("emissive.z", 0.0f);
		}

		ShaderType getShader() const { return ShaderType::PBR; }

		boost::property_tree::ptree toPtree() const {
			boost::property_tree::ptree pt;
			pt.put("$type", "pbr");
			pt.put("albedoMap", albedoMap);
			pt.put("metallic", metallic);
			pt.put("roughness", roughness);
			pt.put("emissive.x", emissive[0]);
			pt.put("emissive.y", emissive[1]);
			pt.put("emissive.z", emissive[2]);
			pt.put("id", id);
			pt.put("name", name);
			pt.put("path", path);
			return pt;
		}


	};

	class MaterialDefault : public MaterialBase {
	public:
		float diffuseColor[3] = { 1, 1, 1 };
		float specularColor[3] = { 1, 1, 1 };
		float shininess = 32.0f;

		MaterialDefault() = default;

		MaterialDefault(const boost::property_tree::ptree& pt) {
			shininess = pt.get<float>("shininess", 32.0f);
			diffuseColor[0] = pt.get<float>("diffuseColor.x", 1.0f);
			diffuseColor[1] = pt.get<float>("diffuseColor.y", 1.0f);
			diffuseColor[2] = pt.get<float>("diffuseColor.z", 1.0f);
			specularColor[0] = pt.get<float>("specularColor.x", 1.0f);
			specularColor[1] = pt.get<float>("specularColor.y", 1.0f);
			specularColor[2] = pt.get<float>("specularColor.z", 1.0f);

		}

		ShaderType getShader() const override { return ShaderType::Basic; }

		boost::property_tree::ptree toPtree() const override {
			boost::property_tree::ptree pt;
			pt.put("$type", "default");
			pt.put("shininess", shininess);
			pt.put("diffuseColor.x", diffuseColor[0]);
			pt.put("diffuseColor.y", diffuseColor[1]);
			pt.put("diffuseColor.z", diffuseColor[2]);
			pt.put("specularColor.x", specularColor[0]);
			pt.put("specularColor.y", specularColor[1]);
			pt.put("specularColor.z", specularColor[2]);
			pt.put("id", id);
			pt.put("name", name);
			pt.put("path", path);
			return pt;
		}
	};

}

#pragma once
#include <assimp/Importer.hpp>
#include <assimp/scene.h>
#include <assimp/postprocess.h>
#include "MeshData.h"
#include <stdexcept>

class LoadEntity {
public:
	Model::MeshData LoadMesh(const std::string& filePath) {
		Assimp::Importer importer;
		const aiScene* scene = importer.ReadFile(filePath,
			aiProcess_Triangulate | aiProcess_JoinIdenticalVertices);

		if (!scene || !scene->HasMeshes()) {
			throw std::runtime_error("No mesh found in file or failed to load.");
		}

		const aiMesh* mesh = scene->mMeshes[0];
		Model::MeshData data;

		for (unsigned int i = 0; i < mesh->mNumVertices; ++i) {
			data.positions.emplace_back(
				mesh->mVertices[i].x,
				mesh->mVertices[i].y,
				mesh->mVertices[i].z
			);

			if (mesh->HasNormals()) {
				data.normals.emplace_back(
					mesh->mNormals[i].x,
					mesh->mNormals[i].y,
					mesh->mNormals[i].z
				);
			}

			if (mesh->HasTextureCoords(0)) {
				data.uvs.emplace_back(
					mesh->mTextureCoords[0][i].x,
					mesh->mTextureCoords[0][i].y
				);
			}
		}

		for (unsigned int i = 0; i < mesh->mNumFaces; ++i) {
			const aiFace& face = mesh->mFaces[i];
			for (unsigned int j = 0; j < face.mNumIndices; ++j) {
				data.indices.push_back(face.mIndices[j]);
			}
		}

		return data;
	}
};

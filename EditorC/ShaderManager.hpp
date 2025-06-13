#pragma once

#include <glad/glad.h>
#include <string>
#include <unordered_map>
#include <stdexcept>
#include <fstream>
#include <sstream>
#include <iostream>
#include "ShaderTypes.h"



class ShaderManager {
public:
	static void Init() {

		shaders.clear();
	}

	static void LoadShaders() {
		Load(ShaderType::Basic, "shader/basic.vert", "shader/basic.frag");
		//		Load(ShaderType::Gizmo, "shader/gizmo.vert", "shader/gizmo.frag");
		Load(ShaderType::Phong, "shader/phong.vert", "shader/phong.frag");
	}

	static GLuint Get(ShaderType id) {
		if (shaders.find(id) == shaders.end())
			throw std::runtime_error("Shader not loaded.");

		return shaders.at(id);
	}

private:
	static std::unordered_map<ShaderType, GLuint> shaders;

	static GLuint Load(ShaderType id, const std::string& vertPath, const std::string& fragPath) {
		if (shaders.find(id) != shaders.end())
			return shaders[id];

		std::string vertexCode = readFile(vertPath);
		std::string fragmentCode = readFile(fragPath);

		GLuint vertexShader = compileShader(GL_VERTEX_SHADER, vertexCode);
		GLuint fragmentShader = compileShader(GL_FRAGMENT_SHADER, fragmentCode);

		GLuint program = glCreateProgram();
		glAttachShader(program, vertexShader);
		glAttachShader(program, fragmentShader);
		glLinkProgram(program);

		GLint success;
		glGetProgramiv(program, GL_LINK_STATUS, &success);
		if (!success) {
			char infoLog[512];
			glGetProgramInfoLog(program, 512, nullptr, infoLog);
			throw std::runtime_error("Shader linking failed: " + std::string(infoLog));
		}

		glDeleteShader(vertexShader);
		glDeleteShader(fragmentShader);

		shaders[id] = program;
		return program;
	}

	static GLuint compileShader(GLenum type, const std::string& source) {
		GLuint shader = glCreateShader(type);
		const char* src = source.c_str();
		glShaderSource(shader, 1, &src, nullptr);
		glCompileShader(shader);

		GLint status;
		glGetShaderiv(shader, GL_COMPILE_STATUS, &status);
		if (!status) {
			char infoLog[512];
			glGetShaderInfoLog(shader, 512, nullptr, infoLog);
			throw std::runtime_error("Shader compilation failed: " + std::string(infoLog));
		}

		return shader;
	}

	static std::string readFile(const std::string& path) {
		std::ifstream file(path);
		if (!file)
			throw std::runtime_error("Failed to open shader file: " + path);

		std::stringstream buffer;
		buffer << file.rdbuf();
		return buffer.str();
	}
};

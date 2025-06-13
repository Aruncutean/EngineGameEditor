#pragma once

#include <glad/glad.h>

class ShaderProgram {
public:
	explicit ShaderProgram(GLuint programId)
		: m_programId(programId) {
	}

	void Use() const {
		glUseProgram(m_programId);
	}

	GLuint GetProgramId() const {
		return m_programId;
	}

private:
	GLuint m_programId;
};

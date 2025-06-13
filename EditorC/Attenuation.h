#pragma once


#include <string>
#include <sstream>
#include <iomanip>
#include <glm/glm.hpp>
#include <stdexcept>

class Attenuation {

public:
	static
		void FromRange(float range, float& constant, float& linear, float& quadratic) {
		constant = 1.0f;
		linear = 4.5f / range;
		quadratic = 75.0f / (range * range);
	}
};

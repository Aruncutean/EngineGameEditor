#include "Input.hpp"

std::function<void(Input::Key)> Input::KeyPressed;
std::function<void(Input::Key)> Input::KeyReleased;
std::function<void(Input::MouseButton)> Input::MouseClicked;
std::function<void(Input::MouseButton)> Input::MouseReleased;
std::function<void(glm::vec2)> Input::MouseMove;

std::unordered_map<Input::Key, bool> Input::keys;
std::unordered_map<Uint8, bool> Input::mouseButtons;
glm::vec2 Input::mousePosition{ 0.0f };
glm::vec2 Input::scrollDelta{ 0.0f };
bool Input::isActive = false;

Input& Input::Instance() {
	static Input instance;
	return instance;
}

void Input::ProcessEvent(const SDL_Event& e) {

	if (e.type== SDL_MOUSEMOTION)
	{
		mousePosition = glm::vec2(e.motion.x, e.motion.y);
		if (MouseMove) MouseMove(mousePosition);
	}
	if (e.type == SDL_MOUSEBUTTONUP) {
		mouseButtons[e.button.button] = false;
		if (MouseReleased) MouseReleased(static_cast<MouseButton>(e.button.button));
	}
		
	


	if (isActive == true) {
		switch (e.type) {
		case SDL_KEYDOWN:
			if (!e.key.repeat) {
				keys[e.key.keysym.sym] = true;
				if (KeyPressed) KeyPressed(e.key.keysym.sym);
			}
			break;
		case SDL_KEYUP:
			keys[e.key.keysym.sym] = false;
			if (KeyReleased) KeyReleased(e.key.keysym.sym);
			break;
		case SDL_MOUSEBUTTONDOWN:
			mouseButtons[e.button.button] = true;
			if (MouseClicked) MouseClicked(static_cast<MouseButton>(e.button.button));
			break;
		
		 
			
		case SDL_MOUSEWHEEL:
			scrollDelta = glm::vec2(static_cast<float>(e.wheel.x), static_cast<float>(e.wheel.y));
			break;
		}
	}
}

void Input::ResetScroll() {
	scrollDelta = glm::vec2(0.0f);
}

bool Input::IsKeyDown(Key key) {
	auto it = keys.find(key);
	return it != keys.end() && it->second;
}

bool Input::IsMouseButtonDown(MouseButton btn) {
	auto it = mouseButtons.find(static_cast<Uint8>(btn));
	return it != mouseButtons.end() && it->second;
}

glm::vec2 Input::GetMousePosition() {
	return mousePosition;
}

glm::vec2 Input::GetScrollDelta() {
	return scrollDelta;
}

bool Input::KeyW() { return IsKeyDown(SDLK_w); }
bool Input::KeyS() { return IsKeyDown(SDLK_s); }
bool Input::KeyA() { return IsKeyDown(SDLK_a); }
bool Input::KeyD() { return IsKeyDown(SDLK_d); }
bool Input::KeyT() { return IsKeyDown(SDLK_t); }
bool Input::KeyR() { return IsKeyDown(SDLK_r); }
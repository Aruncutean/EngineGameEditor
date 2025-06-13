#pragma once
#include <SDL2/SDL.h>
#include <glm/glm.hpp>
#include <functional>
#include <unordered_map>

class Input {
public:
	using Key = SDL_Keycode;
	enum class MouseButton : Uint8 {
		Left = SDL_BUTTON_LEFT,
		Right = SDL_BUTTON_RIGHT,
		Middle = SDL_BUTTON_MIDDLE
	};

	static bool isActive;
	static Input& Instance();

	static std::function<void(Key)> KeyPressed;
	static std::function<void(Key)> KeyReleased;
	static std::function<void(MouseButton)> MouseClicked;
	static std::function<void(MouseButton)> MouseReleased;
	static std::function<void(glm::vec2)> MouseMove;

	static void ProcessEvent(const SDL_Event& e);
	static void ResetScroll();
	static bool IsKeyDown(Key key);
	static bool IsMouseButtonDown(MouseButton btn);
	static glm::vec2 GetMousePosition();
	static glm::vec2 GetScrollDelta();

	static bool KeyW();
	static bool KeyS();
	static bool KeyA();
	static bool KeyD();
	static bool KeyR();
	static bool KeyT();
	


private:
	Input() = default;

	static std::unordered_map<Key, bool> keys;
	static std::unordered_map<Uint8, bool> mouseButtons;
	static glm::vec2 mousePosition;
	static glm::vec2 scrollDelta;
};

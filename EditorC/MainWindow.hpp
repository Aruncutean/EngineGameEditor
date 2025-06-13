#pragma once
#include <SDL2/SDL.h>
#include <SDL2/SDL_main.h>

#include <iostream>
#include <glad/glad.h>

#include "imgui.h"
#include "imgui_stdlib.h"
#include "imgui_impl_opengl3.h"
#include "imgui_impl_sdl2.h"
#include "imgui_impl_opengl3_loader.h"
#include <mutex>
#include "Window.h"
#include <vector>
#include "Input.hpp"
#include "WindowService.hpp"

class MainWindow {


public:

	MainWindow() {
		if (SDL_Init(SDL_INIT_VIDEO) != 0)
		{
			std::cerr << "SDL_Init Error: " << SDL_GetError() << std::endl;
			SDL_Quit();
		}

		SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, 3);
		SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, 3);
		SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);

		window = SDL_CreateWindow("ImGui + SDL2",
			SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED,
			1280, 720,
			SDL_WINDOW_OPENGL | SDL_WINDOW_RESIZABLE);

		if (!window)
		{
			std::cerr << "Failed to create window: " << SDL_GetError() << std::endl;
			SDL_Quit();
		}

		gl_context = SDL_GL_CreateContext(window);
		SDL_GL_MakeCurrent(window, gl_context);
		SDL_GL_SetSwapInterval(1);

		initOpenglContext();

		initImgui();
	}

	void Run() {
		SDL_Event event;
		while (running)
		{
			float currentFrame = SDL_GetTicks() / 1000.0f;  // SDL_GetTicks() e �n ms
			deltaTime = currentFrame - lastFrame;
			lastFrame = currentFrame;

			fpsTimer += deltaTime;
			frameCounter++;
			if (fpsTimer >= 1.0f) {
				currentFPS = frameCounter;
				frameCounter = 0;
				fpsTimer = 0.0f;
			}

			WindowService::getInstance()->setDeltaTime(deltaTime);


			while (SDL_PollEvent(&event))
			{
				Input::ProcessEvent(event);
				ImGui_ImplSDL2_ProcessEvent(&event);
				if (event.type == SDL_QUIT)
					running = false;
			}
			ImGui_ImplOpenGL3_NewFrame();
			ImGui_ImplSDL2_NewFrame();
			ImGui::NewFrame();

			if (IsBockingSpace == true) {
				ShowDockSpace();
			}

			for (auto& w : windows)
				w->Render();

			ImGui::Render();

			glViewport(0, 0, 1280, 720);
			glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
			glClear(GL_COLOR_BUFFER_BIT);

			ImGui_ImplOpenGL3_RenderDrawData(ImGui::GetDrawData());
			ImGuiIO& io = ImGui::GetIO(); (void)io;
			if (io.ConfigFlags & ImGuiConfigFlags_ViewportsEnable)
			{
				SDL_Window* backup_current_window = SDL_GL_GetCurrentWindow();
				SDL_GLContext backup_context = SDL_GL_GetCurrentContext();

				ImGui::UpdatePlatformWindows();
				ImGui::RenderPlatformWindowsDefault();

				SDL_GL_MakeCurrent(backup_current_window, backup_context);
			}
			SDL_GL_SwapWindow(window);

			ProcessWindowQueue();
		}

		ImGui_ImplOpenGL3_Shutdown();
		ImGui_ImplSDL2_Shutdown();
		ImGui::DestroyContext();

		SDL_GL_DeleteContext(gl_context);
		SDL_DestroyWindow(window);
		SDL_Quit();
		isClosed = true;
	}

	void AddWindow(std::shared_ptr<Window> window) {
		windowsToAdd.push_back(window);
	}

	void RemoveWindow(std::shared_ptr<Window> window) {
		windowsToRemove.push_back(window);
	}

	~MainWindow() {

	}

	bool IsBockingSpace = false;

private:
	void initOpenglContext() {
		if (!gladLoadGLLoader((GLADloadproc)SDL_GL_GetProcAddress))
		{
			std::cerr << "Failed to initialize glad\n";
			SDL_Quit();
		}
	}

	void initImgui() {
		IMGUI_CHECKVERSION();
		ImGui::CreateContext();
		ImGuiIO& io = ImGui::GetIO(); (void)io;

		io.ConfigFlags |= ImGuiConfigFlags_DockingEnable;
		io.ConfigFlags |= ImGuiConfigFlags_ViewportsEnable;

		ImGui::StyleColorsDark();

		ImGui_ImplSDL2_InitForOpenGL(window, gl_context);
		ImGui_ImplOpenGL3_Init("#version 330");
	}

	void ShowDockSpace()
	{
		static bool open = true;
		static bool opt_fullscreen = true;
		static ImGuiDockNodeFlags dockspace_flags = ImGuiDockNodeFlags_None;

		ImGuiWindowFlags window_flags = ImGuiWindowFlags_MenuBar | ImGuiWindowFlags_NoDocking;
		if (opt_fullscreen)
		{
			ImGuiViewport* viewport = ImGui::GetMainViewport();
			ImGui::SetNextWindowPos(viewport->Pos);
			ImGui::SetNextWindowSize(viewport->Size);
			ImGui::SetNextWindowViewport(viewport->ID);
			ImGui::PushStyleVar(ImGuiStyleVar_WindowRounding, 0.0f);
			ImGui::PushStyleVar(ImGuiStyleVar_WindowBorderSize, 0.0f);
			window_flags |= ImGuiWindowFlags_NoTitleBar | ImGuiWindowFlags_NoCollapse | ImGuiWindowFlags_NoResize | ImGuiWindowFlags_NoMove;
			window_flags |= ImGuiWindowFlags_NoBringToFrontOnFocus | ImGuiWindowFlags_NoNavFocus;
		}

		ImGui::Begin("DockSpace Demo", &open, window_flags);
		if (opt_fullscreen) ImGui::PopStyleVar(2);

		// DockSpace
		ImGuiIO& io = ImGui::GetIO();
		ImGuiID dockspace_id = ImGui::GetID("MyDockSpace");
		ImGui::DockSpace(dockspace_id, ImVec2(0.0f, 0.0f), dockspace_flags);

		ImGui::End();
	}

	std::vector<std::shared_ptr<Window>> windows;
	std::vector<std::shared_ptr<Window>> windowsToAdd;
	std::vector<std::shared_ptr<Window>> windowsToRemove;

	void ProcessWindowQueue() {

		for (auto& toRemove : windowsToRemove) {
			windows.erase(std::remove(windows.begin(), windows.end(), toRemove), windows.end());
		}
		windowsToRemove.clear();


		for (auto& toAdd : windowsToAdd) {
			windows.push_back(toAdd);
		}
		windowsToAdd.clear();
	}

	SDL_Window* window = nullptr;
	SDL_GLContext gl_context = nullptr;



	std::string title;
	int width = 800;
	int height = 600;
	std::atomic<bool> isClosed = false;
	bool running = true;


	float deltaTime = 0.0f;
	float lastFrame = 0.0f;
	int frameCounter = 0;
	float fpsTimer = 0.0f;
	int currentFPS = 0;

};

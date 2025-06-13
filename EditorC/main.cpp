//#define SDL_MAIN_HANDLED
//#include <SDL2/SDL.h>
//#include <SDL2/SDL_main.h>
//#include <iostream>
//#include "imgui.h"
//#include <glad/glad.h>
//#include "imgui_stdlib.h"
//#include "imgui_impl_opengl3.h"
//#include "imgui_impl_sdl2.h"
//#include "imgui_impl_opengl3_loader.h"
//#include <ImGuizmo.h>
//#include <cmath> 
//
//
//void BuildPerspectiveMatrix(float* matrix, float fov, float aspect, float nearZ, float farZ)
//{
//	float f = 1.0f / tanf(fov * 0.5f);
//	matrix[0] = f / aspect;
//	matrix[1] = 0.0f;
//	matrix[2] = 0.0f;
//	matrix[3] = 0.0f;
//
//	matrix[4] = 0.0f;
//	matrix[5] = f;
//	matrix[6] = 0.0f;
//	matrix[7] = 0.0f;
//
//	matrix[8] = 0.0f;
//	matrix[9] = 0.0f;
//	matrix[10] = (farZ + nearZ) / (nearZ - farZ);
//	matrix[11] = -1.0f;
//
//	matrix[12] = 0.0f;
//	matrix[13] = 0.0f;
//	matrix[14] = (2 * farZ * nearZ) / (nearZ - farZ);
//	matrix[15] = 0.0f;
//}
//
//void BuildViewMatrix(float* matrix, float eyeX, float eyeY, float eyeZ)
//{
//	// Very basic lookAt matrix (camera at (eyeX,eyeY,eyeZ) looking at origin)
//	matrix[0] = 1; matrix[1] = 0; matrix[2] = 0; matrix[3] = 0;
//	matrix[4] = 0; matrix[5] = 1; matrix[6] = 0; matrix[7] = 0;
//	matrix[8] = 0; matrix[9] = 0; matrix[10] = 1; matrix[11] = 0;
//	matrix[12] = -eyeX; matrix[13] = -eyeY; matrix[14] = -eyeZ; matrix[15] = 1;
//}
//
//void ShowDockSpace()
//{
//	static bool open = true;
//	static bool opt_fullscreen = true;
//	static ImGuiDockNodeFlags dockspace_flags = ImGuiDockNodeFlags_None;
//
//	ImGuiWindowFlags window_flags = ImGuiWindowFlags_MenuBar | ImGuiWindowFlags_NoDocking;
//	if (opt_fullscreen)
//	{
//		ImGuiViewport* viewport = ImGui::GetMainViewport();
//		ImGui::SetNextWindowPos(viewport->Pos);
//		ImGui::SetNextWindowSize(viewport->Size);
//		ImGui::SetNextWindowViewport(viewport->ID);
//		ImGui::PushStyleVar(ImGuiStyleVar_WindowRounding, 0.0f);
//		ImGui::PushStyleVar(ImGuiStyleVar_WindowBorderSize, 0.0f);
//		window_flags |= ImGuiWindowFlags_NoTitleBar | ImGuiWindowFlags_NoCollapse | ImGuiWindowFlags_NoResize | ImGuiWindowFlags_NoMove;
//		window_flags |= ImGuiWindowFlags_NoBringToFrontOnFocus | ImGuiWindowFlags_NoNavFocus;
//	}
//
//	ImGui::Begin("DockSpace Demo", &open, window_flags);
//	if (opt_fullscreen) ImGui::PopStyleVar(2);
//
//	// DockSpace
//	ImGuiIO& io = ImGui::GetIO();
//	ImGuiID dockspace_id = ImGui::GetID("MyDockSpace");
//	ImGui::DockSpace(dockspace_id, ImVec2(0.0f, 0.0f), dockspace_flags);
//
//	ImGui::End();
//}
//
//
//int main(int argc, char* argv[])
//{
//	if (SDL_Init(SDL_INIT_VIDEO) != 0)
//	{
//		std::cerr << "SDL_Init Error: " << SDL_GetError() << std::endl;
//		return -1;
//	}
//
//	// Context OpenGL
//	SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, 3);
//	SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, 3);
//	SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);
//
//	SDL_Window* window = SDL_CreateWindow("ImGui + SDL2",
//		SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED,
//		1280, 720,
//		SDL_WINDOW_OPENGL | SDL_WINDOW_RESIZABLE);
//
//	if (!window)
//	{
//		std::cerr << "Failed to create window: " << SDL_GetError() << std::endl;
//		return -1;
//	}
//
//	SDL_GLContext gl_context = SDL_GL_CreateContext(window);
//	SDL_GL_MakeCurrent(window, gl_context);
//	SDL_GL_SetSwapInterval(1); // VSync
//
//	if (!gladLoadGLLoader((GLADloadproc)SDL_GL_GetProcAddress))
//	{
//		std::cerr << "Failed to initialize glad\n";
//		return -1;
//	}
//
//	// Init ImGui
//	IMGUI_CHECKVERSION();
//	ImGui::CreateContext();
//	ImGuiIO& io = ImGui::GetIO(); (void)io;
//
//
//	io.ConfigFlags |= ImGuiConfigFlags_DockingEnable;
//	io.ConfigFlags |= ImGuiConfigFlags_ViewportsEnable;
//
//	ImGui::StyleColorsDark();
//
//	ImGui_ImplSDL2_InitForOpenGL(window, gl_context);
//	ImGui_ImplOpenGL3_Init("#version 330");
//
//	bool running = true;
//	SDL_Event event;
//	while (running)
//	{
//		while (SDL_PollEvent(&event))
//		{
//			ImGui_ImplSDL2_ProcessEvent(&event);
//			if (event.type == SDL_QUIT)
//				running = false;
//		}
//
//
//		ImGui_ImplOpenGL3_NewFrame();
//		ImGui_ImplSDL2_NewFrame();
//		ImGui::NewFrame();
//		ImGuizmo::SetImGuiContext(ImGui::GetCurrentContext());
//
//		ShowDockSpace();
//
//		ImGuizmo::BeginFrame();
//
//		float modelMatrix[16] = {
//	1, 0, 0, 0,
//	0, 1, 0, 0,
//	0, 0, 1, 0,
//	0, 0, 0, 1
//		};
//
//
//		float view[16], projection[16], model[16] = {
//			1, 0, 0, 0,
//			0, 1, 0, 0,
//			0, 0, 1, 0,
//			0, 0, 0, 1
//		};
//
//		BuildViewMatrix(view, 0, 5, 10);
//		BuildPerspectiveMatrix(projection, 45.0f * 3.14159f / 180.0f, 1280.0f / 720.0f, 0.1f, 100.0f);
//
//		ImGui::Begin("Hello, ImGui!");
//		ImGui::Text("This is a demo window.");
//
//		ImGuizmo::SetRect(0, 0, 1280, 720);
//
//		float identityMatrix[16] =
//		{
//			1, 0, 0, 0,
//			0, 1, 0, 0,
//			0, 0, 1, 0,
//			0, 0, 0, 1
//		};
//		ImGuizmo::Manipulate(view, projection, ImGuizmo::TRANSLATE, ImGuizmo::LOCAL, model);
//		ImGui::End();
//
//		// Rendering
//		ImGui::Render();
//
//		glViewport(0, 0, 1280, 720);
//		glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
//		glClear(GL_COLOR_BUFFER_BIT);
//
//		ImGui_ImplOpenGL3_RenderDrawData(ImGui::GetDrawData());
//
//		if (io.ConfigFlags & ImGuiConfigFlags_ViewportsEnable)
//		{
//			SDL_Window* backup_current_window = SDL_GL_GetCurrentWindow();
//			SDL_GLContext backup_context = SDL_GL_GetCurrentContext();
//
//			ImGui::UpdatePlatformWindows();
//			ImGui::RenderPlatformWindowsDefault();
//
//			SDL_GL_MakeCurrent(backup_current_window, backup_context);
//		}
//		SDL_GL_SwapWindow(window);
//	}
//
//	// Cleanup
//	ImGui_ImplOpenGL3_Shutdown();
//	ImGui_ImplSDL2_Shutdown();
//	ImGui::DestroyContext();
//
//	SDL_GL_DeleteContext(gl_context);
//	SDL_DestroyWindow(window);
//	SDL_Quit();
//
//	return 0;
//}


#define SDL_MAIN_HANDLED
#include <iostream>
#include "MainWindow.hpp"
#include "ProjectWindow.hpp"


int main(int argc, char* argv[]) {

	MainWindow* mainWindow = new MainWindow;

	mainWindow->AddWindow(std::make_shared<ProjectWindow>(mainWindow));

	mainWindow->Run();

	delete mainWindow;
	return 0;
}
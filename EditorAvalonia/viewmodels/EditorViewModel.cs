using Core.component;
using Core.entity;
using Core.graphics.light;
using Core.graphics.shader;
using Core.IO;
using EditorAvalonia.models.Mesh;
using EditorAvalonia.runtime;
using EditorAvalonia.service;
using EditorAvalonia.utils;
using EditorAvalonia.views.editor;
using EditorAvalonia.views.project;
using EditorAvalonia.views.scene;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EditorAvalonia.viewmodels
{
    public class EditorViewModel : INotifyPropertyChanged
    {
        public event Func<Task>? CloseThisWindow;

        public event Func<Task<string[]?>>? RequestFileDialog;

        public ICommand RunCommand { get; set; }

        public ICommand RunAndroidCommand { get; set; }
        public EditorViewModel()
        {

    
            RunCommand = new RelayCommand(() => Run(), CanOpenProject);
            RunAndroidCommand = new RelayCommand(() => RunAndroid(), CanOpenProject);

        }

        private bool CanOpenProject()
        {
            return true;
        }

        public void RunAndroid()
        {
            RuntimeLauncher runtimeLauncher = new RuntimeLauncher();
            string path = StoreService.GetInstance().ProjectInfo.Path;
            string outputZipPath = Path.Combine(path, "project.zip");
            RuntimeLauncher.Create(path, outputZipPath);
            runtimeLauncher.StartEmulator();
        }

        public void AddPointLight()
        {
            Entity entityLight = new Entity();
            entityLight.Name = "Point Light";
            entityLight.AddComponent(new TransformComponent
            {
                Position = new Vector3(0, 5, 0),
                Scale = new Vector3(1, 1, 1)
            });

            LightComponent lightComponent = new LightComponent
            {
                Type = LightType.Point,
                LightBase = new LightPoint
                {
                    Color = new Vector3(1, 1, 1),
                    Intensity = 1.0f,
                    Range = 10.0f
                }
            };

            entityLight.AddComponent(lightComponent);

            StoreService.GetInstance().AddEntity(entityLight);
        }

        public void CloseWindows()
        {
            StoreService.GetInstance().CloseEditorRender();
            CloseThisWindow?.Invoke();
        }

        public void CloseProject()
        {
            ProjectPage projectPage = new ProjectPage();
            projectPage.Show();
            StoreService.GetInstance().CloseEditorRender();
            CloseThisWindow?.Invoke();
        }

        public void ChangeScene()
        {
            Scene scene = new Scene();
            scene.Show();
            StoreService.GetInstance().CloseEditorRender();
            CloseThisWindow?.Invoke();
        }


        public void AddSpotLight()
        {
            Entity entityLight = new Entity();
            entityLight.Name = "Spot Light";
            entityLight.AddComponent(new TransformComponent
            {
                Position = new Vector3(0, 5, 0),
                Scale = new Vector3(1, 1, 1)
            });

            LightComponent lightComponent = new LightComponent
            {
                Type = LightType.Point,
                LightBase = new LightSpot
                {
                    Color = new Vector3(1, 1, 1),
                    Intensity = 1.0f,
                    Range = 10.0f,
                    Cutoff = 30.0f,
                    Direction = new Vector3(0, -1, 0)
                }
            };


            entityLight.AddComponent(lightComponent);

            StoreService.GetInstance().AddEntity(entityLight);
        }

        public void AddDirectLight()
        {
            Entity entityLight = new Entity();
            entityLight.Name = "Direct Light";
            entityLight.AddComponent(new TransformComponent
            {
                Position = new Vector3(0, 5, 0),
                Scale = new Vector3(1, 1, 1)
            });

            LightComponent lightComponent = new LightComponent
            {
                Type = LightType.Point,
                LightBase = new LightDirectional { Color = new Vector3(1, 1, 1), Intensity = 1.0f, Direction = new Vector3(0, -1, 0) }
            };

            entityLight.AddComponent(lightComponent);

            StoreService.GetInstance().AddEntity(entityLight);
        }
        public void Save()
        {
            var scenePath = Path.Combine(StoreService.GetInstance().ProjectInfo.Path, "scenes", StoreService.GetInstance().Scene.Path);
            SceneIO sceneIO = new SceneIO();
            sceneIO.SaveScene(scenePath, StoreService.GetInstance().Scene);
        }

        public void Run()
        {
            string path = StoreService.GetInstance().ProjectInfo.Path;
            RuntimeLauncher runtimeLauncher = new RuntimeLauncher();
            runtimeLauncher.RunProject(path);
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

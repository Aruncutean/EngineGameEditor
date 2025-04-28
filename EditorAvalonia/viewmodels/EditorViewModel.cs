using Core.component;
using Core.entity;
using Core.graphics.shader;
using Core.IO;
using EditorAvalonia.models.Mesh;
using EditorAvalonia.runtime;
using EditorAvalonia.service;
using EditorAvalonia.utils;
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

        private AssetsService _assetsService { get; set; } = new();

        public event Func<Task<string[]?>>? RequestFileDialog;

        public ICommand AddMeshCommand { get; set; }
        public ICommand RunCommand { get; set; }

        public ICommand RunAndroidCommand { get; set; }
        public EditorViewModel()
        {

            AddMeshCommand = new RelayCommand(async () => await AddMesh(), CanOpenProject);
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
            entityLight.Name = "Light";
            entityLight.AddComponent(new TransformComponent
            {
                Position = new Vector3(0, 5, 0),
                Scale = new Vector3(1, 1, 1)
            });

            LightComponent lightComponent = new LightComponent
            {
                Color = new Vector3(1, 1, 1),
                Intensity = 1.0f
            };


            entityLight.AddComponent(lightComponent);
            entityLight.AddComponent(new ShaderComponent
            {
                shaderType = ShaderTypes.gizmo
            });
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

        public async Task AddMesh()
        {
            if (RequestFileDialog is not null)
            {
                var result = await RequestFileDialog.Invoke();
                if (result != null)
                {
                    LoadEntity loadEntity = new LoadEntity();
                    foreach (string path in result)
                    {
                        _assetsService.SaveMesh(loadEntity.LoadMesh(path));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

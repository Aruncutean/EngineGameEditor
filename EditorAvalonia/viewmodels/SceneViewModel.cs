using Avalonia.Controls;
using Core.IO;
using Core.models;
using Core.scene;
using EditorAvalonia.models;
using EditorAvalonia.service;
using EditorAvalonia.utils;
using EditorAvalonia.views.editor;
using EditorAvalonia.views.project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EditorAvalonia.viewmodels
{
    public class SceneViewModel : INotifyPropertyChanged
    {
        public event Func<Task>? CloseThisWindow;

        public event Func<Task>? ShowMessageBox;

        private SceneService _sceneService = new();

        private string _sceneName = "";
        public string SceneName
        {
            get => _sceneName;
            set
            {
                _sceneName = value;
                OnPropertyChanged(nameof(SceneName));
            }
        }
        public ObservableCollection<SceneInfo> Scenes { get; } = new();

        private SceneInfo? _selectedScene;
        public SceneInfo? SelectedScene
        {
            get => _selectedScene;
            set
            {
                _selectedScene = value;
                OnPropertyChanged(nameof(SelectedScene));
                OnScenSelected();
            }
        }

        private void OnScenSelected()
        {
            StoreService.GetInstance().CurentScene = SelectedScene;

            var editorWindow = new EditorWindow();
            editorWindow.Show();
            CloseThisWindow?.Invoke();
        }

        public ICommand SaveCommand { get; }
        public SceneViewModel()
        {
            SaveCommand = new RelayCommand(SaveScene);


            _sceneService = new SceneService();
            foreach (var scene in _sceneService.Scenes)
            {
                Scenes.Add(scene);
            }
        }

        public void SaveScene()
        {

            var now = DateTime.Now;
            SceneInfo info = new SceneInfo
            {
                Name = SceneName,
                Path = SceneName + "_scene.json",
                CreatedAt = now,
                LastUpdated = now,

            };
            var scenes = _sceneService.Scenes;

            if (scenes != null)
            {
                bool nameExists = scenes.Any(scene => scene.Name.Equals(info.Name, StringComparison.OrdinalIgnoreCase));

                if (nameExists)
                {
                    ShowMessageBox?.Invoke();
                }
                else
                {
                    _sceneService.addSceneInfo(info);
                    Scenes.Add(info);

                    Scene scene = new Scene
                    {
                        Name = info.Name,
                        Path = info.Path,
                        CreatedAt = info.CreatedAt,
                        LastUpdated = info.CreatedAt,
                    };

                    var scenePath = Path.Combine(StoreService.GetInstance().ProjectInfo.Path, "scenes", info.Path);
                    SceneIO sceneIO = new SceneIO();
                    sceneIO.SaveScene(scenePath, scene);
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

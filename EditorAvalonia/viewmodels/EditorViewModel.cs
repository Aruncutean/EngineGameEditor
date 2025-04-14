using EditorAvalonia.models.Mesh;
using EditorAvalonia.runtime;
using EditorAvalonia.service;
using EditorAvalonia.utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        public EditorViewModel()
        {

            AddMeshCommand = new RelayCommand(async () => await AddMesh(), CanOpenProject);
            RunCommand = new RelayCommand(() => Run(), CanOpenProject);

        }

        private bool CanOpenProject()
        {
            return true;
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

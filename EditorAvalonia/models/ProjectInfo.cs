using Core.entity;
using EditorAvalonia.utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EditorAvalonia.models
{
    public class ProjectInfo : INotifyPropertyChanged
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }



        public ICommand DeleteCommand { get; }

        public ProjectInfo()
        {
        }

        public ProjectInfo(ProjectInfo projectInfo, Action<ProjectInfo> onDelete)
        {
            Name = projectInfo.Name;
            Path = projectInfo.Path;
            CreatedAt = projectInfo.CreatedAt;
            LastUpdated = projectInfo.LastUpdated;


            DeleteCommand = new RelayCommand(() => onDelete(this));
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}

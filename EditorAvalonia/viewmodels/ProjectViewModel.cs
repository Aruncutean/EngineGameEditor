﻿using Avalonia.Controls;
using Core.IO;
using DynamicData;
using EditorAvalonia.models;
using EditorAvalonia.service;
using EditorAvalonia.utils;
using EditorAvalonia.views.project;
using EditorAvalonia.views.scene;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EditorAvalonia.viewmodels
{
    public class ProjectViewModel : INotifyPropertyChanged
    {

        public event Func<Task<string?>>? RequestFolderDialog;

        public event Func<Task>? CloseThisWindow;

        private ProjectService _projectService;

        private string _projectName = "";

        public string ProjectName
        {
            get => _projectName;
            set
            {
                _projectName = value;
                OnPropertyChanged(nameof(ProjectName));
            }
        }

        private string _pathProject = "";

        public string PathProject
        {
            get => _pathProject;
            set
            {
                _pathProject = value;
                OnPropertyChanged(nameof(PathProject));
            }
        }

        public ObservableCollection<ProjectInfoE> Projects { get; } = new();

        private ProjectInfoE? _selectedProject;
        public ProjectInfoE? SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                OnPropertyChanged(nameof(SelectedProject));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand GetProjectPathCommand { get; }
        public ProjectViewModel()
        {
            _projectService = new ProjectService();

            SaveCommand = new RelayCommand(SaveProject);
            GetProjectPathCommand = new RelayCommand(async () => await GetProjectPath());
            LoadProjects();
        }

        private void LoadProjects()
        {
            List<ProjectInfoE> projectInfos = _projectService.LoadProjectList();
            Projects.Clear();
            foreach (var projectInfo in projectInfos)
            {
                Projects.Add(new ProjectInfoE(projectInfo, DeleteProject));
            }
        }

        private void DeleteProject(ProjectInfoE projectInfo)
        {
            _projectService.removeProject(projectInfo.Name);
            Projects.Remove(projectInfo);
        }


        public async Task GetProjectPath()
        {
            if (RequestFolderDialog is not null)
            {
                var result = await RequestFolderDialog.Invoke();
                if (!string.IsNullOrEmpty(result))
                {
                    PathProject = result!;
                }
            }
        }

        public void OpenProject()
        {
            StoreService.GetInstance().ProjectInfo = SelectedProject;
            _projectService.loadProjectData(SelectedProject.Path);
            Scene scene = new Scene();
            scene.Show();

            CloseThisWindow();
        }

        public void SaveProject()
        {
            ProjectInfoE projectInfo = _projectService.CreateProject(ProjectName, PathProject);

            StoreService.GetInstance().ProjectInfo = projectInfo;
            _projectService.loadProjectData(projectInfo.Path);

            Scene scene = new Scene();
            scene.Show();

            CloseThisWindow();
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

}

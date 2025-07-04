﻿using Core.IO;
using Core.models;
using Core.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Editor.service
{
    public class ProjectService
    {
        private readonly string ProjectsFile = "projects.json";

        public void loadProjectData(string projectPath)
        {
            var filePath = Path.Combine(projectPath, "project.json");
            ProjectIO projectIO = new ProjectIO();
            DataService.Instance.ProjectData = projectIO.LoadProject(filePath);
        }

        public List<ProjectInfo> LoadProjectList()
        {
            if (!File.Exists(ProjectsFile))
                return new List<ProjectInfo>();

            var json = File.ReadAllText(ProjectsFile);
            return JsonSerializer.Deserialize<List<ProjectInfo>>(json) ?? new List<ProjectInfo>();
        }

        public void SaveProjectList(List<ProjectInfo> projects)
        {
            var json = JsonSerializer.Serialize(projects, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ProjectsFile, json);
        }

        public void removeProject(string name)
        {
            var projects = LoadProjectList();
            var projectToRemove = projects.FirstOrDefault(p => p.Name == name);
            if (projectToRemove != null)
            {
                projects.Remove(projectToRemove);
                SaveProjectList(projects);

            }
        }

        public ProjectInfo CreateProject(string name, string rootPath)
        {
            var projectDir = Path.Combine(rootPath, name);
            Directory.CreateDirectory(projectDir);
            Directory.CreateDirectory(Path.Combine(projectDir, "src"));
            Directory.CreateDirectory(Path.Combine(projectDir, "assets"));
            Directory.CreateDirectory(Path.Combine(projectDir, "scenes"));
            Directory.CreateDirectory(Path.Combine(projectDir, "script"));

            var now = DateTime.Now;
            var projectInfo = new ProjectInfo
            {
                Name = name,
                Path = projectDir,
                CreatedAt = now,
                LastUpdated = now
            };

            var projects = LoadProjectList();
            projects.Add(projectInfo);
            SaveProjectList(projects);

            ScriptService scriptService = new ScriptService();

            var scriptPath = Path.Combine(projectDir, "script");
            scriptService.CreateSlnProject(scriptPath);

            scriptService.AddNewScrips(scriptPath, "GameScript");


            ProjectData projectData = new ProjectData
            {
                Name = name,
                Path = projectDir,
            };


            var filePath = Path.Combine(projectDir, "project.json");
            ProjectIO projectIO = new ProjectIO();
            projectIO.SaveProject(filePath, projectData);

            return projectInfo;
        }
    }
}

using Core.IO;
using Core.models;
using EditorAvalonia.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EditorAvalonia.service
{
    public class ProjectService
    {
        private readonly string ProjectsFile = "projects.json";

        public void loadProjectData(string projectPath)
        {
            var filePath = Path.Combine(projectPath, "project.json");
            ProjectIO projectIO = new ProjectIO();
            StoreService.GetInstance().ProjectData = projectIO.LoadProject(filePath);
        }

        public List<ProjectInfoE> LoadProjectList()
        {
            if (!File.Exists(ProjectsFile))
                return new List<ProjectInfoE>();

            var json = File.ReadAllText(ProjectsFile);
            return JsonSerializer.Deserialize<List<ProjectInfoE>>(json) ?? new List<ProjectInfoE>();
        }

        public void SaveProjectList(List<ProjectInfoE> projects)
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

        public ProjectInfoE CreateProject(string name, string rootPath)
        {
            var projectDir = Path.Combine(rootPath, name);
            Directory.CreateDirectory(projectDir);
            Directory.CreateDirectory(Path.Combine(projectDir, "src"));
            Directory.CreateDirectory(Path.Combine(projectDir, "assets"));
            Directory.CreateDirectory(Path.Combine(projectDir, "scenes"));
            Directory.CreateDirectory(Path.Combine(projectDir, "script"));

            var now = DateTime.Now;
            var projectInfo = new ProjectInfoE
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

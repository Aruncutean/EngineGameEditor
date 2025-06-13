using Core.IO;
using Core.models;
using Core.services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Editor.service
{
    public class SceneService
    {
        private const string ScenesListFile = "project.json";

        public List<SceneInfo> Scenes = new();

        public SceneService()
        {

            Scenes = LoadSceneList();
        }

        public List<SceneInfo> LoadSceneList()
        {
            return DataService.Instance.ProjectData.Scenes;
        }

        public void addSceneInfo(SceneInfo sceneInfo)
        {
            Scenes.Add(sceneInfo);
            DataService.Instance.ProjectData.Scenes = Scenes;

            if (DataService.Instance.ProjectData.MainScene == string.Empty)
            {
                DataService.Instance.ProjectData.MainScene = sceneInfo.Name;
            }

            ProjectIO projectIO = new ProjectIO();
            projectIO.SaveProject(Path.Combine(DataService.Instance.ProjectInfo.Path, ScenesListFile), DataService.Instance.ProjectData);
        }

    }
}

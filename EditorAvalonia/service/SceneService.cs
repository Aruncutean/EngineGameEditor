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
    public class SceneService
    {
        private const string ScenesListFile = "project.json";

        public  List<SceneInfo> Scenes=new();

        public SceneService()
        {

            Scenes = LoadSceneList();
        }

        public List<SceneInfo> LoadSceneList()
        {
            return StoreService.GetInstance().ProjectData.Scenes;
        }

        public void addSceneInfo(SceneInfo sceneInfo)
        {
            Scenes.Add(sceneInfo);
            StoreService.GetInstance().ProjectData.Scenes = Scenes;

            if(StoreService.GetInstance().ProjectData.MainScene == string.Empty)
            {
                StoreService.GetInstance().ProjectData.MainScene = sceneInfo.Name;
            }

            ProjectIO projectIO = new ProjectIO();  
            projectIO.SaveProject(Path.Combine(StoreService.GetInstance().ProjectInfo.Path, ScenesListFile), StoreService.GetInstance().ProjectData);
        }

    }
}

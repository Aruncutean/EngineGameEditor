using Core.models;
using Core.scene;
using EditorAvalonia.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace EditorAvalonia.service
{
    public class StoreService
    {
        private static readonly StoreService storeService = new();

        public ProjectInfo? ProjectInfo { get; set; }
        public ProjectData? ProjectData { get; set; }
        public SceneInfo? CurentScene { get; set; }
        public Scene? Scene { get; set; }

        public AssetCollection? AssetCollection { get; set; }
        public static StoreService GetInstance()
        {
            return storeService;
        }

     
    }
}

using Core.models;
using Core.scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.services
{
    public class DataService
    {
        private static DataService instance = null;
        public ProjectData? ProjectData { get; set; }
        public ProjectInfo? ProjectInfo { get; set; }
        public SceneInfo? CurrentScene { get; set; }

        public World? Scene { get; set; }

        public AssetCollection? AssetCollection;

        private DataService()
        {
        }

        public static DataService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataService();
                }
                return instance;
            }
        }

    }
}

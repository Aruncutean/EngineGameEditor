using Core.entity;
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

        public event Action<Entity>? EntityAdded;
        public event Action<Entity>? EntityRemoved;
        public event Action<Scene>? SceneChanged;
        public event Action<EntityView>? SelectedEntityChanged;
        public event Action<string>? refreshListAction;
        public event Action<MaterialBase>? EditMaterialAction;
        public ProjectInfo? ProjectInfo { get; set; }
        public ProjectData? ProjectData { get; set; }
        public SceneInfo? CurentScene { get; set; }
        public Scene? Scene { get; set; }

        public EntityView? SelectedEntity { get; set; }

        public AssetCollection? AssetCollection { get; set; }
        public static StoreService GetInstance()
        {
            return storeService;
        }

        public void AddEntity(Entity e)
        {
            Scene?.AddEntity(e);
            EntityAdded?.Invoke(e);
        }

        public void RemoveEntity(Entity e)
        {
            Scene?.Entities.Remove(e);
            EntityRemoved?.Invoke(e);
        }

        public void SetScene(Scene scene)
        {
            Scene = scene;
            SceneChanged?.Invoke(scene);
        }

        public void SetSelectedEntity(EntityView entity)
        {
            SelectedEntity = entity;
            SelectedEntityChanged?.Invoke(entity);
        }

        public void EditMaterial(MaterialBase material)
        {
            EditMaterialAction?.Invoke(material);
        }

        public void refreshList(string name)
        {
            refreshListAction?.Invoke(name);
        }

    }
}

using Assimp.Unmanaged;
using CommunityToolkit.Mvvm.Input;
using Core.component;
using Core.entity;
using Core.IO;
using Core.models;
using EditorAvalonia.models;
using EditorAvalonia.service;
using EditorAvalonia.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EditorAvalonia.viewmodels
{
    public class EntityListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<EntityView> Entities { get; set; } = new();

        private EntityView? _selectedEntity;
        public EntityView? SelectedEntity
        {
            get => _selectedEntity;
            set
            {
                _selectedEntity = value;
                OnPropertyChanged(nameof(SelectedEntity));
                OnEntitySelected();
            }
        }
        private void DeleteEntity(EntityView view)
        {
            Entities.Remove(view);
            StoreService.GetInstance().RemoveEntity(view.Model);
        }
        private void OnEntitySelected()
        {
           StoreService.GetInstance().SetSelectedEntity(SelectedEntity);
        }

        public EntityListViewModel()
        {
            StoreService.GetInstance().SceneChanged += (scene) =>
            {
                Entities.Clear();
                scene.Entities.ForEach(e =>
                {
                    Entities.Add(new EntityView(e, DeleteEntity));
                });
            };

            StoreService.GetInstance().EntityAdded += (entity) =>
            {
                EntityView entityView = new EntityView(entity, DeleteEntity);
                Entities.Add(entityView);
            };

            StoreService.GetInstance().refreshListAction += (string name) =>
            {
                   SelectedEntity.Name = name;
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

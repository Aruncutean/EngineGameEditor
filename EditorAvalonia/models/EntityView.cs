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
    public class EntityView : INotifyPropertyChanged
    {
        public Entity Model { get; }

        public string Name
        {
            get => Model.Name;
            set
            {
                if (Model.Name != value)
                {
                    Model.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public ICommand DeleteCommand { get; }

        public EntityView(Entity model, Action<EntityView> onDelete)
        {
            Model = model;

            DeleteCommand = new RelayCommand(() => onDelete(this));
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

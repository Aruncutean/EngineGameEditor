using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Core.component;
using Core.entity;
using Core.models;
using EditorAvalonia.models;
using EditorAvalonia.service;
using EditorAvalonia.views.component;
using System;

namespace EditorAvalonia.views.editor;

public partial class RightPanel : UserControl
{
    public RightPanel()
    {
        InitializeComponent();
        StoreService.GetInstance().SelectedEntityChanged += OnEntitySelected;
        StoreService.GetInstance().EditMaterialAction += OnEditMaterial;
    }

    private void OnEditMaterial(MaterialBase materialBase)
    {
        myPanel.Children.Clear();

        if (materialBase != null)
        {
            var control = ComponentViewGenerator.GenerateFor(materialBase);
            myPanel.Children.Add(control);
        }

    }

    private void OnEntitySelected(EntityView entity)
    {
        myPanel.Children.Clear();
        if (entity != null)
        {
            myPanel.Children.Add(ComponentViewGenerator.GenereteFor(entity));

            foreach (var component in entity.Model.Components)
            {
                if (component.Value is MeshComponent)
                {
                    continue;
                }
                if (component.Value is ShaderComponent && entity.Model.HasComponent<LightComponent>() == true)
                {
                    continue;
                }
                if (component.Value is MaterialComponent && entity.Model.HasComponent<LightComponent>() == true)
                {
                    continue;
                }
                var control = ComponentViewGenerator.GenerateFor(component.Value);
                myPanel.Children.Add(control);
            }
        }

    }
}
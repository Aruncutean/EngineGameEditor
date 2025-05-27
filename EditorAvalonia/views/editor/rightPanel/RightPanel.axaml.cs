using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Core.component;
using Core.entity;
using Core.graphics.material;
using EditorAvalonia.models;
using EditorAvalonia.service;
using EditorAvalonia.views.component;
using System;
using System.ComponentModel;

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
        if (materialBase != null)
        {
            UpdateMaterialUI(materialBase);
        }
    }

    void UpdateMaterialUI(MaterialBase mat)
    {
        myPanel.Children.Clear();
        var control = ComponentViewGenerator.GenerateFor(mat, UpdateMaterialUI);
        myPanel.Children.Add(control);
    }
    private void OnEntitySelected(EntityView entity)
    {
        myPanel.Children.Clear();
        if (entity != null)
        {
            myPanel.Children.Add(ComponentViewGenerator.GenereteFor(entity));

            foreach (var component in entity.Model.Components)
            {
                if (component.Value is LightComponent)
                {
                    var control = ComponentViewGenerator.GenereteForLight(component.Value);
                    myPanel.Children.Add(control);
                }
                else

                if (component.Value is MeshComponent)
                {
                    continue;
                }
                else
                {
                    var control = ComponentViewGenerator.GenerateFor(component.Value);
                    myPanel.Children.Add(control);
                }
            }
        }

    }
}
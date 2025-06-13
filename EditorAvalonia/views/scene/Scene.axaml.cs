using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using EditorAvalonia.models;
using EditorAvalonia.service;
using EditorAvalonia.viewmodels;
using EditorAvalonia.views.editor;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace EditorAvalonia.views.scene;

public partial class Scene : Window
{


    ProjectInfoE projectInfo;
    public Scene()
    {
        InitializeComponent();


        var vm = new SceneViewModel();
        vm.CloseThisWindow += CloseThisWindow;
        vm.ShowMessageBox += ShowMessageBox;
        DataContext = vm;
    }

    private Task CloseThisWindow()
    {
        this.Close();
        return Task.CompletedTask;
    }
    private Task ShowMessageBox()
    {
        var messageBox = MessageBoxManager.GetMessageBoxStandard
        (new MessageBoxStandardParams
        {
            ButtonDefinitions = ButtonEnum.Ok,
            ContentTitle = "Atenție",
            ContentMessage = "Scena există deja!",

        });

        messageBox.ShowWindowDialogAsync(this);
        return Task.CompletedTask;
    }
}
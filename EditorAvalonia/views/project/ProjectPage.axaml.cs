using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;

using EditorAvalonia.models;
using EditorAvalonia.views.project;
using EditorAvalonia.views.scene;

using EditorAvalonia.viewmodels;
using System.Threading.Tasks;

namespace EditorAvalonia.views.project;

public partial class ProjectPage : Window
{

    public ProjectPage()
    {
        InitializeComponent();
        var vm = new ProjectViewModel();
        vm.RequestFolderDialog += ShowFolderDialogAsync;

        vm.CloseThisWindow += CloseThisWindow;
        DataContext = vm;
    }

    private Task CloseThisWindow()
    {
        this.Close();
        return Task.CompletedTask;
    }

    private async Task<string?> ShowFolderDialogAsync()
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Selectează un folder"
        };
        return await dialog.ShowAsync(this);
    }
}

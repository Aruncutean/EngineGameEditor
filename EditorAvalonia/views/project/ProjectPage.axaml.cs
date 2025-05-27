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
    public ProjectViewModel ViewModel;
    public ProjectPage()
    {
        InitializeComponent();
        ViewModel = new ProjectViewModel();
        ViewModel.RequestFolderDialog += ShowFolderDialogAsync;

        ViewModel.CloseThisWindow += CloseThisWindow;
        DataContext = ViewModel;
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
    private void Asset_DoubleTapped(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

        ViewModel.OpenProject();

    }
}

using Avalonia.Controls;
using EditorAvalonia.models;
using System;
using System.Buffers;
using System.Windows.Input;
using System;
using System.Windows.Input;
using Avalonia.Interactivity;
using EditorAvalonia.viewmodels;
using System.Threading.Tasks;

namespace EditorAvalonia.views.editor;

public partial class EditorWindow : Window
{
    public EditorWindow()
    {
        InitializeComponent();
        var vm = new EditorViewModel();
        DataContext = vm;

        vm.RequestFileDialog += ShowFileDialogAsync;

    }
    private async Task<string[]?> ShowFileDialogAsync()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Selectează un folder"
        };
        return await dialog.ShowAsync(this);
    }
}


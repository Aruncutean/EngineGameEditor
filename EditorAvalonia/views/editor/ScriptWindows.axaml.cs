using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace EditorAvalonia;

public partial class ScriptWindows : Window
{
    public event Action SaveScript;
    public string ScriptName => ScriptNameBox.Text ?? string.Empty;
    public ScriptWindows()
    {
        InitializeComponent();
    }

    private void ClickHandler(object? sender, RoutedEventArgs e)
    {

        SaveScript?.Invoke();
    }
}
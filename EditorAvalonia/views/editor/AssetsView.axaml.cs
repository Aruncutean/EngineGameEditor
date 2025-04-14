using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EditorAvalonia.viewmodels;

namespace EditorAvalonia.views.editor;

public partial class AssetsView : UserControl
{
    public AssetsView()
    {
        InitializeComponent();
        var vm = new AssetsViewModel();
        DataContext = vm;
    }
}
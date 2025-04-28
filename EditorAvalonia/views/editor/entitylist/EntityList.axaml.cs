using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EditorAvalonia.viewmodels;

namespace EditorAvalonia.views.editor;

public partial class EntityList : UserControl
{
    public EntityList()
    {
        InitializeComponent();
        var vm = new EntityListViewModel();
        DataContext = vm;
    }
}
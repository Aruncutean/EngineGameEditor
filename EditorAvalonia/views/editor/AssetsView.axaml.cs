using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Core.models;
using EditorAvalonia.models;
using EditorAvalonia.viewmodels;

namespace EditorAvalonia.views.editor;

public partial class AssetsView : UserControl
{

    private AssetsViewModel assetsViewModel;

    public AssetsView()
    {
        InitializeComponent();
        assetsViewModel = new AssetsViewModel();
        DataContext = assetsViewModel;
    }

    private void OnRenameLostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox tb && tb.DataContext is AssetItemView asset)
        {
            asset.IsRenaming = false;
        }
    }

    private void Asset_DoubleTapped(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is AssetItemView asset)
        {
            if (asset.Model.Type == AssetType.Folder)
            {
                assetsViewModel.OpenFolder();
            }
            if (asset.Model.Type == AssetType.Mesh)
            {
                assetsViewModel.OnAssetSelected();
            }
            if (asset.Model.Type == AssetType.Material)
            {
                assetsViewModel.OnMeshSelected();
            }
        }
    }
}

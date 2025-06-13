using Dock.Model.ReactiveUI.Controls;
using Dock.Model.ReactiveUI.Factories; // Add this using directive

namespace AvaloniaApplication4
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var layout = new RootDock
            {
                Title = "Root",
                DefaultDockable = new DocumentDock
                {
                    ActiveDockable = new Dock.Model.ReactiveUI.Controls.Document
                    {
                        Title = "My Document",
                    }
                }
            };

            var factory = new DockFactory(); // DockFactory is part of Dock.Model.ReactiveUI.Factories
            var dock = factory.CreateLayout(layout);
            DockControl.Layout = dock;
        }
    }
}

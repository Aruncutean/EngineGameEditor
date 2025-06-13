using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorAvalonia.views.component
{
    public class ComponentView
    {
        public static Control CreateNumericUpDown(string axis, float value, Action<float> onChange)
        {
            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("Auto, *"),
                Margin = new Thickness(0, 5, 5, 5)
            };

            var label = new TextBlock
            {
                Text = axis,
                FontSize = 12,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 8, 0)
            };
            Grid.SetColumn(label, 0);

            var upDown = new NumericUpDown
            {
                Value = (decimal?)value,
                FormatString = "0.00",
                Increment = 0.1m,
                FontSize = 12,
                Background = new SolidColorBrush(Color.Parse("#2A2A2A")),
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                MinWidth = 120
            };
            Grid.SetColumn(upDown, 1);

            upDown.PropertyChanged += (_, e) =>
            {
                if (e.Property == NumericUpDown.ValueProperty)
                {
                    onChange((float)(upDown.Value ?? 0));
                }
            };

            grid.Children.Add(label);
            grid.Children.Add(upDown);

            return grid;
        }
    }
}

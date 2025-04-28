using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Avalonia.Layout;
using Core.component;
using Core.entity;
using EditorAvalonia.service;
using EditorAvalonia.models;
using Silk.NET.Input;
using System.Text.Json.Serialization;
using Core.attributes;
using System.ComponentModel;
using Core.models;

namespace EditorAvalonia.views.component
{
    public class ComponentViewGenerator
    {
        public static Control GenereteFor(EntityView entity)
        {
            var expander = new Expander
            {
                Header = "Base",
                IsExpanded = true,
                Focusable = true,
                Margin = new Thickness(5),
            };

            var stack = new StackPanel();
            expander.Content = stack;
            expander.HorizontalAlignment = HorizontalAlignment.Stretch;
            if (entity != null)
            {
                stack.Children.Add(CreateTextBox("Name", entity.Name, newVal =>
            {
                if (entity.Name != newVal)
                {
                    entity.Name = newVal;
                    StoreService.GetInstance().refreshList(newVal);
                }
            }));
            }

            return expander;
        }
        public static Control GenerateFor(MaterialBase material)
        {

            var expander = new Expander
            {
                Header = "Material",
                IsExpanded = true,
                Focusable = true,
                Margin = new Thickness(5),
            };


            var stack = new StackPanel();
            expander.Content = stack;
            expander.HorizontalAlignment = HorizontalAlignment.Stretch;

            var props = material.GetType().GetProperties();

            foreach (var prop in props)
            {
                var label = new TextBlock { Text = prop.Name, Margin = new Thickness(4) };
                stack.Children.Add(label);

                var value = prop.GetValue(material);
                if (prop.PropertyType.IsEnum)
                {

                    var enumType = prop.PropertyType;

                    var values = Enum.GetValues(enumType)
                        .Cast<object>()
                        .Where(v =>
                        {
                            var field = enumType.GetField(v.ToString());
                            var attr = field?.GetCustomAttribute<BrowsableAttribute>();
                            return attr?.Browsable ?? true;
                        })
                        .ToList();


                    var comboBox = new ComboBox
                    {
                        ItemsSource = values,
                        SelectedItem = value,
                        Margin = new Thickness(2),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                    };

                    comboBox.SelectionChanged += (_, _) =>
                    {
                       
                    };

                    stack.Children.Add(comboBox);
                }
                else
                if (value is float f)
                {
                    var box = CreateFloatInput(f, val => prop.SetValue(material, val));
                    stack.Children.Add(box);
                }
                else if (value is Vector3 vec)
                {
                    stack.Children.Add(CreateVector3Input(vec, val => prop.SetValue(material, val)));
                }
                else if (value is string s)
                {
                    var tb = new TextBox { Text = s, Width = 200 };
                    tb.LostFocus += (_, _) => prop.SetValue(material, tb.Text);
                    stack.Children.Add(tb);
                }
            }

            return expander;
        }

        private static Control CreateFloatInput(float value, Action<float> onChange)
        {
            var upDown = new NumericUpDown
            {
                Value = (decimal)value,
                Width = 100,
                Increment = 0.1m,
            };
            upDown.PropertyChanged += (_, e) =>
            {
                if (e.Property == NumericUpDown.ValueProperty && upDown.Value.HasValue)
                    onChange((float)upDown.Value.Value);
            };
            return upDown;
        }

        private static Control CreateVector3Input(Vector3 v, Action<Vector3> onChange)
        {
            var row = new StackPanel { Orientation = Orientation.Horizontal };
            Vector3 temp = v;

            row.Children.Add(CreateFloatInput(v.X, x => { temp.X = x; onChange(temp); }));
            row.Children.Add(CreateFloatInput(v.Y, y => { temp.Y = y; onChange(temp); }));
            row.Children.Add(CreateFloatInput(v.Z, z => { temp.Z = z; onChange(temp); }));

            return row;
        }

        public static Control GenerateFor(Core.component.IComponent component)
        {
            var expander = new Expander
            {
                Header = component.GetType().Name,
                IsExpanded = true,
                Focusable = true,
                Margin = new Thickness(5),
            };

            var stack = new StackPanel();
            expander.Content = stack;
            expander.HorizontalAlignment = HorizontalAlignment.Stretch;
            var props = component.GetType()
       .GetProperties(BindingFlags.Public | BindingFlags.Instance)
       .Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() == null)
       .Where(p => !p.GetGetMethod()?.IsStatic ?? true)
       .ToArray();

            foreach (var prop in props)
            {
                var label = new TextBlock
                {
                    FontSize = 12,
                    Text = prop.Name
                };
                stack.Children.Add(label);

                var value = prop.GetValue(component);

                if (value is Vector3 vec3)
                {
                    var row = new StackPanel { Orientation = Orientation.Vertical };
                    row.Children.Add(CreateNumericUpDown("X", vec3.X, newVal =>
                    {
                        var current = (Vector3)prop.GetValue(component)!;
                        prop.SetValue(component, current with { X = newVal });
                    }));
                    row.Children.Add(CreateNumericUpDown("Y", vec3.Y, newVal =>
                    {
                        var current = (Vector3)prop.GetValue(component)!;
                        prop.SetValue(component, current with { Y = newVal });
                    }));
                    row.Children.Add(CreateNumericUpDown("Z", vec3.Z, newVal =>
                    {
                        var current = (Vector3)prop.GetValue(component)!;
                        prop.SetValue(component, current with { Z = newVal });
                    }));
                    stack.Children.Add(row);
                }
                else if (value is float f)
                {
                    stack.Children.Add(CreateFloatBox(prop.Name, f, v => prop.SetValue(component, v)));
                }
                else if (value is bool b)
                {
                    var cb = new CheckBox
                    {
                        IsChecked = b,
                        Content = prop.Name
                    };
                    cb.Checked += (_, _) => prop.SetValue(component, true);
                    cb.Unchecked += (_, _) => prop.SetValue(component, false);
                    stack.Children.Add(cb);
                }
                else if (prop.PropertyType.IsEnum)
                {

                    var enumType = prop.PropertyType;

                    var values = Enum.GetValues(enumType)
                        .Cast<object>()
                        .Where(v =>
                        {
                            var field = enumType.GetField(v.ToString());
                            var attr = field?.GetCustomAttribute<BrowsableAttribute>();
                            return attr?.Browsable ?? true;
                        })
                        .ToList();


                    var comboBox = new ComboBox
                    {
                        ItemsSource = values,
                        SelectedItem = value,
                        Margin = new Thickness(2),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                    };

                    comboBox.SelectionChanged += (_, _) =>
                    {
                        prop.SetValue(component, comboBox.SelectedItem);
                    };

                    stack.Children.Add(comboBox);
                }
            }

            return expander;
        }

        private static Control CreateTextBox(string name, string value, Action<string> onChange)
        {
            var row = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("50, *"), // 100px pentru label
                Margin = new Thickness(2, 2)
            };

            var label = new TextBlock
            {
                Text = name,
                FontSize = 12,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 8, 0)
            };
            Grid.SetColumn(label, 0);

            var tb = new TextBox
            {
                Text = value,
                FontSize = 12,
                Background = new SolidColorBrush(Color.Parse("#1E1E1E")),
                Foreground = Brushes.White,
                BorderBrush = Brushes.Gray,
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(6, 2),
                BorderThickness = new Thickness(1),
                VerticalContentAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(tb, 1);

            tb.LostFocus += (_, _) =>
            {
                onChange(tb.Text);
            };

            tb.KeyDown += (sender, e) =>
            {
                if (e.Key == Avalonia.Input.Key.Enter)
                {
                    onChange(tb.Text);
                    e.Handled = true;
                }
            };
            row.Children.Add(label);
            row.Children.Add(tb);

            return row;

        }

        private static Control CreateFloatBox(string name, float value, Action<float> onChange)
        {
            var tb = new TextBox
            {
                Text = value.ToString("0.00"),
                Width = 60,
                Margin = new Thickness(2),
                VerticalContentAlignment = VerticalAlignment.Center
            };

            tb.PropertyChanged += (_, _) =>
            {
                if (float.TryParse(tb.Text, out var f))
                {
                    onChange(f);
                }
            };

            return tb;
        }


        private static Control CreateNumericUpDown(string axis, float value, Action<float> onChange)
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
                MinWidth = 120 // sau mai mare dacă vrei și mai lung
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

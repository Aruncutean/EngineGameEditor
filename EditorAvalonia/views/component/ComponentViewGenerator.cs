using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using System;
using System.Linq;
using System.Numerics;
using System.Reflection;

using Avalonia.Layout;
using EditorAvalonia.service;
using EditorAvalonia.models;

using System.Text.Json.Serialization;
using System.ComponentModel;
using Core.models;
using Core.graphics.shader;
using Core.IO;
using System.IO;
using Core.services;
using Core.graphics.material;
using Avalonia.Data;


namespace EditorAvalonia.views.component
{
    public class ComponentViewGenerator
    {
        public static Control GenereteForLight(Core.component.IComponent component)
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

                if (prop.Name == "MaterialID")
                {
                    DataService dataService = DataService.Instance;

                    if (dataService != null && dataService.AssetCollection != null && dataService.AssetCollection.Assets != null)
                    {

                        var defaultMaterial = new AssetItem
                        {
                            Id = "default",
                            Name = "default",
                            Type = AssetType.Material,
                            Path = string.Empty,
                            BaseDirector = string.Empty,
                            CreatedAt = DateTime.UtcNow
                        };

                        if (value != null)
                        {
                            var selected = dataService.AssetCollection.Assets
                                .FirstOrDefault(a => a.Id == value.ToString());

                            if (selected == null && value == "default")
                            {
                                selected = defaultMaterial;
                            }

                            if (selected != null)
                            {
                                value = selected;
                            }
                        }

                        var values = dataService.AssetCollection.Assets
                            .Where(a => a.Type == AssetType.Material)
                            .ToList();

                        values.Insert(0, defaultMaterial);

                        var comboBox = new ComboBox
                        {
                            ItemsSource = values,
                            SelectedItem = value,
                            Margin = new Thickness(2),
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                        };

                        comboBox.SelectionChanged += (_, _) =>
                        {
                            var selectedAsset = comboBox.SelectedItem as AssetItem;
                            prop.SetValue(component, selectedAsset.Id);
                        };

                        stack.Children.Add(comboBox);
                    }


                }
                else if (value is Vector3 vec3)
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
        public static Control GenerateFor(MaterialBase material, Action<MaterialBase> onMaterialChanged)
        {

            var materialIsLoad = MaterialManager.Get(material.Id);

            var expander = new Expander
            {
                Header = "Material",
                IsExpanded = true,
                Focusable = true,
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            var stack = new StackPanel();
            expander.Content = stack;

            void SaveMaterial()
            {
                MaterialIO materialIO = new MaterialIO();
                materialIO.Save(Path.Combine(StoreService.GetInstance().ProjectInfo.Path, material.Path), material);
            }

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
                        if (comboBox.SelectedItem is ShaderTypes shaderType)
                        {
                            MaterialBase newMaterial = shaderType switch
                            {
                                ShaderTypes.Phong => new MaterialPhong(),
                                ShaderTypes.PBR => new MaterialPBR(),
                                ShaderTypes.Basic => new MaterialDefault(),
                                _ => material
                            };

                            onMaterialChanged?.Invoke(newMaterial);
                        }
                    };

                    stack.Children.Add(comboBox);
                }
                else if (value is float f)
                {
                    stack.Children.Add(CreateFloatBox(prop.Name, f, v =>
                    {
                        prop.SetValue(material, v);
                        if (materialIsLoad != null)
                        {
                            prop.SetValue(materialIsLoad, v);
                        }
                        SaveMaterial();
                    }));
                }
                else if (value is Vector3 vec3)
                {
                    var row = new StackPanel { Orientation = Orientation.Vertical };
                    row.Children.Add(CreateNumericUpDown("X", vec3.X, newVal =>
                    {
                        var current = (Vector3)prop.GetValue(material)!;
                        prop.SetValue(material, current with { X = newVal });
                        if (materialIsLoad != null)
                        {
                            prop.SetValue(materialIsLoad, current with { X = newVal });
                        }
                        SaveMaterial();
                    }));

                    row.Children.Add(CreateNumericUpDown("Y", vec3.Y, newVal =>
                    {
                        var current = (Vector3)prop.GetValue(material)!;
                        prop.SetValue(material, current with { Y = newVal });
                        if (materialIsLoad != null)
                        {
                            prop.SetValue(materialIsLoad, current with { Y = newVal });
                        }
                        SaveMaterial();
                    }));

                    row.Children.Add(CreateNumericUpDown("Z", vec3.Z, newVal =>
                    {
                        var current = (Vector3)prop.GetValue(material)!;
                        prop.SetValue(material, current with { Z = newVal });
                        if (materialIsLoad != null)
                        {
                            prop.SetValue(materialIsLoad, current with { Z = newVal });
                        }
                        SaveMaterial();
                    }));
                    stack.Children.Add(row);
                }
                else if (value is string s)
                {
                    var tb = new TextBox { Text = s, Width = 200 };
                    tb.LostFocus += (_, _) => prop.SetValue(material, tb.Text);
                    stack.Children.Add(tb);
                }
                else if (value is ColorOrTexture colorOrTexture)
                {
                    var stackColorOrTexture = new StackPanel();

                    var colorPicker = new ColorPicker
                    {
                        Color = colorOrTexture.Color != null ? Color.Parse(colorOrTexture.Color) : Color.Parse("#ffffff"),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Margin = new Thickness(4)
                    };


                    colorPicker.PropertyChanged += (_, args) =>
                    {
                        if (args.Property == ColorPicker.ColorProperty)
                        {
                            var newColor = colorPicker.Color;
                            string hex = $"#{newColor.R:X2}{newColor.G:X2}{newColor.B:X2}";
                            colorOrTexture.Color = hex;

                            if (materialIsLoad != null)
                            {
                                prop.SetValue(materialIsLoad, colorOrTexture);
                            }
                            SaveMaterial();
                        }
                    };


                    DataService dataService = DataService.Instance;
                    if (colorOrTexture.TexturePath != null)
                    {

                        var selected = dataService.AssetCollection.Assets
                            .FirstOrDefault(a => a.Id == colorOrTexture.TexturePath);

                        if (selected != null)
                        {
                            value = selected;
                        }
                    }

                    var values = dataService.AssetCollection.Assets
                         .Where(a => a.Type == AssetType.Texture)
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
                        var selectedAsset = comboBox.SelectedItem as AssetItem;
                        colorOrTexture.TexturePath = selectedAsset.Id;
                        SaveMaterial();
                    };

                    var check = new CheckBox
                    {
                        IsChecked = colorOrTexture.IsTexture,
                        Content = "Is texture",
                        Margin = new Thickness(4)
                    };

                    check.Checked += (_, _) =>
                    {
                        colorOrTexture.IsTexture = true;
                        stackColorOrTexture.Children.Remove(colorPicker);
                        stackColorOrTexture.Children.Add(comboBox);
                        SaveMaterial();
                    };

                    check.Unchecked += (_, _) =>
                    {
                        stackColorOrTexture.Children.Add(colorPicker);
                        stackColorOrTexture.Children.Remove(comboBox);
                        colorOrTexture.IsTexture = false;
                        SaveMaterial();
                    };

                    stackColorOrTexture.Children.Add(check);

                    if (colorOrTexture.IsTexture == false)
                    {
                        stackColorOrTexture.Children.Add(colorPicker);
                    }
                    else
                    {
                        stackColorOrTexture.Children.Add(comboBox);
                    }

                    stack.Children.Add(stackColorOrTexture);


                }
            }

            return expander;
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

                if (prop.Name == "MaterialID")
                {
                    DataService dataService = DataService.Instance;

                    if (dataService != null && dataService.AssetCollection != null && dataService.AssetCollection.Assets != null)
                    {

                        var defaultMaterial = new AssetItem
                        {
                            Id = "default",
                            Name = "default",
                            Type = AssetType.Material,
                            Path = string.Empty,
                            BaseDirector = string.Empty,
                            CreatedAt = DateTime.UtcNow
                        };

                        if (value != null)
                        {
                            var selected = dataService.AssetCollection.Assets
                                .FirstOrDefault(a => a.Id == value.ToString());

                            if (selected == null && value == "default")
                            {
                                selected = defaultMaterial;
                            }

                            if (selected != null)
                            {
                                value = selected;
                            }
                        }

                        var values = dataService.AssetCollection.Assets
                            .Where(a => a.Type == AssetType.Material)
                            .ToList();

                        values.Insert(0, defaultMaterial);

                        var comboBox = new ComboBox
                        {
                            ItemsSource = values,
                            SelectedItem = value,
                            Margin = new Thickness(2),
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                        };

                        comboBox.SelectionChanged += (_, _) =>
                        {
                            var selectedAsset = comboBox.SelectedItem as AssetItem;
                            prop.SetValue(component, selectedAsset.Id);
                        };

                        stack.Children.Add(comboBox);
                    }


                }
                else if (value is Vector3 vec3)
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
                ColumnDefinitions = new ColumnDefinitions("50, *"),
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
                FontSize = 12,
                Background = new SolidColorBrush(Color.Parse("#1E1E1E")),
                Foreground = Brushes.White,
                BorderBrush = Brushes.Gray,
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(6, 2),
                BorderThickness = new Thickness(1),
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

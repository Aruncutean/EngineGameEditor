using Core.graphics.material;
using Core.IO;
using Core.models;
using Core.services;
using Editor.component;
using Editor.entity;
using EditorAvalonia.service;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.OpenGL;
using Core.component;
using Core.entity;

namespace Editor.windows.editorWindow
{
    public class AssetsView : IView
    {
        private nint[] icons = new nint[5];
        List<bool> isRenaming = new();
        List<string> renameBuffer = new();
        AssetsService _assetsService = new AssetsService();

        private string CurrentFolder = "assets";

        public GL? gl = null;
        public AssetsView(GL? gl)
        {
            this.gl = gl;
        }

        public void Init()
        {
            List<AssetItem> assetItems = DataService.Instance.AssetCollection.Assets;
            isRenaming = assetItems.Select(a => false).ToList();
            renameBuffer = assetItems.Select(a => a.Name).ToList();
            LoadIcons();
        }

        public void Run(float deltaTime)
        {
            bool isOpen = true;
            UI.Window("Assets", ref isOpen, () =>
            {
                RenderBreadcrumb(CurrentFolder, (newPath) =>
                {
                    CurrentFolder = newPath.Replace('\\', '/'); ;
                });

                List<AssetItem> assetItems = DataService.Instance.AssetCollection.Assets.Where(a => a.BaseDirector == CurrentFolder)
                .OrderByDescending(a => a.Type == AssetType.Folder)
                .ThenBy(a => a.Name)
                .ToList();

                if (ImGui.BeginPopupContextWindow("window_context", ImGuiPopupFlags.MouseButtonRight))
                {
                    if (ImGui.MenuItem("AddFolder"))
                    {
                        _assetsService.AddFolder("NewFolder", CurrentFolder);
                        assetItems = DataService.Instance.AssetCollection.Assets.Where(a => a.BaseDirector == CurrentFolder)
                .OrderByDescending(a => a.Type == AssetType.Folder)
                .ThenBy(a => a.Name)
                .ToList();
                        isRenaming = assetItems.Select(a => false).ToList();
                        renameBuffer = assetItems.Select(a => a.Name).ToList();
                    }
                    if (ImGui.MenuItem("AddTexture"))
                    {
                        UI.OpenFileDialog("AddTexture", "Image Files (*.png;*.jpg;*.jpeg)\0*.png;*.jpg;*.jpeg\0", (filePath) =>
                        {
                            if (!string.IsNullOrEmpty(filePath[0]))
                            {
                                _assetsService.SaveTexture(filePath[0], CurrentFolder);
                                assetItems = DataService.Instance.AssetCollection.Assets.Where(a => a.BaseDirector == CurrentFolder)
            .OrderByDescending(a => a.Type == AssetType.Folder)
            .ThenBy(a => a.Name)
            .ToList();
                                isRenaming = assetItems.Select(a => false).ToList();
                                renameBuffer = assetItems.Select(a => a.Name).ToList();
                            }
                        });
                    }
                    if (ImGui.MenuItem("AddMesh"))
                    {
                        UI.OpenFileDialog("AddMesh", "Mesh Files (*.obj;*.fbx)\0*.obj;*.fbx\0", (filePath) =>
                        {
                            if (!string.IsNullOrEmpty(filePath[0]))
                            {
                                LoadEntity loadEntity = new LoadEntity();
                                _assetsService.SaveMesh(loadEntity.LoadMesh(filePath[0]), CurrentFolder);

                                assetItems = DataService.Instance.AssetCollection.Assets.Where(a => a.BaseDirector == CurrentFolder)
            .OrderByDescending(a => a.Type == AssetType.Folder)
            .ThenBy(a => a.Name)
            .ToList();
                                isRenaming = assetItems.Select(a => false).ToList();
                                renameBuffer = assetItems.Select(a => a.Name).ToList();
                            }
                        });
                    }
                    if (ImGui.MenuItem("AddScript"))
                    {
                        _assetsService.AddScript(CurrentFolder);
                        isRenaming = assetItems.Select(a => false).ToList();
                        renameBuffer = assetItems.Select(a => a.Name).ToList();
                    }
                    if (ImGui.MenuItem("AddMaterial"))
                    {
                        _assetsService.AddMaterial(CurrentFolder);

                        assetItems = DataService.Instance.AssetCollection.Assets.Where(a => a.BaseDirector == CurrentFolder)
            .OrderByDescending(a => a.Type == AssetType.Folder)
            .ThenBy(a => a.Name)
            .ToList();
                        isRenaming = assetItems.Select(a => false).ToList();
                        renameBuffer = assetItems.Select(a => a.Name).ToList();
                    }

                    ImGui.EndPopup();
                }

                Vector2 iconSize = new Vector2(64, 64);
                float padding = 16f;
                float cellWidth = iconSize.X + padding;

                float availableWidth = ImGui.GetContentRegionAvail().X;
                int columns = Math.Max(1, (int)(availableWidth / cellWidth));


                for (int i = 0; i < assetItems.Count; i++)
                {
                    bool isDeleting = false;
                    ImGui.PushID(i);
                    ImGui.BeginGroup();

                    if (assetItems[i].Type == AssetType.Texture)
                    {
                        if (ImGui.ImageButton($"icon_{i}", icons[1], iconSize))
                        {
                            Console.WriteLine($"Icon {i} clicked!");
                        }
                    }
                    else if (assetItems[i].Type == AssetType.Script)
                    {
                        if (ImGui.ImageButton($"icon_{i}", icons[2], iconSize))
                        {
                            Console.WriteLine($"Icon {i} clicked!");
                        }
                    }
                    else if (assetItems[i].Type == AssetType.Folder)
                    {
                        if (ImGui.ImageButton($"icon_{i}", icons[3], iconSize))
                        {
                            CurrentFolder = CurrentFolder + "/" + assetItems[i].Name;
                        }
                    }
                    else if (assetItems[i].Type == AssetType.Mesh)
                    {
                        if (ImGui.ImageButton($"icon_{i}", icons[0], iconSize))
                        {
                            var shaderComponent = new MaterialComponent();
                            shaderComponent.MaterialID = "default";

                            var transformComponent = new TransformComponent();
                            transformComponent.Position = new Vector3(0, 0, 0);
                            transformComponent.Scale = new Vector3(1, 1, 1);

                            var meshComponent = new MeshComponent();
                            meshComponent.MeshPath = assetItems[i].Path;

                            Entity entity = new Entity();
                            entity.AddComponent(shaderComponent);
                            entity.AddComponent(meshComponent);
                            entity.AddComponent(transformComponent);

                            DataService.Instance.Scene.AddEntity(entity);

                        }
                    }
                    else if (assetItems[i].Type == AssetType.Material)
                    {
                        if (ImGui.ImageButton($"icon_{i}", icons[4], iconSize))
                        {
                            Console.WriteLine($"Icon {i} clicked!");
                        }
                    }

                    if (ImGui.BeginPopupContextItem("context1"))
                    {
                        if (ImGui.MenuItem("Rename"))
                        {
                            isRenaming[i] = true;
                            renameBuffer[i] = assetItems[i].Name;
                        }

                        if (ImGui.MenuItem("Delete"))
                        {
                            isDeleting = true;

                        }

                        ImGui.EndPopup();
                    }

                    if (isRenaming[i])
                    {
                        ImGui.PushItemWidth(iconSize.X);
                        string renameBufferValue = renameBuffer[i];
                        if (ImGui.InputText($"##rename_{i}", ref renameBufferValue, 128,
                               ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.AutoSelectAll))
                        {
                            assetItems[i].Name = renameBufferValue;
                            isRenaming[i] = false;

                            var assetsService = new AssetsService();
                            assetsService.SaveAssets();
                        }

                        if (isRenaming[i] && ImGui.IsItemHovered() && !ImGui.IsAnyItemActive())
                        {
                            ImGui.SetKeyboardFocusHere(-1);
                        }


                        if (!(ImGui.IsItemActive() || ImGui.IsItemHovered() || !ImGui.IsKeyPressed(ImGuiKey.Enter)))
                        {
                            isRenaming[i] = false;

                        }

                        ImGui.PopItemWidth();
                    }
                    else
                    {
                        float maxLabelWidth = iconSize.X;
                        string label = assetItems[i].Name;


                        if (label.Length > 10)
                        {
                            label = label.Substring(0, 10);
                        }

                        float textWidth = ImGui.CalcTextSize(label).X;
                        float labelOffset = (iconSize.X - textWidth) / 2.0f;
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Math.Max(labelOffset, 0));
                        ImGui.Text(label);
                    }

                    ImGui.EndGroup();

                    if ((i + 1) % columns != 0)
                        ImGui.SameLine();

                    ImGui.PopID();

                    if (isDeleting)
                    {
                        DataService.Instance.AssetCollection.Assets.Remove(assetItems[i]);
                        isRenaming.RemoveAt(i);
                        renameBuffer.RemoveAt(i);

                        var assetsService = new AssetsService();
                        assetsService.SaveAssets();
                    }
                }
            });
        }
        public void RenderBreadcrumb(string currentPath, Action<string> onNavigate)
        {
            string[] parts = currentPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string pathAccum = "";

            for (int i = 0; i < parts.Length; i++)
            {
                if (i > 0)
                    ImGui.SameLine();

                pathAccum = Path.Combine(pathAccum, parts[i]);

                if (ImGui.SmallButton(parts[i]))
                {
                    onNavigate(pathAccum);
                }

                if (i < parts.Length - 1)
                {
                    ImGui.SameLine();
                    ImGui.Text(">");
                    ImGui.SameLine();
                }
            }
        }
        private void LoadIcons()
        {
            icons[0] = UI.LoadTextureFromFile(gl, "assets/geometry.png");
            icons[1] = UI.LoadTextureFromFile(gl, "assets/image.png");
            icons[2] = UI.LoadTextureFromFile(gl, "assets/script.png");
            icons[3] = UI.LoadTextureFromFile(gl, "assets/folder.png");
            icons[4] = UI.LoadTextureFromFile(gl, "assets/material.png");
        }

    }
}

using ImGuiNET;
using NativeFileDialogNET;
using Silk.NET.OpenGL;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.component
{
    internal class UI
    {
        public static void Text(string text) => ImGui.Text(text);

        public static bool Input(string label, ref string value, int bufferSize = 256, bool enabled = true)
        {
            byte[] buffer = new byte[bufferSize];
            Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, 0);
            if (!enabled)
                ImGui.BeginDisabled();

            bool changed = ImGui.InputText("##" + label, buffer, (uint)buffer.Length);

            if (!enabled)
                ImGui.EndDisabled();


            if (changed)
            {
                value = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
            }
            return changed;
        }

        public static bool Checkbox(string label, ref bool value) => ImGui.Checkbox(label, ref value);

        public static void Button(string label, Action onClick)
        {
            if (ImGui.Button(label))
            {
                onClick?.Invoke();
            }
        }
        public static void Window(string title, ref bool isOpen, Action content)
        {
            if (!isOpen)
                return;

            if (ImGui.Begin(title, ref isOpen))
            {
                content?.Invoke();
            }

            ImGui.End();
        }

        public static void OpenFileDialog(string title, string filter, Action<string[]> onFilesSelected)
        {
            using var dlg = new NativeFileDialog()
                .SelectFile()
                .AddFilter(title, filter)
                .AllowMultiple();

            var result = dlg.Open(out string[]? files, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

            if (result == DialogResult.Okay && files != null)
            {
                onFilesSelected?.Invoke(files);
            }
        }

        public static void OpenFolderDialog(string title, Action<string[]> onFolderSelected)
        {
            using var dlg = new NativeFileDialog()
                .SelectFolder();

            var result = dlg.Open(out string[]? folder, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

            if (result == DialogResult.Okay && folder != null)
            {
                onFolderSelected?.Invoke(folder);
            }



        }

        public static IntPtr LoadTextureFromFile(GL gl, string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Texture file not found at path: {path}");
            }

            using var stream = File.OpenRead(path);
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            uint tex = gl.GenTexture();
            gl.BindTexture(TextureTarget.Texture2D, tex);

            unsafe
            {
                fixed (byte* dataPtr = image.Data)
                {
                    gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba,
                        (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba,
                        PixelType.UnsignedByte, dataPtr);
                }
            }

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);

            gl.GenerateMipmap(TextureTarget.Texture2D);

            gl.BindTexture(TextureTarget.Texture2D, 0);

            return (IntPtr)tex;
        }
        public static void ListBoxWithEvents(
      string label,
      string[] items,
      ref int selectedIndex,
      Action<int, string>? onSelect = null,
      Action<int, string>? onDoubleClick = null,
      Action<int, string>? contextMenuBuilder = null,
      int heightInItems = 4)
        {
            if (ImGui.BeginListBox("##" + label, new System.Numerics.Vector2(0, ImGui.GetTextLineHeightWithSpacing() * heightInItems)))
            {
                for (int i = 0; i < items.Length; i++)
                {
                    bool isSelected = selectedIndex == i;

                    if (ImGui.Selectable(items[i], isSelected))
                    {
                        if (!isSelected)
                        {
                            selectedIndex = i;
                            onSelect?.Invoke(i, items[i]);
                        }
                    }

                    if (ImGui.IsItemHovered())
                    {
                        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                        {
                            onDoubleClick?.Invoke(i, items[i]);
                        }

                        if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                        {
                            ImGui.OpenPopup($"##context_{label}_{i}");
                        }
                    }

                    if (ImGui.BeginPopup($"##context_{label}_{i}"))
                    {
                        contextMenuBuilder?.Invoke(i, items[i]);
                        ImGui.EndPopup();
                    }

                    if (isSelected)
                        ImGui.SetItemDefaultFocus();
                }

                ImGui.EndListBox();
            }
        }
    }
}

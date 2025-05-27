using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Window = Silk.NET.Windowing.Window;
using Avalonia.Platform;
using Silk.NET.Maths;
using Buffer = System.Buffer;
using PixelFormat = Avalonia.Platform.PixelFormat;
using Core.graphics.shader;
using Core.component;
using Core.entity;
using Core.system;
using System.Numerics;
using EditorAvalonia.service;
using Silk.NET.Input;
using Avalonia.Input;
using Core.IO;
using Core.graphics.framebuffer;
using Core.services;

namespace EditorAvalonia.views.editor
{
    public class OpenGLViewport : Image
    {

        private IWindow? _window;
        private GL? _gl;
        private WriteableBitmap? _bitmap;

        private int width = 512;
        private int height = 512;


        private IInputContext _input;

        private WorldManager _worldSystem;
        public OpenGLViewport()
        {
            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
            this.PointerMoved += OnPointerMoved;
            this.PointerPressed += OnMouseClick;
            this.PointerReleased += OnMouseUp;
            Loaded += async (_, _) => await InitGLAsync();
        }
        private void CheckForResize()
        {
            var newWidth = (int)Bounds.Width;
            var newHeight = (int)Bounds.Height;

            if (newWidth <= 0 || newHeight <= 0) return;

            if (newWidth != width || newHeight != height)
            {
                width = newWidth;
                height = newHeight;

                if (_window != null)
                {
                    _window.Size = new Vector2D<int>(width, height);
                    WindowsService.Instance.Width = width;
                    WindowsService.Instance.Height = height;
                    _worldSystem.Resize(width, height);
                }
            }
        }

        private async Task InitGLAsync()
        {
            var options = WindowOptions.Default with
            {
                API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.Default, new APIVersion(3, 3)),
                IsVisible = false,
                Title = "OpenGL Viewport",
                Size = new Vector2D<int>(width, height)
            };

            _window = Window.Create(options);
            _window.Load += () =>
            {
                var storeService = StoreService.GetInstance();
                if (storeService.ProjectData != null)
                {
                    _input = _window.CreateInput();
                    _gl = GL.GetApi(_window);

                    var scenePath = Path.Combine(StoreService.GetInstance().ProjectInfo.Path, "scenes", StoreService.GetInstance().CurentScene.Path);
                    var json = File.ReadAllText(Path.Combine(StoreService.GetInstance().ProjectInfo.Path, scenePath));

                    DataService.Instance.ProjectData = storeService.ProjectData;

                    SceneIO sceneIO = new SceneIO();
                    StoreService.GetInstance().SetScene(sceneIO.LoadScene(scenePath));

                    _worldSystem = new WorldManager(storeService.ProjectData);
                    _worldSystem.isEditMode = true;
                    _worldSystem.renderInFrameBuffer = true;
                    _worldSystem.Init(_gl);

                    if (StoreService.GetInstance().Scene != null)
                    {
                        _worldSystem.LoadWorld(StoreService.GetInstance().Scene);
                    }
                }
            };

            _window.Update += _ =>
            {
                CheckForResize();
                if (_worldSystem != null)
                {
                    _worldSystem.Update((float)_);
                }
            };

            _window.Render += _ =>
            {
                _worldSystem.Render((float)_);


                _gl?.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                _gl?.ClearColor(0.247f, 0.247f, 0.247f, 1.0f);

                _gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _worldSystem._frameBuffer.msFBO);
                _gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _worldSystem._frameBuffer._framebuffer);

                _gl.BlitFramebuffer(
                    0, 0, width, height,
                    0, 0, width, height,
                    ClearBufferMask.ColorBufferBit,
                    BlitFramebufferFilter.Linear
                );

                _gl.BindFramebuffer(FramebufferTarget.Framebuffer, _worldSystem._frameBuffer._framebuffer);

                byte[] pixels = new byte[width * height * 4];
                _gl.ReadPixels(0, 0, (uint)width, (uint)height,
                    Silk.NET.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, pixels.AsSpan());

                _gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _bitmap = new WriteableBitmap(
                        new PixelSize(width, height),
                        new Avalonia.Vector(96, 96),
                        PixelFormat.Rgba8888,
                        AlphaFormat.Unpremul
                    );

                    using var fb = _bitmap.Lock();
                    FlipImageVertically(pixels, width, height);
                    Marshal.Copy(pixels, 0, fb.Address, pixels.Length);

                    Source = _bitmap;
                });
            };

            await Task.Run(() => _window.Run());
        }


        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            this.Focus();
            _worldSystem._cameraControllerSystem?.OnKeyDown(MapKey(e.Key));
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            _worldSystem._cameraControllerSystem?.OnKeyUp(MapKey(e.Key));
        }

        private void OnMouseUp(object? sender, PointerReleasedEventArgs e)
        {

            bool mouseButtonPressed = e.GetCurrentPoint(this).Properties.IsLeftButtonPressed;

            _worldSystem._cameraControllerSystem?.mousePresss(mouseButtonPressed);
        }
        private void OnMouseClick(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            this.Focus();
            var pos = e.GetPosition(this);

            bool mouseButtonPressed = e.GetCurrentPoint(this).Properties.IsLeftButtonPressed;

            _worldSystem._cameraControllerSystem?.mousePresss(mouseButtonPressed);
        }

        public Silk.NET.Input.Key MapKey(Avalonia.Input.Key key)
        {
            return key switch
            {
                Avalonia.Input.Key.W => Silk.NET.Input.Key.W,
                Avalonia.Input.Key.A => Silk.NET.Input.Key.A,
                Avalonia.Input.Key.S => Silk.NET.Input.Key.S,
                Avalonia.Input.Key.D => Silk.NET.Input.Key.D,
                Avalonia.Input.Key.Space => Silk.NET.Input.Key.Space,
                Avalonia.Input.Key.LeftShift => Silk.NET.Input.Key.ShiftLeft,
                _ => Silk.NET.Input.Key.Unknown
            };
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            var pos = e.GetPosition(this);
            _worldSystem._cameraControllerSystem?.OnMouseMove(new Vector2((float)pos.X, (float)pos.Y));
        }

        private static void FlipImageVertically(byte[] pixels, int width, int height)
        {
            int stride = width * 4;
            byte[] tempRow = new byte[stride];

            for (int y = 0; y < height / 2; y++)
            {
                int topIndex = y * stride;
                int bottomIndex = (height - y - 1) * stride;

                int expectedSize = width * height * 4;
                if (pixels.Length == expectedSize)
                {
                    Buffer.BlockCopy(pixels, topIndex, tempRow, 0, stride);
                    Buffer.BlockCopy(pixels, bottomIndex, pixels, topIndex, stride);
                    Buffer.BlockCopy(tempRow, 0, pixels, bottomIndex, stride);
                }
            }
        }

    }
}

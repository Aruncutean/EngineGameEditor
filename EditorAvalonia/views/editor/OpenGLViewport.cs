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
using Core.scene;
using Core.system;
using System.Numerics;
using EditorAvalonia.service;
using System.Text.Json;
using EditorAvalonia.models;
using System.Collections.Generic;
using Assimp;
using Silk.NET.Input;
using Avalonia.Input;
using Core.IO;
using MsBox.Avalonia;

namespace EditorAvalonia.views.editor
{
    public class OpenGLViewport : Image
    {

        private IWindow? _window;
        private GL? _gl;
        private WriteableBitmap? _bitmap;
        private static uint _vao, _vbo, _shaderProgram;

        private uint _framebuffer;
        private uint _texture;
        private bool _initialized = false;
        private int width = 512;
        private int height = 512;
        private bool _framebufferReady = false;

        private Core.scene.Scene _scene;
        private static RenderSystem _renderer;
        private static EditorGizmoSystem _editorGizmoSystem;
        private static CameraControllerSystem _cameraControllerSystem;
        private IInputContext _input;
        private Entity? _cameraEntity;
        private uint msFBO;
        private uint msColorTex;
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

                if (_initialized)
                {
                    if (_window != null)
                    {
                        _window.Size = new Vector2D<int>(width, height);
                        SetupFramebuffer();
                        _renderer.screenWidth = width;
                        _renderer.screenHeight = height;
                        _editorGizmoSystem.screenWidth = width;
                        _editorGizmoSystem.screenHeight = height;
                    }

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
                _input = _window.CreateInput();

                _gl = GL.GetApi(_window);
                ShaderManager.Init(_gl);

                ShaderManager.Load(ShaderTypes.Basic, "shader/basic.vert.glsl", "shader/basic.frag.glsl");
                ShaderManager.Load(ShaderTypes.gizmo, "shader/gizmo.vert.glsl", "shader/gizmo.frag.glsl");
                ShaderManager.Load(ShaderTypes.Phong, "shader/phong.vert.glsl", "shader/phong.frag.glsl");

                var scenePath = Path.Combine(StoreService.GetInstance().ProjectInfo.Path, "scenes", StoreService.GetInstance().CurentScene.Path);
                var json = File.ReadAllText(Path.Combine(StoreService.GetInstance().ProjectInfo.Path, scenePath));

                SceneIO sceneIO = new SceneIO();

                StoreService.GetInstance().SetScene(sceneIO.LoadScene(scenePath));
                _scene = StoreService.GetInstance().Scene;

                if (_scene == null)
                    throw new Exception("Scene is null");

                _cameraEntity = new Entity();
                _cameraEntity.AddComponent(new TransformComponent
                {
                    Position = new Vector3(0, 0, 5),

                });
                _cameraEntity.AddComponent(new CameraComponent { IsMainCamera = true });
                _cameraEntity.AddComponent(new CameraControllerComponent { MoveSpeed = 5f });

                _renderer = new RenderSystem();
                _renderer.cameraEntity = _cameraEntity;

                _editorGizmoSystem = new EditorGizmoSystem(_gl);
                _editorGizmoSystem.cameraEntity = _cameraEntity;

                _cameraControllerSystem = new CameraControllerSystem(_cameraEntity);

                _gl.Enable(GLEnum.DepthTest);
                _gl.Enable(GLEnum.Multisample);
                _gl.Enable(GLEnum.Blend);
                _gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);

            };

            _window.Update += _ =>
            {
                CheckForResize();
                if (_cameraControllerSystem != null)
                {
                    if (_scene != null)
                    {
                        _cameraControllerSystem.Update((float)_);
                    }
                }
            };

            _window.Render += _ =>
            {
                _gl.BindFramebuffer(FramebufferTarget.Framebuffer, msFBO);
                _gl.Viewport(0, 0, (uint)width, (uint)height);
                _gl?.ClearColor(0.247f, 0.247f, 0.247f, 1.0f);
                _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


                if (!_initialized)
                {
                    SetupFramebuffer();
                    _initialized = true;
                }

                if (_scene != null)
                {
                    _editorGizmoSystem.Render(_scene);
                    _renderer.Render(_scene, _gl);

                }

                _gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _framebuffer);

                _gl.BlitFramebuffer(
                    0, 0, width, height,
                    0, 0, width, height,
                    ClearBufferMask.ColorBufferBit,
                    BlitFramebufferFilter.Linear
                );

                _gl.BindFramebuffer(FramebufferTarget.Framebuffer, _framebuffer);

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
                    Source = _bitmap;
                });
            };

            await Task.Run(() => _window.Run());
        }

        private void SetupFramebuffer()
        {
            unsafe
            {
                if (_gl == null)
                    throw new InvalidOperationException("GL is not initialized");

                // 🟡 Multisample framebuffer (anti-aliasing)
                msFBO = _gl.GenFramebuffer();
                _gl.BindFramebuffer(FramebufferTarget.Framebuffer, msFBO);

                // Multisample color buffer
                msColorTex = _gl.GenTexture();
                _gl.BindTexture(TextureTarget.Texture2DMultisample, msColorTex);
                _gl.TexImage2DMultisample(TextureTarget.Texture2DMultisample, 8, InternalFormat.Rgba8, (uint)width, (uint)height, true);
                _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                    TextureTarget.Texture2DMultisample, msColorTex, 0);

                // Multisample depth buffer
                uint msDepthRBO = _gl.GenRenderbuffer();
                _gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, msDepthRBO);
                _gl.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, 8, InternalFormat.DepthComponent24, (uint)width, (uint)height);
                _gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                    RenderbufferTarget.Renderbuffer, msDepthRBO);

                // Check if framebuffer is complete
                if (_gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
                    throw new Exception("Multisample framebuffer not complete!");

                // 🔵 Normal framebuffer (rezultat final în textură)
                _framebuffer = _gl.GenFramebuffer();
                _gl.BindFramebuffer(FramebufferTarget.Framebuffer, _framebuffer);

                _texture = _gl.GenTexture();
                _gl.BindTexture(TextureTarget.Texture2D, _texture);
                _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)width, (uint)height, 0,
                    Silk.NET.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, null);
                _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
                _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

                _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                    TextureTarget.Texture2D, _texture, 0);

                // Depth buffer pentru cel final (opțional)
                uint depthRBO = _gl.GenRenderbuffer();
                _gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthRBO);
                _gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent24, (uint)width, (uint)height);
                _gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                    RenderbufferTarget.Renderbuffer, depthRBO);

                if (_gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
                    throw new Exception("Final framebuffer not complete!");

                // ✔ Store it if needed
                _framebufferReady = true;

                // Setează default la final
                _gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                // 🔁 Salvează MSFBO & textura multisample în câmpuri dacă vrei să le folosești în render loop
            }

        }
        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            this.Focus(); // ensure we keep focus
            _cameraControllerSystem?.OnKeyDown(MapKey(e.Key));
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            _cameraControllerSystem?.OnKeyUp(MapKey(e.Key));
        }

        private void OnMouseUp(object? sender, PointerReleasedEventArgs e)
        {

            bool mouseButtonPressed = e.GetCurrentPoint(this).Properties.IsLeftButtonPressed;

            _cameraControllerSystem?.mousePresss(mouseButtonPressed);
        }
        private void OnMouseClick(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            this.Focus();
            var pos = e.GetPosition(this);

            bool mouseButtonPressed = e.GetCurrentPoint(this).Properties.IsLeftButtonPressed;

            _cameraControllerSystem?.mousePresss(mouseButtonPressed);
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
            _cameraControllerSystem?.OnMouseMove(new Vector2((float)pos.X, (float)pos.Y));
        }
        private void DeleteFramebuffer()
        {
            if (_gl == null)
                return;

            if (_texture != 0)
            {
                _gl.DeleteTexture(_texture);
                _texture = 0;
            }

            if (_framebuffer != 0)
            {
                _gl.DeleteFramebuffer(_framebuffer);
                _framebuffer = 0;
            }
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

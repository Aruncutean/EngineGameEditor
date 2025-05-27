using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.graphics.framebuffer
{
    public class FrameBuffer
    {
        private GL _gl;
        public uint _framebuffer;
        private uint _texture;
        private int width = 512;
        private int height = 512;
        private bool _framebufferReady = false;

        public uint msFBO;
        private uint msColorTex;

        public FrameBuffer(GL gl)
        {
            this._gl = gl;
        }

        public void resize(int width, int height)
        {
            this.width = width;
            this.height = height;
            init();
        }

        public void init()
        {

            unsafe
            {
                if (_gl == null)
                    throw new InvalidOperationException("GL is not initialized");

                // Multisample framebuffer (anti-aliasing)
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

                // Normal framebuffer (rezultat final în textură)
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

                _framebufferReady = true;


                _gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
        }
        public void render(Action renderAction)
        {
            if (!_framebufferReady)
                init();
            _gl.BindFramebuffer(FramebufferTarget.Framebuffer, msFBO);
            _gl.Viewport(0, 0, (uint)width, (uint)height);
            _gl?.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _gl?.ClearColor(0.247f, 0.247f, 0.247f, 1.0f);


            renderAction.Invoke();

            _gl?.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        public void delete()
        {
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

    }
}

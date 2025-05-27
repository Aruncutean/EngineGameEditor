using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.graphics.material.texture
{
    public class Texture
    {
        public uint LoadTexture(GL gl, string path)
        {
            using var image = Image.Load<Rgba32>(path);
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            var pixels = new byte[image.Width * image.Height * 4];
            image.CopyPixelDataTo(pixels);

            uint texture = gl.GenTexture();
            gl.BindTexture(TextureTarget.Texture2D, texture);

            unsafe
            {
                fixed (void* data = pixels)
                {
                    gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba,
                        (uint)image.Width, (uint)image.Height, 0,
                        PixelFormat.Rgba, PixelType.UnsignedByte, data);
                }
            }

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            gl.BindTexture(TextureTarget.Texture2D, 0);
            return texture;
        }

    }
}

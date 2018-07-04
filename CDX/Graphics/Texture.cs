//#define USE_BITMAP
#define USE_STB

using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

#if USE_STB
using StbSharp;
#endif

#if USE_BITMAP
using System.Drawing;
using System.Drawing.Imaging;
#endif


namespace CDX.Graphics
{
    public class Texture : GLTexture
    {
        internal int width;
        internal int height;

        public static Texture loadFromFile(string path)
        {
#if USE_STB
            var buffer = File.ReadAllBytes(path);
            var image  = StbImage.LoadFromMemory(buffer, StbImage.STBI_rgb_alpha);

            var tex = new Texture(TextureTarget.Texture2D, GL.GenTexture());
            tex.width  = image.Width;
            tex.height = image.Height;
            tex.setData(image.Data, image.Width, image.Height);
#elif USE_BITMAP
            var bmp = new Bitmap(path);
            var tex = new Texture(TextureTarget.Texture2D, GL.GenTexture());
            tex.width = bmp.Width; 
            tex.height = bmp.Height; 
            tex.setData(bmp);
#else
            throw new Exception("Not supported")
#endif


            return tex;
        }

        public Texture(TextureTarget glTarget) : base(glTarget)
        {
        }

        public Texture(TextureTarget glTarget, int glHandle) : base(glTarget, glHandle)
        {
        }

        private void setData(byte[] data, int w, int h)
        {
            bind();

            // todo: handle mipmaps

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.TexImage2D(glTarget, 0, PixelInternalFormat.Rgba, w, h, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);


            unsafeSetFilter(minFilter, magFilter, true);
            unsafeSetWrap(uWrap, vWrap, true);

            GL.BindTexture(glTarget, 0);
        }

#if USE_BITMAP
        private void setData(Bitmap bitmap)
        {
            bind();

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);


            unsafeSetFilter(minFilter, magFilter, true);
            unsafeSetWrap(uWrap, vWrap, true);

            bitmap.UnlockBits(data);


            GL.BindTexture(glTarget, 0);
        }
#endif

        public override int getWidth()
        {
            return width;
        }

        public override int getHeight()
        {
            return height;
        }

        public override int getDepth()
        {
            throw new NotImplementedException();
        }

        public override bool isManaged()
        {
            throw new NotImplementedException();
        }

        protected override void reload()
        {
            throw new NotImplementedException();
        }
    }
}
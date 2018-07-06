using System;
using OpenTK;

namespace CDX.Graphics
{
    public static class BatchHelper
    {
        public const int X1 = 0;
        public const int Y1 = 1;
        public const int C1 = 2;
        public const int U1 = 3;
        public const int V1 = 4;
        public const int X2 = 5;
        public const int Y2 = 6;
        public const int C2 = 7;
        public const int U2 = 8;
        public const int V2 = 9;
        public const int X3 = 10;
        public const int Y3 = 11;
        public const int C3 = 12;
        public const int U3 = 13;
        public const int V3 = 14;
        public const int X4 = 15;
        public const int Y4 = 16;
        public const int C4 = 17;
        public const int U4 = 18;
        public const int V4 = 19;
    }

    public interface IBatch : IDisposable
    {
        void begin();

        void end();

        void setColor(Color tint);

        void setColor(float r, float g, float b, float a);

        void setColor(float color);

        Color getColor();

        float getPackedColor();

        void draw(Texture texture, float x, float y, float originX, float originY, float width, float height, float scaleX,
            float scaleY, float rotation, int srcX, int srcY, int srcWidth, int srcHeight, bool flipX, bool flipY);

        void draw(Texture texture, float x, float y, float width, float height, int srcX, int srcY, int srcWidth,
            int srcHeight, bool flipX, bool flipY);

        void draw(Texture texture, float x, float y, int srcX, int srcY, int srcWidth, int srcHeight);

        void draw(Texture texture, float x, float y, float width, float height, float u, float v, float u2, float v2);

        void draw(Texture texture, float x, float y);

        void draw(Texture texture, float x, float y, float width, float height);

        void draw(Texture texture, float[] spriteVertices, int offset, int count);

        void draw(TextureRegion region, float x, float y);

        void draw(TextureRegion region, float x, float y, float width, float height);

        void draw(TextureRegion region, float x, float y, float originX, float originY, float width, float height,
            float scaleX, float scaleY, float rotation);

        void draw(TextureRegion region, float x, float y, float originX, float originY, float width, float height,
            float scaleX, float scaleY, float rotation, bool clockwise);

        void flush();

        void disableBlending();

        void enableBlending();

        void setBlendFunction(int srcFunc, int dstFunc);

        void setBlendFunctionSeparate(int srcFuncColor, int dstFuncColor, int srcFuncAlpha, int dstFuncAlpha);

        int getBlendSrcFunc();

        int getBlendDstFunc();

        int getBlendSrcFuncAlpha();

        int getBlendDstFuncAlpha();

        Matrix4 getProjectionMatrix();

        Matrix4 getTransformMatrix();

        void setProjectionMatrix(Matrix4 projection);

        void setTransformMatrix(Matrix4 transform);

        void setShader(ShaderProgram shader);

        ShaderProgram getShader();

        bool isBlendingEnabled();

        bool isDrawing();
    }
}
using System;
using CDX.Graphics;
using CDX.Graphics.GLUtils;

namespace CDX
{
    public enum GraphicsType
    {
        GLFW
    }

    public class DisplayMode
    {
        public readonly int width;
        public readonly int height;
        public readonly int refreshRate;
        public readonly int bitsPerPixel;

        protected DisplayMode(int width, int height, int refreshRate, int bitsPerPixel)
        {
            this.width        = width;
            this.height       = height;
            this.refreshRate  = refreshRate;
            this.bitsPerPixel = bitsPerPixel;
        }

        public override string ToString()
        {
            return width + "x" + height + ", bpp: " + bitsPerPixel + ", hz: " + refreshRate;
        }
    }

    public class Monitor
    {
        public readonly int    virtualX;
        public readonly int    virtualY;
        public readonly string name;

        protected Monitor(int virtualX, int virtualY, string name)
        {
            this.virtualX = virtualX;
            this.virtualY = virtualY;
            this.name     = name;
        }
    }

    public class BufferFormat
    {
        public readonly int r, g, b, a;

        public readonly int depth, stencil;

        public readonly int samples;

        public readonly bool coverageSampling;

        public BufferFormat(int r, int g, int b, int a, int depth, int stencil, int samples, bool coverageSampling)
        {
            this.r                = r;
            this.g                = g;
            this.b                = b;
            this.a                = a;
            this.depth            = depth;
            this.stencil          = stencil;
            this.samples          = samples;
            this.coverageSampling = coverageSampling;
        }

        public override string ToString()
        {
            return "r: " + r + ", g: " + g + ", b: " + b + ", a: " + a + ", depth: " + depth + ", stencil: " + stencil
                   + ", num samples: " + samples + ", coverage sampling: " + coverageSampling;
        }
    }

    public interface IGraphics
    {
        int getWidth();

        int getHeight();

        int getBackBufferWidth();

        int getBackBufferHeight();

        long getFrameId();

        float getDeltaTime();

        float getRawDeltaTime();

        int getFramesPerSecond();

        GraphicsType getType();

        float getPpiX();

        float getPpiY();

        float getPpcX();

        float getPpcY();

        float getDensity();

        bool supportsDisplayModeChange();

        Monitor getPrimaryMonitor();

        Monitor getMonitor();

        Monitor[] getMonitors();

        DisplayMode[] getDisplayModes();

        DisplayMode[] getDisplayModes(Monitor monitor);

        DisplayMode getDisplayMode();

        DisplayMode getDisplayMode(Monitor monitor);

        bool setFullscreenMode(DisplayMode displayMode);

        bool setWindowedMode(int width, int height);

        void setTitle(string title);

        void setUndecorated(bool undecorated);

        void setResizable(bool resizable);

        void setVSync(bool vsync);

        BufferFormat getBufferFormat();

        bool supportsExtension(string extension);

        void setContinuousRendering(bool isContinuous);

        bool isContinuousRendering();

        void requestRendering();

        bool isFullscreen();
    }
}
using System;
using CDX.Graphics;

namespace CDX.GLFWBackend
{
	public class WindowConfiguration
    {
        internal int                  windowX         = -1;
        internal int                  windowY         = -1;
        internal int                  windowWidth     = 640;
        internal int                  windowHeight    = 480;
        internal int                  windowMinWidth  = -1;
        internal int windowMinHeight = -1;
        internal int windowMaxWidth = -1;
        internal int windowMaxHeight = -1;
        internal bool                 windowResizable = true;
        internal bool                 windowDecorated = true;
        internal bool                 windowMaximized = false;
        internal FileType             windowIconFileType;
        internal string               windowIconPaths;
        internal IWindowListener windowListener;
        internal DisplayMode    fullscreenMode;
        internal string               title                  = "";
        internal Color                initialBackgroundColor = Color.BLACK;
        protected internal bool                 initialVisible         = true;

        protected void setWindowConfiguration(WindowConfiguration config)
        {
            windowX                = config.windowX;
            windowY                = config.windowY;
            windowWidth            = config.windowWidth;
            windowHeight           = config.windowHeight;
            windowMinWidth         = config.windowMinWidth;
            windowMinHeight        = config.windowMinHeight;
            windowMaxWidth         = config.windowMaxWidth;
            windowMaxHeight        = config.windowMaxHeight;
            windowResizable        = config.windowResizable;
            windowDecorated        = config.windowDecorated;
            windowMaximized        = config.windowMaximized;
            windowIconFileType     = config.windowIconFileType;
            windowIconPaths        = config.windowIconPaths;
            windowListener         = config.windowListener;
            fullscreenMode         = config.fullscreenMode;
            title                  = config.title;
            initialBackgroundColor = config.initialBackgroundColor;
            initialVisible         = config.initialVisible;
        }

        public void setInitialVisible(bool visibility)
        {
            initialVisible = visibility;
        }

        public void setWindowedMode(int width, int height)
        {
            windowWidth  = width;
            windowHeight = height;
        }

        public void setResizable(bool resizable)
        {
            windowResizable = resizable;
        }

        public void setDecorated(bool decorated)
        {
            windowDecorated = decorated;
        }

        public void setMaximized(bool maximized)
        {
            windowMaximized = maximized;
        }

        public void setWindowPosition(int x, int y)
        {
            windowX = x;
            windowY = y;
        }

        public void setWindowSizeLimits(int minWidth, int minHeight, int maxWidth, int maxHeight)
        {
            windowMinWidth  = minWidth;
            windowMinHeight = minHeight;
            windowMaxWidth  = maxWidth;
            windowMaxHeight = maxHeight;
        }

        public void setWindowIcon(string filePaths)
        {
            setWindowIcon(FileType.Internal, filePaths);
        }

        public void setWindowIcon(FileType fileType, string filePaths)
        {
            windowIconFileType = fileType;
            windowIconPaths    = filePaths;
        }

        public void setWindowListener(IWindowListener windowListener)
        {
            this.windowListener = windowListener;
        }

        public void setFullscreenMode(CDX.DisplayMode mode)
        {
            fullscreenMode = (DisplayMode) mode;
        }

        public void setTitle(string title)
        {
            this.title = title;
        }

        public void setInitialBackgroundColor(Color color)
        {
            initialBackgroundColor = color;
        }
    }
}
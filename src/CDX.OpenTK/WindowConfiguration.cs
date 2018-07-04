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
        internal String               windowIconPaths;
        internal IWindowListener windowListener;
        internal DisplayMode    fullscreenMode;
        internal String               title                  = "";
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

        /**
         * @param visibility whether the window will be visible on creation. (default true)
         */
        public void setInitialVisible(bool visibility)
        {
            initialVisible = visibility;
        }

        /**
         * Sets the app to use windowed mode.
         * 
         * @param width
         *            the width of the window (default 640)
         * @param height
         *            the height of the window (default 480)
         */
        public void setWindowedMode(int width, int height)
        {
            windowWidth  = width;
            windowHeight = height;
        }

        /** 
         * @param resizable whether the windowed mode window is resizable (default true)
         */
        public void setResizable(bool resizable)
        {
            windowResizable = resizable;
        }

        /**
         * @param decorated whether the windowed mode window is decorated, i.e. displaying the title bars (default true)
         */
        public void setDecorated(bool decorated)
        {
            windowDecorated = decorated;
        }

        /**
         * @param maximized whether the window starts maximized. Ignored if the window is full screen. (default false)
         */
        public void setMaximized(bool maximized)
        {
            windowMaximized = maximized;
        }

        /**
         * Sets the position of the window in windowed mode on the
         * primary monitor. Default -1 for both coordinates for centered.
         */
        public void setWindowPosition(int x, int y)
        {
            windowX = x;
            windowY = y;
        }

        /**
         * Sets minimum and maximum size limits for the window. If the window is full screen or not resizable, these 
         * limits are ignored. The default for all four parameters is -1, which means unrestricted.
         */
        public void setWindowSizeLimits(int minWidth, int minHeight, int maxWidth, int maxHeight)
        {
            windowMinWidth  = minWidth;
            windowMinHeight = minHeight;
            windowMaxWidth  = maxWidth;
            windowMaxHeight = maxHeight;
        }

        /**
         * Sets the icon that will be used in the window's title bar. Has no effect in macOS, which doesn't use window icons.
         * @param filePaths One or more {@linkplain FileType#Internal internal} image paths. Must be JPEG, PNG, or BMP format.
         * The one closest to the system's desired size will be scaled. Good sizes include 16x16, 32x32 and 48x48.
         */
        public void setWindowIcon(String filePaths)
        {
            setWindowIcon(FileType.Internal, filePaths);
        }

        /**
         * Sets the icon that will be used in the window's title bar. Has no effect in macOS, which doesn't use window icons.
         * @param fileType The type of file handle the paths are relative to.
         * @param filePaths One or more image paths, relative to the given {@linkplain FileType}. Must be JPEG, PNG, or BMP format. 
         * The one closest to the system's desired size will be scaled. Good sizes include 16x16, 32x32 and 48x48.
         */
        public void setWindowIcon(FileType fileType, String filePaths)
        {
            windowIconFileType = fileType;
            windowIconPaths    = filePaths;
        }

        /**
         * Sets the {@link Lwjgl3WindowListener} which will be informed about
         * iconficiation, focus loss and window close events.
         */
        public void setWindowListener(IWindowListener windowListener)
        {
            this.windowListener = windowListener;
        }

        /**
         * Sets the app to use fullscreen mode. Use the static methods like
         * {@link #getDisplayMode()} on this class to enumerate connected monitors
         * and their fullscreen display modes.
         */
        public void setFullscreenMode(CDX.DisplayMode mode)
        {
            fullscreenMode = (DisplayMode) mode;
        }

        /**
         * Sets the window title. Defaults to empty string.
         */
        public void setTitle(String title)
        {
            this.title = title;
        }

        /**
         * Sets the initial background color. Defaults to black.
         */
        public void setInitialBackgroundColor(Color color)
        {
            initialBackgroundColor = color;
        }
    }
}
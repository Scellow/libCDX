using System;
using System.Collections.Generic;

using GLFWWindow = GLFW.GLFW.Window;
using GLFWImage = GLFW.GLFW.Image;

namespace CDX.GLFWBackend
{
    public class Window : IDisposable
    {
        private          GLFWWindow               windowHandle;
        private readonly IApplicationListener           listener;
        private          bool                           listenerInitialized = false;
        private          IWindowListener           windowListener;
        private          Graphics                 graphics;
        private          Input                    input;
        private readonly ApplicationConfiguration config;
        private readonly List<Runnable>                 runnables         = new List<Runnable>();
        private readonly List<Runnable>                 executedRunnables = new List<Runnable>();
        private          bool                           iconified        = false;
        internal         bool                           requestRendering = false;


        private GLFW.GLFW.WindowFocusFunc _focusFunc;
        private GLFW.GLFW.WindowIconifyFunc _iconifyFunc;
        private GLFW.GLFW.WindowCloseFunc _closeFunc;
        private GLFW.GLFW.DropFunc _dropFunc;
        private GLFW.GLFW.WindowRefreshFunc _refreshFunc;

        internal Window(IApplicationListener listener, ApplicationConfiguration config)
        {
            this.listener       = listener;
            windowListener = config.windowListener;
            this.config         = config;
        }

        internal void create(GLFWWindow windowHandle)
        {
            this.windowHandle = windowHandle;
            input        = new Input(this);
            graphics     = new Graphics(this);

            
            // that's to aboid callbacks from getting garbage collected
            _focusFunc   = focusCallback;
            _iconifyFunc = iconifyCallback;
            _closeFunc   = closeCallback;
            _dropFunc    = dropCallback;
            _refreshFunc = refreshCallback;
            

            GLFW.GLFW.SetWindowFocusCallback(windowHandle, _focusFunc);
            GLFW.GLFW.SetWindowIconifyCallback(windowHandle, _iconifyFunc);
            // todo: missing binding
            //GLFW.GLFW.SetWindowMaximizeCallback(windowHandle, maximizeCallback);
            GLFW.GLFW.SetWindowCloseCallback(windowHandle, _closeFunc);
            GLFW.GLFW.SetDropCallback(windowHandle, _dropFunc);
            GLFW.GLFW.SetWindowRefreshCallback(windowHandle, _refreshFunc);
            
            if (windowListener != null)
            {
                windowListener.created(this);
            }
        }

        private void refreshCallback(GLFWWindow window)
        {
            postRunnable((() => { windowListener?.refreshRequested(); }));
        }

        private void dropCallback(GLFWWindow window, int count, string[] paths)
        {
            postRunnable((() => { windowListener?.filesDropped(paths); }));
        }

        private void closeCallback(GLFWWindow window)
        {
            postRunnable((() =>
            {
                if (windowListener != null)
                {
                    if (!windowListener.closeRequested())
                        GLFW.GLFW.SetWindowShouldClose(window, false);
                }
            }));
        }

        private void iconifyCallback(GLFWWindow window, bool focused)
        {
            postRunnable((() =>
            {
                windowListener?.iconified(focused);
                iconified = focused;
                if (focused) listener.pause();
                else listener.resume();
            }));
        }

        private void focusCallback(GLFWWindow window, bool focused)
        {
            postRunnable((() =>
            {
                if (windowListener != null)
                {
                    if (focused)
                    {
                        windowListener.focusGained();
                    }
                    else
                    {
                        windowListener.focusLost();
                    }
                }
            }));
        }


        public IApplicationListener getListener()
        {
            return listener;
        }

        public IWindowListener getWindowListener()
        {
            return windowListener;
        }

        public void setWindowListener(IWindowListener listener)
        {
            windowListener = listener;
        }

        public void postRunnable(Runnable runnable)
        {
            lock (runnables)
            {
                runnables.Add(runnable);
            }
        }

        public void setPosition(int x, int y)
        {
            GLFW.GLFW.SetWindowPos(windowHandle, x, y);
        }

        public int getPositionX()
        {
            int x;
            int y;
            GLFW.GLFW.GetWindowPos(windowHandle, out x, out y);
            return x;
        }

        public int getPositionY()
        {
            int x;
            int y;
            GLFW.GLFW.GetWindowPos(windowHandle, out x, out y);
            return y;
        }

        public void setVisible(bool visible)
        {
            if (visible)
            {
                GLFW.GLFW.ShowWindow(windowHandle);
            }
            else
            {
                GLFW.GLFW.HideWindow(windowHandle);
            }
        }

        public void closeWindow()
        {
            GLFW.GLFW.SetWindowShouldClose(windowHandle, true);
        }

        public void iconifyWindow()
        {
            GLFW.GLFW.IconifyWindow(windowHandle);
        }

        public void restoreWindow()
        {
            GLFW.GLFW.RestoreWindow(windowHandle);
        }

        public void maximizeWindow()
        {
            GLFW.GLFW.MaximizeWindow(windowHandle);
        }

        internal static void setIcon(GLFWWindow windowHandle, GLFWImage image)
        {
            //if (SharedLibraryLoader.isMac)
            //    return;

            GLFW.GLFW.SetWindowIcon(windowHandle, image);
        }

        public void setTitle(string title)
        {
            GLFW.GLFW.SetWindowTitle(windowHandle, title);
        }

        public void setSizeLimits(int minWidth, int minHeight, int maxWidth, int maxHeight)
        {
            setSizeLimits(windowHandle, minWidth, minHeight, maxWidth, maxHeight);
        }

        internal static void setSizeLimits(GLFWWindow windowHandle, int minWidth, int minHeight, int maxWidth, int maxHeight)
        {
            GLFW.GLFW.SetWindowSizeLimits(windowHandle,
                minWidth > -1 ? minWidth : GLFW.GLFW.DontCare,
                minHeight > -1 ? minHeight : GLFW.GLFW.DontCare,
                maxWidth > -1 ? maxWidth : GLFW.GLFW.DontCare,
                maxHeight > -1 ? maxHeight : GLFW.GLFW.DontCare);
        }

        internal Graphics getGraphics()
        {
            return graphics;
        }

        internal Input getInput()
        {
            return input;
        }

        public GLFWWindow getWindowHandle()
        {
            return windowHandle;
        }

        void windowHandleChanged(GLFWWindow windowHandle)
        {
            this.windowHandle = windowHandle;
            input.windowHandleChanged(windowHandle);
        }

        internal bool update()
        {
            if (!listenerInitialized)
            {
                initializeListener();
            }

            lock (runnables)
            {
                executedRunnables.AddRange(runnables);
                runnables.Clear();
            }

            foreach (var runnable in executedRunnables)
            {
                runnable.Invoke();
            }

            var shouldRender = executedRunnables.Count > 0 || graphics.isContinuousRendering();
            executedRunnables.Clear();

            if (!iconified)
                input.update();

            lock (this)
            {
                shouldRender     |= requestRendering && !iconified;
                requestRendering =  false;
            }

            if (shouldRender)
            {
                graphics.update();
                listener.render();
                GLFW.GLFW.SwapBuffers(windowHandle);
            }

            if (!iconified)
                input.prepareNext();

            return shouldRender;
        }

        internal void setRequestRendering()
        {
            lock (this)
            {
                requestRendering = true;
            }
        }

        internal bool shouldClose()
        {
            return GLFW.GLFW.WindowShouldClose(windowHandle);
        }

        internal ApplicationConfiguration getConfig()
        {
            return config;
        }

        internal bool isListenerInitialized()
        {
            return listenerInitialized;
        }

        void initializeListener()
        {
            if (!listenerInitialized)
            {
                listener.create();
                listener.resize(graphics.getWidth(), graphics.getHeight());
                listenerInitialized = true;
            }
        }

        internal void makeCurrent()
        {
            Gdx.graphics = graphics;
            Gdx.input    = input;

            GLFW.GLFW.MakeContextCurrent(windowHandle);
        }

        public void Dispose()
        {
            listener.pause();
            listener.dispose();
            graphics.Dispose();
            input.Dispose();
            GLFW.GLFW.DestroyWindow(windowHandle);
        }
    }
}
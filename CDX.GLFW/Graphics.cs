using System;
using CDX.Graphics;
using CDX.Graphics.GLUtils;
using CDX.Utils;
using OpenTK.Graphics.OpenGL4;

namespace CDX.GLFWBackend
{
    public class DisplayMode : CDX.DisplayMode
    {
        readonly global::GLFW.GLFW.Monitor monitorHandle;

        internal DisplayMode(global::GLFW.GLFW.Monitor monitor, int width, int height, int refreshRate, int bitsPerPixel) : base(width, height, refreshRate, bitsPerPixel)
        {
            monitorHandle = monitor;
        }

        public global::GLFW.GLFW.Monitor getMonitor()
        {
            return monitorHandle;
        }
    }

    public class Lwjgl3Monitor : Monitor
    {
        readonly global::GLFW.GLFW.Monitor monitorHandle;

        internal Lwjgl3Monitor(global::GLFW.GLFW.Monitor monitor, int virtualX, int virtualY, string name) : base(virtualX, virtualY, name)
        {
            monitorHandle = monitor;
        }

        public global::GLFW.GLFW.Monitor getMonitorHandle()
        {
            return monitorHandle;
        }
    }

    public class Graphics : IGraphics, IDisposable
    {
        private readonly Window window;
        private          GLVersion    glVersion;
        private volatile int          backBufferWidth;
        private volatile int          backBufferHeight;
        private volatile int          logicalWidth;
        private volatile int          logicalHeight;
        private volatile bool         isContinuous = true;
        private          BufferFormat bufferFormat;
        private          long         lastFrameTime = -1;
        private          float        deltaTime;
        private          long         frameId;
        private          long         frameCounterStart = 0;
        private          int          frames;
        private          int          fps;
        private          int          windowPosXBeforeFullscreen;
        private          int          windowPosYBeforeFullscreen;
        private          CDX.DisplayMode  displayModeBeforeFullscreen = null;

        public Graphics(Window window)
        {
            this.window = window;

            updateFramebufferInfo();
            initiateGL();
            GLFW.GLFW.SetFramebufferSizeCallback(window.getWindowHandle(), resizeCallback);
        }

        private void resizeCallback(global::GLFW.GLFW.Window window1, int width, int height)
        {
            updateFramebufferInfo();
            if (!window.isListenerInitialized())
            {
                return;
            }

            window.makeCurrent();
            GL.Viewport(0, 0, width, height);
            window.getListener().resize(getWidth(), getHeight());
            window.getListener().render();
            GLFW.GLFW.SwapBuffers(window1);
        }

        private void initiateGL()
        {
            String versionString  = GL.GetString(StringName.Version);  //GL11.glGetString(GL11.GL_VERSION);
            String vendorString   = GL.GetString(StringName.Vendor);   //GL11.glGetString(GL11.GL_VENDOR);
            String rendererString = GL.GetString(StringName.Renderer); //GL11.glGetString(GL11.GL_RENDERER);
            
            glVersion = new GLVersion(ApplicationType.Desktop, versionString, vendorString, rendererString);
        }

        public Window getWindow()
        {
            return window;
        }

        private void updateFramebufferInfo()
        {
            GLFW.GLFW.GetFramebufferSize(window.getWindowHandle(), out backBufferWidth, out backBufferHeight);
            GLFW.GLFW.GetWindowSize(window.getWindowHandle(), out logicalWidth, out logicalHeight);
            ApplicationConfiguration config = window.getConfig();
            bufferFormat = new BufferFormat(config.r, config.g, config.b, config.a, config.depth, config.stencil,
                config.samples, false);
        }

        internal void update()
        {
            long time = TimeUtils.nanoTime();
            if (lastFrameTime == -1)
                lastFrameTime = time;
            deltaTime     = (time - lastFrameTime) / 1000000000.0f;
            lastFrameTime = time;

            if (time - frameCounterStart >= 1000000000)
            {
                fps               = frames;
                frames            = 0;
                frameCounterStart = time;
            }

            frames++;
            frameId++;
        }


        public int getWidth()
        {
            if (window.getConfig().hdpiMode == HdpiMode.Pixels)
            {
                return backBufferWidth;
            }
            else
            {
                return logicalWidth;
            }
        }


        public int getHeight()
        {
            if (window.getConfig().hdpiMode == HdpiMode.Pixels)
            {
                return backBufferHeight;
            }
            else
            {
                return logicalHeight;
            }
        }


        public int getBackBufferWidth()
        {
            return backBufferWidth;
        }


        public int getBackBufferHeight()
        {
            return backBufferHeight;
        }

        public int getLogicalWidth()
        {
            return logicalWidth;
        }

        public int getLogicalHeight()
        {
            return logicalHeight;
        }


        public long getFrameId()
        {
            return frameId;
        }


        public float getDeltaTime()
        {
            return deltaTime;
        }


        public float getRawDeltaTime()
        {
            return deltaTime;
        }


        public int getFramesPerSecond()
        {
            return fps;
        }


        public GraphicsType getType()
        {
            return GraphicsType.GLFW;
        }

        public float getPpiX()
        {
            return getPpcX() / 0.393701f;
        }


        public float getPpiY()
        {
            return getPpcY() / 0.393701f;
        }


        public float getPpcX()
        {
            Lwjgl3Monitor monitor = (Lwjgl3Monitor) getMonitor();
            GLFW.GLFW.GetMonitorPhysicalSize(monitor.getMonitorHandle(), out var sizeX, out var sizeY);
            CDX.DisplayMode mode = getDisplayMode();
            return mode.width / (float) sizeX * 10;
        }


        public float getPpcY()
        {
            Lwjgl3Monitor monitor = (Lwjgl3Monitor) getMonitor();
            GLFW.GLFW.GetMonitorPhysicalSize(monitor.getMonitorHandle(), out var sizeX, out var sizeY);
            CDX.DisplayMode mode = getDisplayMode();
            return mode.height / (float) sizeY * 10;
        }


        public float getDensity()
        {
            return getPpiX() / 160f;
        }


        public bool supportsDisplayModeChange()
        {
            return true;
        }


        public Monitor getPrimaryMonitor()
        {
            return ApplicationConfiguration.toLwjgl3Monitor(GLFW.GLFW.GetPrimaryMonitor());
        }


        public Monitor getMonitor()
        {
            Monitor[] monitors = getMonitors();
            Monitor   result   = monitors[0];

            GLFW.GLFW.GetWindowPos(window.getWindowHandle(), out var windowX, out var windowY);
            GLFW.GLFW.GetWindowSize(window.getWindowHandle(), out var windowWidth, out var windowHeight);
            int overlap;
            int bestOverlap = 0;

            foreach (Monitor monitor in monitors)
            {
                CDX.DisplayMode mode = getDisplayMode(monitor);

                overlap = Math.Max(0,
                              Math.Min(windowX + windowWidth, monitor.virtualX + mode.width)
                              - Math.Max(windowX, monitor.virtualX))
                          * Math.Max(0, Math.Min(windowY + windowHeight, monitor.virtualY + mode.height)
                                        - Math.Max(windowY, monitor.virtualY));

                if (bestOverlap < overlap)
                {
                    bestOverlap = overlap;
                    result      = monitor;
                }
            }

            return result;
        }


        public Monitor[] getMonitors()
        {
            var glfwMonitors = GLFW.GLFW.GetMonitors();

            var monitors = new Lwjgl3Monitor[glfwMonitors.Length];
            for (int i = 0; i < glfwMonitors.Length; i++)
            {
                var glfwMonitor = glfwMonitors[i];
                monitors[i] = ApplicationConfiguration.toLwjgl3Monitor(glfwMonitor);
            }

            return monitors;
        }


        public CDX.DisplayMode[] getDisplayModes()
        {
            return ApplicationConfiguration.getDisplayModes(getMonitor());
        }


        public CDX.DisplayMode[] getDisplayModes(Monitor monitor)
        {
            return ApplicationConfiguration.getDisplayModes(monitor);
        }


        public CDX.DisplayMode getDisplayMode()
        {
            return ApplicationConfiguration.getDisplayMode(getMonitor());
        }


        public CDX.DisplayMode getDisplayMode(Monitor monitor)
        {
            return ApplicationConfiguration.getDisplayMode(monitor);
        }


        public bool setFullscreenMode(CDX.DisplayMode displayMode)
        {
            window.getInput().resetPollingStates();
            DisplayMode newMode = (DisplayMode) displayMode;
            if (isFullscreen())
            {
                DisplayMode currentMode = (DisplayMode) getDisplayMode();
                if (currentMode.getMonitor() == newMode.getMonitor() && currentMode.refreshRate == newMode.refreshRate)
                {
                    // same monitor and refresh rate
                    GLFW.GLFW.SetWindowSize(window.getWindowHandle(), newMode.width, newMode.height);
                }
                else
                {
                    // different monitor and/or refresh rate
                    GLFW.GLFW.SetWindowMonitor(window.getWindowHandle(), newMode.getMonitor(),
                        0, 0, newMode.width, newMode.height, newMode.refreshRate);
                }
            }
            else
            {
                // store window position so we can restore it when switching from fullscreen to windowed later
                storeCurrentWindowPositionAndDisplayMode();

                // switch from windowed to fullscreen
                GLFW.GLFW.SetWindowMonitor(window.getWindowHandle(), newMode.getMonitor(),
                    0, 0, newMode.width, newMode.height, newMode.refreshRate);
            }

            updateFramebufferInfo();
            return true;
        }

        private void storeCurrentWindowPositionAndDisplayMode()
        {
            windowPosXBeforeFullscreen  = window.getPositionX();
            windowPosYBeforeFullscreen  = window.getPositionY();
            displayModeBeforeFullscreen = getDisplayMode();
        }


        public bool setWindowedMode(int width, int height)
        {
            window.getInput().resetPollingStates();
            if (!isFullscreen())
            {
                GLFW.GLFW.SetWindowSize(window.getWindowHandle(), width, height);
            }
            else
            {
                if (displayModeBeforeFullscreen == null)
                {
                    storeCurrentWindowPositionAndDisplayMode();
                }

                // TODO: might not work, should be a nullptr
                GLFW.GLFW.SetWindowMonitor(window.getWindowHandle(), default(global::GLFW.GLFW.Monitor),
                    windowPosXBeforeFullscreen, windowPosYBeforeFullscreen, width, height,
                    displayModeBeforeFullscreen.refreshRate);
            }

            updateFramebufferInfo();
            return true;
        }


        public void setTitle(string title)
        {
            if (title == null)
            {
                title = "";
            }

            GLFW.GLFW.SetWindowTitle(window.getWindowHandle(), title);
        }


        public void setUndecorated(bool undecorated)
        {
            var config = getWindow().getConfig();
            config.setDecorated(!undecorated);

            throw new NotImplementedException();
            //GLFW.GLFW.glfwSetWindowAttrib(window.getWindowHandle(), GLFW.GLFW_DECORATED, undecorated ? GLFW.GLFW_FALSE : GLFW.GLFW_TRUE);
        }


        public void setResizable(bool resizable)
        {
            var config = getWindow().getConfig();
            config.setResizable(resizable);
            
            throw new NotImplementedException();
            //GLFW.glfwSetWindowAttrib(window.getWindowHandle(), GLFW.GLFW_RESIZABLE, resizable ? GLFW.GLFW_TRUE : GLFW.GLFW_FALSE);
        }


        public void setVSync(bool vsync)
        {
            GLFW.GLFW.SwapInterval(vsync ? 1 : 0);
        }


        public BufferFormat getBufferFormat()
        {
            return bufferFormat;
        }


        public bool supportsExtension(string extension)
        {
            return GLFW.GLFW.ExtensionSupported(extension);
        }


        public void setContinuousRendering(bool isContinuous)
        {
            this.isContinuous = isContinuous;
        }


        public bool isContinuousRendering()
        {
            return isContinuous;
        }


        public void requestRendering()
        {
            window.setRequestRendering();
        }


        public bool isFullscreen()
        {
            // TODO: won't work because we don't set fullscreen corectly..
            return GLFW.GLFW.GetWindowMonitor(window.getWindowHandle()) != default(global::GLFW.GLFW.Monitor);
        }


        public void Dispose()
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using CDX.Graphics.GLUtils;
using CDX.Utils;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace CDX.GLFWBackend
{
    public class Application : IApplication
    {
        private readonly ApplicationConfiguration   config;
        private readonly List<Window>               windows = new List<Window>();
        private          Window                     currentWindow;
        private          IAudio                           audio;
        private readonly IFiles                           files;
        private readonly INet                             net;
        private readonly Dictionary<string, IPreferences> preferences = new Dictionary<string, IPreferences>();
        private readonly Clipboard                  clipboard;
        private          int                              logLevel = (int) LogLevel.LOG_INFO;
        private          IApplicationLogger               applicationLogger;
        private          bool                             running            = true;
        private readonly List<Runnable>                   runnables          = new List<Runnable>();
        private readonly List<Runnable>                   executedRunnables  = new List<Runnable>();
        private readonly List<ILifecycleListener>         lifecycleListeners = new List<ILifecycleListener>();

        private static GLVersion glVersion;
        //private static   Callback                         glDebugCallback;

        internal static void initializeGlfw()
        {
            {
                GLFW.GLFW.SetErrorCallback(onGlfwError);
                if (!GLFW.GLFW.Init())
                {
                    throw new Exception("Unable to initialize GLFW");
                }
            }
        }

        private static void onGlfwError(global::GLFW.GLFW.ErrorCode errorCode, string description)
        {
            Gdx.app.error("GLFW", $"Error: {errorCode} {description}");
        }

        public Application(IApplicationListener listener, ApplicationConfiguration config)
        {
            
                       
            initializeGlfw();
            setApplicationLogger(new ApplicationLogger());
            this.config = ApplicationConfiguration.copy(config);
            if (this.config.title == null) this.config.title = listener.GetType().Name;
            Gdx.app = this;
            if (!config.disableAudio)
            {
                try
                {
                    //this.audio = Gdx.audio = new OpenALAudio(config.audioDeviceSimultaneousSources, config.audioDeviceBufferCount, config.audioDeviceBufferSize);
                }
                catch (Exception t)
                {
                    log("Lwjgl3Application", "Couldn't initialize audio, disabling audio", t);
                    //this.audio = Gdx.audio = new MockAudio();
                }
            }
            else
            {
                //this.audio = Gdx.audio = new MockAudio();
            }

            files     = Gdx.files = new Files();
            net       = Gdx.net   = new Net();
            clipboard = new Clipboard();

            var window = createWindow(config, listener, null);
            windows.Add(window);

            loop();
            cleanupWindows();
            cleanup();
        }

        private Window createWindow(ApplicationConfiguration config, IApplicationListener listener, global::GLFW.GLFW.Window? sharedContext = null)
        {
            var window = new Window(listener, config);
            if (sharedContext == null)
            {
                // the main window is created immediately
                createWindow(window, config, sharedContext);
            }
            else
            {
                // creation of additional windows is deferred to avoid GL context trouble
                postRunnable((() =>
                {
                    createWindow(window, config, sharedContext);
                    windows.Add(window);
                }));
            }

            return window;
        }

        private void createWindow(Window window, ApplicationConfiguration config, global::GLFW.GLFW.Window? sharedContext)
        {
            var windowHandle = createGlfwWindow(config, sharedContext);
            window.create(windowHandle);
            window.setVisible(config.initialVisible);

            for (var i = 0; i < 2; i++)
            {
                GL.ClearColor(config.initialBackgroundColor.r, config.initialBackgroundColor.g, config.initialBackgroundColor.b,
                    config.initialBackgroundColor.a);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GLFW.GLFW.SwapBuffers(windowHandle);
            }
        }

        static global::GLFW.GLFW.Window createGlfwWindow(ApplicationConfiguration config, global::GLFW.GLFW.Window? sharedContextWindow)
        {
            GLFW.GLFW.DefaultWindowHints();
            GLFW.GLFW.WindowHint(GLFW.GLFW.Hint.Visible, false);
            GLFW.GLFW.WindowHint(GLFW.GLFW.Hint.Resizable, config.windowResizable);
            GLFW.GLFW.WindowHint(GLFW.GLFW.Hint.Maximized, config.windowMaximized);

            if (sharedContextWindow == null)
            {
                GLFW.GLFW.WindowHint(GLFW.GLFW.Hint.RedBits, config.r);
                GLFW.GLFW.WindowHint(GLFW.GLFW.Hint.GreenBits, config.g);
                GLFW.GLFW.WindowHint(GLFW.GLFW.Hint.BlueBits, config.b);
                GLFW.GLFW.WindowHint(GLFW.GLFW.Hint.AlphaBits, config.a);
                GLFW.GLFW.WindowHint(GLFW.GLFW.Hint.StencilBits, config.stencil);
                GLFW.GLFW.WindowHint(GLFW.GLFW.Hint.DepthBits, config.depth);
                GLFW.GLFW.WindowHint(GLFW.GLFW.Hint.Samples, config.samples);
            }

            
            GLFW.GLFW.WindowHint(GLFW.GLFW.Hint.ContextVersionMajor, config.glContextMajorVersion);
            GLFW.GLFW.WindowHint(GLFW.GLFW.Hint.ContextVersionMinor, config.glContextMinorVersion);


            if (config.debug)
            {
                GLFW.GLFW.WindowHint(GLFW.GLFW.Hint.OpenglDebugContext, true);
            }

            var windowHandle = default(global::GLFW.GLFW.Window);

            if (config.fullscreenMode != null)
            {
                GLFW.GLFW.WindowHint(GLFW.GLFW.Hint.RefreshRate, config.fullscreenMode.refreshRate);
                windowHandle = GLFW.GLFW.CreateWindow(config.fullscreenMode.width, config.fullscreenMode.height, config.title, config.fullscreenMode.getMonitor(), null);
            }
            else
            {
                GLFW.GLFW.WindowHint(GLFW.GLFW.Hint.Decorated, config.windowDecorated);
                windowHandle = GLFW.GLFW.CreateWindow(config.windowWidth, config.windowHeight, config.title, null, null);
            }

            if (windowHandle == default(global::GLFW.GLFW.Window))
            {
                throw new Exception("Couldn't create window");
            }
            
            Window.setSizeLimits(windowHandle, config.windowMinWidth, config.windowMinHeight, config.windowMaxWidth, config.windowMaxHeight);
            if (config.fullscreenMode == null && !config.windowMaximized)
            {
                if (config.windowX == -1 && config.windowY == -1)
                {
                    var windowWidth                               = Math.Max(config.windowWidth, config.windowMinWidth);
                    var windowHeight                              = Math.Max(config.windowHeight, config.windowMinHeight);
                    if (config.windowMaxWidth > -1) windowWidth   = Math.Min(windowWidth, config.windowMaxWidth);
                    if (config.windowMaxHeight > -1) windowHeight = Math.Min(windowHeight, config.windowMaxHeight);
                    var vidMode                                   = GLFW.GLFW.GetVideoMode(GLFW.GLFW.GetPrimaryMonitor());
                    GLFW.GLFW.SetWindowPos(windowHandle, vidMode.Width / 2 - windowWidth / 2, vidMode.Height / 2 - windowHeight / 2);
                }
                else
                {
                    GLFW.GLFW.SetWindowPos(windowHandle, config.windowX, config.windowY);
                }
            }

            if (config.windowIconPaths != null)
            {
                // TODO: finish image stuff
                //Lwjgl3Window.setIcon(windowHandle, config.windowIconPaths, config.windowIconFileType);
            }

            GLFW.GLFW.MakeContextCurrent(windowHandle);
            GLFW.GLFW.SwapInterval(config.vSyncEnabled ? 1 : 0);
            
            // todo: in case i replace OpenTK with plain OpenGL, don't forget to bind gl context here
            Toolkit.Init();
            var context = new GraphicsContext(new ContextHandle(), null, null, 4, 1, GraphicsContextFlags.Debug);
            context.LoadAll();

            initiateGL();
            
            
            if (!glVersion.isVersionEqualToOrHigher(2, 0))
            {
                throw new Exception("wut wut");
                //throw new Exception("OpenGL 2.0 or higher with the FBO extension is required. OpenGL version: "+ GL11.glGetString(GL11.GL_VERSION) + "\n" + glVersion.getDebugVersionString());
            }
            if (!supportsFBO())
            {
                throw new Exception("wut wut");
                //throw new Exception("OpenGL 2.0 or higher with the FBO extension is required. OpenGL version: "+ GL11.glGetString(GL11.GL_VERSION) + ", FBO extension: false\n" + glVersion.getDebugVersionString());
            }

            if (config.debug)
            {
                //glDebugCallback = GLUtil.setupDebugMessageCallback(config.debugStream);
                //setGLDebugMessageControl(GLDebugMessageSeverity.NOTIFICATION, false);
            }

            return windowHandle;
        }

        private static void initiateGL()
        {
            var versionString  = GL.GetString(StringName.Version);  //GL11.glGetString(GL11.GL_VERSION);
            var vendorString   = GL.GetString(StringName.Vendor);   //GL11.glGetString(GL11.GL_VENDOR);
            var rendererString = GL.GetString(StringName.Renderer); //GL11.glGetString(GL11.GL_RENDERER);
            glVersion = new GLVersion(ApplicationType.Desktop, versionString, vendorString, rendererString);
            
            Gdx.app.log("Application", $"GL Version: \t{versionString}");
            Gdx.app.log("Application", $"GL Vendor: \t{vendorString}");
            Gdx.app.log("Application", $"GL Renderer: \t{rendererString}");
        }

        
        private static bool supportsFBO () {
            // FBO is in core since OpenGL 3.0, see https://www.opengl.org/wiki/Framebuffer_Object
            return glVersion.isVersionEqualToOrHigher(3, 0) || GLFW.GLFW.ExtensionSupported("GL_EXT_framebuffer_object")
                                                            || GLFW.GLFW.ExtensionSupported("GL_ARB_framebuffer_object");
        }
        
        private void loop()
        {
            var closedWindows = new List<Window>();
            while (running && windows.Count > 0)
            {
                // FIXME put it on a separate thread
                //if (audio is OpenALAudio) {
                //    ((OpenALAudio) audio).update();
                //}

                var haveWindowsRendered = false;
                closedWindows.Clear();
                foreach (var window in windows)
                {
                    window.makeCurrent();
                    currentWindow = window;
                    lock (lifecycleListeners)
                    {
                        haveWindowsRendered |= window.update();
                    }

                    if (window.shouldClose())
                    {
                        closedWindows.Add(window);
                    }
                }

                GLFW.GLFW.PollEvents();

                bool shouldRequestRendering;
                lock (runnables)
                {
                    shouldRequestRendering = runnables.Count > 0;
                    executedRunnables.Clear();
                    executedRunnables.AddRange(runnables);
                    runnables.Clear();
                }

                foreach (var runnable in executedRunnables)
                {
                    runnable.Invoke();
                }

                if (shouldRequestRendering)
                {
                    // Must follow Runnables execution so changes done by Runnables are reflected
                    // in the following render.
                    foreach (var window in windows)
                    {
                        if (!window.getGraphics().isContinuousRendering())
                            window.setRequestRendering();
                    }
                }

                foreach (var closedWindow in closedWindows)
                {
                    if (windows.Count == 1)
                    {
                        // Lifecycle listener methods have to be called before ApplicationListener methods. The
                        // application will be disposed when _all_ windows have been disposed, which is the case,
                        // when there is only 1 window left, which is in the process of being disposed.
                        for (var i = lifecycleListeners.Count - 1; i >= 0; i--)
                        {
                            var l = lifecycleListeners[i];
                            l.pause();
                            l.dispose();
                        }

                        lifecycleListeners.Clear();
                    }

                    closedWindow.Dispose();

                    windows.Remove(closedWindow);
                }

                if (!haveWindowsRendered)
                {
                    // Sleep a few milliseconds in case no rendering was requested
                    // with continuous rendering disabled.
                    try
                    {
                        Thread.Sleep(1000 / config.idleFPS);
                    }
                    catch (Exception e)
                    {
                        // ignore
                    }
                }
            }
        }

        private void cleanupWindows()
        {
            lock (lifecycleListeners)
            {
                foreach (var lifecycleListener in lifecycleListeners)
                {
                    lifecycleListener.pause();
                    lifecycleListener.dispose();
                }
            }

            foreach (var window in windows)
            {
                window.Dispose();
            }

            windows.Clear();
        }

        private void cleanup()
        {
            //if (audio is OpenALAudio) {
            //((OpenALAudio) audio).dispose();
            //}
            //errorCallback.free();
            //errorCallback = null;
            //if (glDebugCallback != null)
            //{
            //    glDebugCallback.free();
            //    glDebugCallback = null;
            //}

            GLFW.GLFW.Terminate();
        }

        public IApplicationListener getApplicationListener()
        {
            return currentWindow.getListener();
        }

        public IGraphics getGraphics()
        {
            return currentWindow.getGraphics();
        }

        public IAudio getAudio()
        {
            return audio;
        }

        public IInput getInput()
        {
            return currentWindow.getInput();
        }

        public IFiles getFiles()
        {
            return files;
        }

        public INet getNet()
        {
            return net;
        }

        public void log(string tag, string message)
        {
            if (logLevel >= (int) LogLevel.LOG_INFO) getApplicationLogger().debug(tag, message);
        }

        public void log(string tag, string message, Exception exception)
        {
            if (logLevel >= (int) LogLevel.LOG_INFO) getApplicationLogger().debug(tag, message, exception);
        }

        public void error(string tag, string message)
        {
            if (logLevel >= (int) LogLevel.LOG_ERROR) getApplicationLogger().debug(tag, message);
        }

        public void error(string tag, string message, Exception exception)
        {
            if (logLevel >= (int) LogLevel.LOG_ERROR) getApplicationLogger().debug(tag, message, exception);
        }

        public void debug(string tag, string message)
        {
            if (logLevel >= (int) LogLevel.LOG_DEBUG) getApplicationLogger().debug(tag, message);
        }

        public void debug(string tag, string message, Exception exception)
        {
            if (logLevel >= (int) LogLevel.LOG_DEBUG) getApplicationLogger().debug(tag, message, exception);
        }

        public void setLogLevel(int logLevel)
        {
            this.logLevel = logLevel;
        }

        public int getLogLevel()
        {
            return logLevel;
        }

        public void setApplicationLogger(IApplicationLogger applicationLogger)
        {
            this.applicationLogger = applicationLogger;
        }

        public IApplicationLogger getApplicationLogger()
        {
            return applicationLogger;
        }

        public ApplicationType getType()
        {
            return ApplicationType.Desktop;
        }

        public int getVersion()
        {
            return 0;
        }

        public long getJavaHeap()
        {
            return 0;
        }

        public long getNativeHeap()
        {
            return 0;
        }

        public IPreferences getPreferences(string name)
        {
            if (preferences.ContainsKey(name))
            {
                return preferences[name];
            }
            else
            {
                // var prefs = new Lwjgl3Preferences(
                //     new Lwjgl3FileHandle(new File(config.preferencesDirectory, name), config.preferencesFileType));
                // preferences.put(name, prefs);
                // return prefs;
            }

            throw new NotImplementedException();
        }

        public IClipboard getClipboard()
        {
            return clipboard;
        }

        public void postRunnable(Runnable runnable)
        {
            lock (runnables)
            {
                runnables.Add(runnable);
            }
        }

        public void exit()
        {
            running = false;
        }

        public void addLifecycleListener(ILifecycleListener listener)
        {
            lock (lifecycleListeners)
            {
                lifecycleListeners.Add(listener);
            }
        }

        public void removeLifecycleListener(ILifecycleListener listener)
        {
            lock (lifecycleListeners)
            {
                lifecycleListeners.Remove(listener);
            }
        }
    }
}
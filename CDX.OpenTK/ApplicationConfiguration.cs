namespace CDX.GLFWBackend
{
    public enum HdpiMode
    {
        /**
         * mouse coordinates, {@link Graphics#getWidth()} and
         * {@link Graphics#getHeight()} will return logical coordinates
         * according to the system defined HDPI scaling. Rendering will be
         * performed to a backbuffer at raw resolution. Use {@link HdpiUtils}
         * when calling {@link GL20#glScissor} or {@link GL20#glViewport} which
         * expect raw coordinates.
         */
        Logical,

        /**
         * Mouse coordinates, {@link Graphics#getWidth()} and
         * {@link Graphics#getHeight()} will return raw pixel coordinates
         * irrespective of the system defined HDPI scaling.
         */
        Pixels
    }

    public class ApplicationConfiguration : WindowConfiguration
    {
        internal bool disableAudio                   = false;
        int           audioDeviceSimultaneousSources = 16;
        int           audioDeviceBufferSize          = 512;
        int           audioDeviceBufferCount         = 9;

        internal bool useGL30                   = true;
        internal int  gles30ContextMajorVersion = 3;
        internal int  gles30ContextMinorVersion = 2;

        internal int r       = 8;
        internal int g       = 8;
        internal int b       = 8;
        internal int a       = 8;
        internal int depth   = 16;
        internal int stencil = 0;
        internal int samples = 0;

        internal bool         vSyncEnabled = true;
        internal int idleFPS      = 60;

        string   preferencesDirectory = ".prefs/";
        FileType preferencesFileType  = FileType.External;

        internal HdpiMode hdpiMode = HdpiMode.Logical;

        internal bool debug = false;

        internal static ApplicationConfiguration copy(ApplicationConfiguration config)
        {
            ApplicationConfiguration copy = new ApplicationConfiguration();
            copy.set(config);
            return copy;
        }

        void set(ApplicationConfiguration config)
        {
            setWindowConfiguration(config);
            disableAudio                   = config.disableAudio;
            audioDeviceSimultaneousSources = config.audioDeviceSimultaneousSources;
            audioDeviceBufferSize          = config.audioDeviceBufferSize;
            audioDeviceBufferCount         = config.audioDeviceBufferCount;
            useGL30                        = config.useGL30;
            gles30ContextMajorVersion      = config.gles30ContextMajorVersion;
            gles30ContextMinorVersion      = config.gles30ContextMinorVersion;
            r                              = config.r;
            g                              = config.g;
            b                              = config.b;
            a                              = config.a;
            depth                          = config.depth;
            stencil                        = config.stencil;
            samples                        = config.samples;
            vSyncEnabled                   = config.vSyncEnabled;
            preferencesDirectory           = config.preferencesDirectory;
            preferencesFileType            = config.preferencesFileType;
            hdpiMode                       = config.hdpiMode;
            debug                          = config.debug;
        }

        /**
         * @param visibility whether the window will be visible on creation. (default true)
         */
        public void setInitialVisible(bool visibility)
        {
            initialVisible = visibility;
        }

        /**
         * Whether to disable audio or not. If set to false, the returned audio
         * class instances like {@link Audio} or {@link Music} will be mock
         * implementations.
         */
        public void setDisableAudio(bool disableAudio)
        {
            this.disableAudio = disableAudio;
        }

        /**
         * Sets the audio device configuration.
         * 
         * @param simultaniousSources
         *            the maximum number of sources that can be played
         *            simultaniously (default 16)
         * @param bufferSize
         *            the audio device buffer size in samples (default 512)
         * @param bufferCount
         *            the audio device buffer count (default 9)
         */
        public void setAudioConfig(int simultaniousSources, int bufferSize, int bufferCount)
        {
            audioDeviceSimultaneousSources = simultaniousSources;
            audioDeviceBufferSize          = bufferSize;
            audioDeviceBufferCount         = bufferCount;
        }

        /**
         * Sets whether to use OpenGL ES 3.0 emulation. If the given major/minor
         * version is not supported, the backend falls back to OpenGL ES 2.0
         * emulation. The default parameters for major and minor should be 3 and 2
         * respectively to be compatible with Mac OS X. Specifying major version 4
         * and minor version 2 will ensure that all OpenGL ES 3.0 features are
         * supported. Note however that Mac OS X does only support 3.2.
         * 
         * @see <a href=
         *      "http://legacy.lwjgl.org/javadoc/org/lwjgl/opengl/ContextAttribs.html">
         *      LWJGL OSX ContextAttribs note
         * 
         * @param useGL30
         *            whether to use OpenGL ES 3.0
         * @param gles3MajorVersion
         *            OpenGL ES major version, use 3 as default
         * @param gles3MinorVersion
         *            OpenGL ES minor version, use 2 as default
         */
        public void useOpenGL3(bool useGL30, int gles3MajorVersion, int gles3MinorVersion)
        {
            this.useGL30                   = useGL30;
            gles30ContextMajorVersion = gles3MajorVersion;
            gles30ContextMinorVersion = gles3MinorVersion;
        }

        /**
         * Sets the bit depth of the color, depth and stencil buffer as well as
         * multi-sampling.
         * 
         * @param r
         *            red bits (default 8)
         * @param g
         *            green bits (default 8)
         * @param b
         *            blue bits (default 8)
         * @param a
         *            alpha bits (default 8)
         * @param depth
         *            depth bits (default 16)
         * @param stencil
         *            stencil bits (default 0)
         * @param samples
         *            MSAA samples (default 0)
         */
        public void setBackBufferConfig(int r, int g, int b, int a, int depth, int stencil, int samples)
        {
            this.r       = r;
            this.g       = g;
            this.b       = b;
            this.a       = a;
            this.depth   = depth;
            this.stencil = stencil;
            this.samples = samples;
        }

        /**
         * Sets whether to use vsync. This setting can be changed anytime at runtime
         * via {@link Graphics#setVSync(bool)}.
         */
        public void useVsync(bool vsync)
        {
            vSyncEnabled = vsync;
        }

        /**Sets the polling rate during idle time in non-continuous rendering mode. Must be positive. 
         * Default is 60. */
        public void setIdleFPS(int fps)
        {
            idleFPS = fps;
        }

        /**
         * Sets the directory where {@link Preferences} will be stored, as well as
         * the file type to be used to store them. Defaults to "$USER_HOME/.prefs/"
         * and {@link FileType#External}.
         */
        public void setPreferencesConfig(string preferencesDirectory, FileType preferencesFileType)
        {
            this.preferencesDirectory = preferencesDirectory;
            this.preferencesFileType  = preferencesFileType;
        }

        /**
         * Defines how HDPI monitors are handled. Operating systems may have a
         * per-monitor HDPI scale setting. The operating system may report window
         * width/height and mouse coordinates in a logical coordinate system at a
         * lower resolution than the actual physical resolution. This setting allows
         * you to specify whether you want to work in logical or raw pixel units.
         * See {@link HdpiMode} for more information. Note that some OpenGL
         * functions like {@link GL#glViewport()} and {@link GL#glScissor()} require
         * raw pixel units. Use {@link HdpiUtils} to help with the conversion if
         * HdpiMode is set to {@link HdpiMode#Logical}. Defaults to {@link HdpiMode#Logical}.
         */
        public void setHdpiMode(HdpiMode mode)
        {
            hdpiMode = mode;
        }

        /**
         * Enables use of OpenGL debug message callbacks. If not supported by the core GL driver
         * (since GL 4.3), this uses the KHR_debug, ARB_debug_output or AMD_debug_output extension
         * if available. By default, debug messages with NOTIFICATION severity are disabled to
         * avoid log spam.
         *
         * You can call with {@link System#err} to output to the "standard" error output stream.
         *
         * Use {@link Lwjgl3Application#setGLDebugMessageControl(Lwjgl3Application.GLDebugMessageSeverity, bool)}
         * to enable or disable other severity debug levels.
         */
        public void enableGLDebugOutput(bool enable)
        {
            debug = enable;
        }

        /**
         * @return the currently active {@link DisplayMode} of the primary monitor
         */
        public static CDX.DisplayMode getDisplayMode()
        {
            Application.initializeGlfw();
            var videoMode = GLFW.GLFW.GetVideoMode(GLFW.GLFW.GetPrimaryMonitor());
            return new DisplayMode(GLFW.GLFW.GetPrimaryMonitor(), videoMode.Width, videoMode.Height, videoMode.RefreshRate,
                videoMode.RedBits + videoMode.GreenBits + videoMode.BlueBits);
        }

        /**
         * @return the currently active {@link DisplayMode} of the given monitor
         */
        public static CDX.DisplayMode getDisplayMode(Monitor monitor)
        {
            Application.initializeGlfw();
            var videoMode = GLFW.GLFW.GetVideoMode(((Lwjgl3Monitor) monitor).getMonitorHandle());
            return new DisplayMode(((Lwjgl3Monitor) monitor).getMonitorHandle(), videoMode.Width, videoMode.Height, videoMode.RefreshRate,
                videoMode.RedBits + videoMode.GreenBits + videoMode.BlueBits);
        }

        /**
         * @return the available {@link DisplayMode}s of the primary monitor
         */
        public static CDX.DisplayMode[] getDisplayModes()
        {
            Application.initializeGlfw();
            var           videoModes = GLFW.GLFW.GetVideoModes(GLFW.GLFW.GetPrimaryMonitor());
            CDX.DisplayMode[] result     = new CDX.DisplayMode[videoModes.Length];
            for (int i = 0; i < result.Length; i++)
            {
                var videoMode = videoModes[i];
                result[i] = new DisplayMode(GLFW.GLFW.GetPrimaryMonitor(), videoMode.Width, videoMode.Height,
                    videoMode.RefreshRate, videoMode.RedBits + videoMode.GreenBits + videoMode.BlueBits);
            }

            return result;
        }

        /**
         * @return the available {@link DisplayMode}s of the given {@link Monitor}
         */
        public static CDX.DisplayMode[] getDisplayModes(Monitor monitor)
        {
            Application.initializeGlfw();
            var           videoModes = GLFW.GLFW.GetVideoModes(((Lwjgl3Monitor) monitor).getMonitorHandle());
            CDX.DisplayMode[] result     = new CDX.DisplayMode[videoModes.Length];
            for (int i = 0; i < result.Length; i++)
            {
                var videoMode = videoModes[i];
                result[i] = new DisplayMode(((Lwjgl3Monitor) monitor).getMonitorHandle(), videoMode.Width, videoMode.Height,
                    videoMode.RefreshRate, videoMode.RedBits + videoMode.GreenBits + videoMode.BlueBits);
            }

            return result;
        }

        /**
         * @return the primary {@link Monitor}
         */
        public static Monitor getPrimaryMonitor()
        {
            Application.initializeGlfw();
            return toLwjgl3Monitor(GLFW.GLFW.GetPrimaryMonitor());
        }

        /**
         * @return the connected {@link Monitor}s
         */
        public static Monitor[] getMonitors()
        {
            Application.initializeGlfw();
            var       glfwMonitors = GLFW.GLFW.GetMonitors();
            Monitor[] monitors     = new Monitor[glfwMonitors.Length];
            for (int i = 0; i < glfwMonitors.Length; i++)
            {
                monitors[i] = toLwjgl3Monitor(glfwMonitors[i]);
            }

            return monitors;
        }

        internal static Lwjgl3Monitor toLwjgl3Monitor(global::GLFW.GLFW.Monitor glfwMonitor)
        {
            GLFW.GLFW.GetMonitorPos(glfwMonitor, out var virtualX, out var virtualY);
            //string name     = GLFW.GetMonitorName(glfwMonitor);
            return new Lwjgl3Monitor(glfwMonitor, virtualX, virtualY, "unknow name");
        }
    }
}
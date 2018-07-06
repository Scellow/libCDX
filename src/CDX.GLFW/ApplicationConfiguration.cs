using GLFWMonitor = GLFW.GLFW.Monitor;

namespace CDX.GLFWBackend
{
    public enum HdpiMode
    {
        Logical,
        Pixels
    }

    public class ApplicationConfiguration : WindowConfiguration
    {
        internal bool disableAudio                   = false;
        int           audioDeviceSimultaneousSources = 16;
        int           audioDeviceBufferSize          = 512;
        int           audioDeviceBufferCount         = 9;

        internal int  glContextMajorVersion = 4;
        internal int  glContextMinorVersion = 1;

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
            glContextMajorVersion          = config.glContextMajorVersion;
            glContextMinorVersion          = config.glContextMinorVersion;
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

        public void setInitialVisible(bool visibility)
        {
            initialVisible = visibility;
        }

        public void setDisableAudio(bool disableAudio)
        {
            this.disableAudio = disableAudio;
        }

        public void setAudioConfig(int simultaniousSources, int bufferSize, int bufferCount)
        {
            audioDeviceSimultaneousSources = simultaniousSources;
            audioDeviceBufferSize          = bufferSize;
            audioDeviceBufferCount         = bufferCount;
        }
        
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

        public void useVsync(bool vsync)
        {
            vSyncEnabled = vsync;
        }

        public void setIdleFPS(int fps)
        {
            idleFPS = fps;
        }

        public void setPreferencesConfig(string preferencesDirectory, FileType preferencesFileType)
        {
            this.preferencesDirectory = preferencesDirectory;
            this.preferencesFileType  = preferencesFileType;
        }

        public void setHdpiMode(HdpiMode mode)
        {
            hdpiMode = mode;
        }

        public void enableGLDebugOutput(bool enable)
        {
            debug = enable;
        }

        public static CDX.DisplayMode getDisplayMode()
        {
            Application.initializeGlfw();
            var videoMode = GLFW.GLFW.GetVideoMode(GLFW.GLFW.GetPrimaryMonitor());
            return new DisplayMode(GLFW.GLFW.GetPrimaryMonitor(), videoMode.Width, videoMode.Height, videoMode.RefreshRate,
                videoMode.RedBits + videoMode.GreenBits + videoMode.BlueBits);
        }

        public static CDX.DisplayMode getDisplayMode(Monitor monitor)
        {
            Application.initializeGlfw();
            var videoMode = GLFW.GLFW.GetVideoMode(((Lwjgl3Monitor) monitor).getMonitorHandle());
            return new DisplayMode(((Lwjgl3Monitor) monitor).getMonitorHandle(), videoMode.Width, videoMode.Height, videoMode.RefreshRate,
                videoMode.RedBits + videoMode.GreenBits + videoMode.BlueBits);
        }

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

        public static Monitor getPrimaryMonitor()
        {
            Application.initializeGlfw();
            return toLwjgl3Monitor(GLFW.GLFW.GetPrimaryMonitor());
        }

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

        internal static Lwjgl3Monitor toLwjgl3Monitor(GLFWMonitor glfwMonitor)
        {
            GLFW.GLFW.GetMonitorPos(glfwMonitor, out var virtualX, out var virtualY);
            //string name     = GLFW.GetMonitorName(glfwMonitor);
            return new Lwjgl3Monitor(glfwMonitor, virtualX, virtualY, "unknow name");
        }
    }
}
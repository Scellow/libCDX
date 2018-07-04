using System;
using CDX.Utils;

namespace CDX
{
    public enum ApplicationType
    {
        Android,
        Desktop,
        HeadlessDesktop,
        Applet,
        WebGL,
        iOS
    }

    public enum LogLevel
    {
        LOG_NONE  = 0,
        LOG_DEBUG = 3,
        LOG_INFO  = 2,
        LOG_ERROR = 1
    }

    public interface IApplication
    {
        IApplicationListener getApplicationListener();

        IGraphics getGraphics();

        IAudio getAudio();

        IInput getInput();

        IFiles getFiles();

        INet getNet();

        void log(string tag, string message);

        void log(string tag, string message, Exception exception);

        void error(string tag, string message);

        void error(string tag, string message, Exception exception);

        void debug(string tag, string message);

        void debug(string tag, string message, Exception exception);

        void setLogLevel(int logLevel);

        int getLogLevel();

        void setApplicationLogger(IApplicationLogger applicationLogger);

        IApplicationLogger getApplicationLogger();

        ApplicationType getType();

        int getVersion();

        long getJavaHeap();

        long getNativeHeap();

        IPreferences getPreferences(string name);

        IClipboard getClipboard();

        void postRunnable(Runnable runnable);

        void exit();

        void addLifecycleListener(ILifecycleListener listener);

        void removeLifecycleListener(ILifecycleListener listener);
    }

    public delegate void Runnable();
}
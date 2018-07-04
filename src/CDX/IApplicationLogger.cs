using System;

namespace CDX
{
    public interface IApplicationLogger
    {
        void log(string tag, string message);

        void log(string tag, string message, Exception exception);

        void error(string tag, string message);

        void error(string tag, string message, Exception exception);

        void debug(string tag, string message);

        void debug(string tag, string message, Exception exception);
    }
}
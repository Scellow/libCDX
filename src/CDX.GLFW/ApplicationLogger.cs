using System;

namespace CDX.GLFWBackend
{
    public class ApplicationLogger : IApplicationLogger
    {
        public void log(string tag, string message)
        {
            Console.WriteLine($"[LOG] ({tag}) {message}");
        }

        public void log(string tag, string message, Exception exception)
        {
            Console.WriteLine($"[LOG] ({tag}) {message}");
            Console.WriteLine($"{exception}");
        }

        public void error(string tag, string message)
        {
            Console.WriteLine($"[ERROR] ({tag}) {message}");
        }

        public void error(string tag, string message, Exception exception)
        {
            Console.WriteLine($"[ERROR] ({tag}) {message}");
            Console.WriteLine($"{exception}");
        }

        public void debug(string tag, string message)
        {
            Console.WriteLine($"[DEBUG] ({tag}) {message}");
        }

        public void debug(string tag, string message, Exception exception)
        {
            Console.WriteLine($"[DEBUG] ({tag}) {message}");
            Console.WriteLine($"{exception}");
        }
    }
}
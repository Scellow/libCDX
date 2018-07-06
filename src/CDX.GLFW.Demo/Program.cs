using System;
using CDX.Graphics;

namespace CDX.GLFWBackend.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ApplicationConfiguration();
            config.useVsync(true);
            config.setIdleFPS(1);
            config.setInitialBackgroundColor(Color.BLACK);

            //var app = new Application(new TestModel(), config);
            var app = new Application(new TestMesh(), config);
        }
    }
}
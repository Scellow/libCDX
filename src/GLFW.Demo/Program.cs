using System;
using System.IO;

using GLFW;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GLFW.Demo
{
    class Example
    {
        static void Main(string[] args)
        {
            // If the library isn't in the environment path we need to set it
            
            //Glfw.ConfigureNativesDirectory("./");

            // Initialize the library


            var data = new int[] {1, 2};
            unsafe
            {
                
   
            }

            if (!GLFW.Init()) 
            {
                Console.Error.WriteLine("ERROR: Could not initialize GLFW, shutting down.");
                Console.WriteLine(Directory.GetCurrentDirectory());
                GLFW.Terminate();
                Environment.Exit(1);
            }
        
            
            GLFW.WindowHint(GLFW.Hint.ContextVersionMajor, 4);
            GLFW.WindowHint(GLFW.Hint.ContextVersionMinor, 1);
            

            // Create a windowed mode window and its OpenGL context
            var window = GLFW.CreateWindow(640, 480, "Hello World");
            if (!window)
            {
                Console.Error.WriteLine("ERROR: Could not initialize GLFW window, shutting down.");
                Environment.Exit(1);
                GLFW.Terminate();
            }

            // Make the window's context current
            GLFW.MakeContextCurrent(window);
    
            Toolkit.Init();
            var context = new GraphicsContext(new ContextHandle(IntPtr.Zero), null);
            
            // Loop until the user closes the window
            while (!GLFW.WindowShouldClose(window))
            {
           
                // Render
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Begin(PrimitiveType.Triangles);
                GL.Color3(1.0f, 0.0f, 0.0f); GL.Vertex2(0.0f, 0.0f);
                GL.Color3(0.0f, 1.0f, 0.0f); GL.Vertex2(0.5f, 1.0f);
                GL.Color3(0.0f, 0.0f, 1.0f); GL.Vertex2(1.0f, 0.0f);
                GL.End();

                // Swap front and back buffers
                GLFW.SwapBuffers(window);
                
                // Poll for and process events
                GLFW.PollEvents();
            }
            
            GLFW.Terminate();
        }
    }
}

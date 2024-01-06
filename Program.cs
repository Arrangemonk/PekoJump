// This file is part of Silk.NET.
//
// You may modify and distribute Silk.NET under the terms
// of the MIT license. See the LICENSE file for details.

using System.Drawing;
using System.Numerics;
using ImGuiNET;
using Silk.NET.Windowing;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Vulkan;
//using Silk.NET.OpenGL.Extensions.ImGui;
using Gui = ImGuiNET.ImGui;

namespace ImGui
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a Silk.NET window as usual
            using var window = Window.Create(WindowOptions.Default);

            // Declare some variables
            Silk.NET.OpenGL.Extensions.ImGui.ImGuiController controller = null;
            GL gl = null;
            IInputContext inputContext = null;
            Vector4 myColor = new Vector4(1);

            // Our loading function
            window.Load += () =>
            {
                controller = new Silk.NET.OpenGL.Extensions.ImGui.ImGuiController(
                    gl = window.CreateOpenGL(), // load OpenGL
                    window, // pass in our window
                    inputContext = window.CreateInput() // create an input context
                );
            };

            // Handle resizes
            window.FramebufferResize += s =>
            {
                // Adjust the viewport to the new window size
                gl.Viewport(s);
            };

            // The render function
            window.Render += delta =>
            {
                // Make sure ImGui is up-to-date
                controller.Update((float)delta);

                // This is where you'll do any rendering beneath the ImGui context
                // Here, we just have a blank screen.
                gl.ClearColor(Color.FromArgb(255, (int)(.45f * 255), (int)(.55f * 255), (int)(.60f * 255)));
                gl.Clear((uint)ClearBufferMask.ColorBufferBit);

                // This is where you'll do all of your ImGUi rendering
                // Here, we're just showing the ImGui built-in demo window.
                //ImGuiNET.ImGui.ShowDemoWindow();
                //Gui.Begin("mainmenu", );
                Gui.BeginMainMenuBar();

                Gui.BeginMenu("File");
                Gui.MenuItem("open..", "ctrl+O");
                Gui.MenuItem("Save..", "ctrl+S");
                Gui.MenuItem("Close..", "ctrl+w");
                Gui.EndMenu();

                Gui.EndMainMenuBar();
                //Gui.End();

                Gui.Begin("Tool");

                Gui.ColorEdit4("Color", ref myColor);
                if (Gui.Button("Spam"))
                {
                    Console.WriteLine("spam!");
                }

                Gui.End();

                // Make sure ImGui renders too!
                controller.Render();
            };

            // The closing function
            window.Closing += () =>
            {
                // Dispose our controller first
                controller?.Dispose();

                // Dispose the input context
                inputContext?.Dispose();

                // Unload OpenGL
                gl?.Dispose();
            };

            // Now that everything's defined, let's run this bad boy!
            window.Run();

            window.Dispose();
        }
    }
}
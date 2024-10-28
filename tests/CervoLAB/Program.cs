// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Cervo;
using Cervo.Data;
using Cervo.Elements;
using Cervo.Platform.Windows;
using Cervo.Type.Enum;
using Mochi.DearImGui;

namespace CervoLAB;

internal static class Program
{
    // ReSharper disable once InconsistentNaming
    private static void Main()
    {
        Window window = new Window();
        window.Create(new WindowOptions
        {
            Title = "CervoLAB",
            Size = new Size(800, 500),
            MinSize = new Size(400, 250),
            BackendApi = BackendApi.DirectX9,
            StartPosition = new Point(-1, -1),
            AllowResize = true
        }, OnRender);
        //TestPage.Initialize();
        window.Render();
    }

    private static void OnRender()
    {
        //using var test = new Div().Bg(Color.FromArgb(255, 255, 255)).Flex().SizeFull();
        /*using (var page = new Div().Bg(Color.FromArgb(255, 255, 255)).Flex().SizeFull())
        {
            using (var header = new Div().Bg(Color.FromArgb(0, 0, 0)).Size(new System.Numerics.Vector2(800, 50)))
            {
                ImGui.Text("Header");
            }
            using (var content = new Div().Bg(Color.FromArgb(255, 255, 255)).Flex())
            {
                ImGui.Text("Content");
            }
        }*/
    }
}
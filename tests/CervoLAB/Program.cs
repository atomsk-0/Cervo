// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Cervo;
using Cervo.Data;
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
            BackendApi = BackendApi.OpenGL,
            StartPosition = new Point(-1, -1),
            AllowResize = true
        }, () => {});
        window.Render();
    }
}
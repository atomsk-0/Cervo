// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Cervo;
using Cervo.Components;
using Cervo.Components.Internal;
using Cervo.Data;
using Cervo.Data.Style;
using Cervo.Platform.Windows;
using Cervo.Type.Enum;
using Mochi.DearImGui;

namespace CervoLAB;

internal static unsafe class Program
{
    private static NormalButtonStyle buttonStyle = new NormalButtonStyle
    {
        Padding = new Padding(10),
        Font = ImGui.GetFont(),
        TextAlign = TextAlign.Center,
        Display = Display.Flex,

        BorderThickness = 1,
        Radius = 3,
        FadeinSpeed = 400,
        FadeoutSpeed = 400,

        BackgroundColor = Color.FromArgb(62, 62, 62),
        BackgroundHoverColor = Color.FromArgb(82, 82, 82),
        BackgroundActiveColor = Color.FromArgb(22, 22, 22),
        BackgroundDisabledColor = Color.FromArgb(12, 12, 12),

        TextColor = Color.White,
        TextHoverColor = Color.White,
        TextActiveColor = Color.White,
        TextDisabledColor = Color.FromArgb(128, 128, 128),

        BorderColor = Color.FromArgb(50, 50, 50),
        BorderHoverColor = Color.FromArgb(50, 50, 50),
        BorderActiveColor = Color.FromArgb(50, 50, 50),
        BorderDisabledColor = Color.FromArgb(50, 50, 50)
    };

    // ReSharper disable once InconsistentNaming
    private static void Main()
    {
        Titlebar.SetStyle(new TitlebarStyle
        {
            Height = 40,
            BackgroundColor = Color.FromArgb(32, 32, 32),
            BorderColor = Color.FromArgb(50, 50, 50),
            BorderThickness = 1
        });
        Window window = new Window();
        window.Create(new WindowOptions
        {
            Title = "CervoLAB",
            Size = new Size(800, 500),
            MinSize = new Size(400, 250),
            BackendApi = BackendApi.DirectX11,
            StartPosition = new Point(-1, -1),
            AllowResize = true,
            NativeBorders = true
        }, () =>
        {
            Button.Normal("test", "Test Button", buttonStyle);
        });
        window.Render();
    }
}
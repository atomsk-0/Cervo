// See https://aka.ms/new-console-template for more information

using Cervo.Data;
using Cervo.Platform.Windows;

W32Window window = new W32Window();
window.Create(new WindowOptions
{
    Title = "CervoLAB",
    Width = 800,
    Height = 600
});
window.Render();
using System.Drawing;
using Cervo.Data;
using Cervo.Platform.Windows;
using Cervo.Type.Enum;

namespace Cervo.Tests.Backends;

public class D3D11BackendTests
{
    private Window window;

    [OneTimeSetUp]
    public void Setup()
    {
        window = new Window();
        window.Create(new WindowOptions
        {
            Title = "D3D11Test",
            Size = new Size(800, 500),
            MinSize = new Size(0, 0),
            BackendApi = BackendApi.DirectX11,
            StartPosition = new Point(-1, -1),
        }, () => {});
    }

    [Test]
    public void LoadTextureFromFile()
    {
        Assert.That(window.Backend.TryLoadTextureFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.png"), out _), Is.True);
    }

    [Test]
    public unsafe void LoadTextureFromMemory()
    {
        byte[] data = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.png"));
        fixed (byte* pData = data)
        {
            Assert.That(window.Backend.TryLoadTextureFromMemory(pData, 0, 0, (nuint)data.Length, out _), Is.True);
        }
    }

    [Test]
    public void LoadTextureFromMemoryStream()
    {
        MemoryStream stream = new(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.png")));
        Assert.That(window.Backend.TryLoadTextureFromMemory(stream, 0, 0, out _), Is.True);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        window.Destroy();
    }
}
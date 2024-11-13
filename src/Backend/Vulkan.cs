using System.Drawing;
using Cervo.Data;
using Cervo.Type.Interface;

namespace Cervo.Backend;

// Not sure if this will stay/be done, but if it does probably will replace OpenGL

public class Vulkan : IBackend
{
    public bool Setup(IWindow window)
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public void Render()
    {
        throw new NotImplementedException();
    }

    public void Destroy()
    {
        throw new NotImplementedException();
    }

    public void OnResize(int width, int height)
    {
        throw new NotImplementedException();
    }

    public Action OnRender { get; set; }
    public bool TryLoadTextureFromFile(string path, out Texture texture)
    {
        throw new NotImplementedException();
    }

    public bool TryLoadTextureFromMemory(in MemoryStream stream, uint width, uint height, out Texture texture)
    {
        throw new NotImplementedException();
    }

    public unsafe bool TryLoadTextureFromMemory(byte* data, uint width, uint height, UIntPtr length, out Texture texture)
    {
        throw new NotImplementedException();
    }

    public Size GetViewportSize()
    {
        throw new NotImplementedException();
    }
}
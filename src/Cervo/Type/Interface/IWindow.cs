using Cervo.Data;

namespace Cervo.Type.Interface;

public interface IWindow
{
    public void Create(in WindowOptions options);

    public void Render();

    public void Destroy();

    public IntPtr GetHandle();
}
using System.Drawing;
using Cervo.Data;

namespace Cervo.Type.Interface;

public interface IWindow
{
    public IBackend Backend { get; set; }

    public void Create(in WindowOptions options, Action onRender);

    public void Render();

    public void Destroy();

    public IntPtr GetHandle();

    public Size GetSize();
}
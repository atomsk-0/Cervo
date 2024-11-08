using System.Drawing;
using Cervo.Data;

namespace Cervo.Type.Interface;

public interface IWindow
{
    public IBackend Backend { get; set; }

    public void Create(in WindowOptions options, Action onRender);

    public void Render();

    public void Destroy();

    public bool IsMinimized();

    public bool IsMaximized();

    public void Minimize();

    public void Maximize();

    public void Restore();

    public void Show();

    public void Hide();

    public void Close();

    public bool CanResize();

    public void DragWindow();

    public IntPtr GetHandle();

    public WindowOptions GetOptions();

    public Size GetSize();
}
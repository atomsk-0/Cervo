using System.Drawing;
using System.Numerics;
using Cervo.Type.Enum;

namespace Cervo.Type.Interface;

public abstract class Element()
{
    public Element? Parent { get; set; }
    public Vector2 LocalSize { get; set; }

    private readonly string? id = null;
    internal abstract void Render();
    internal abstract void SetBg(Color color);
    internal abstract void SetDisplay(Display display);

    internal void SetSize(Vector2 size)
    {
        LocalSize = size;
    }

    public abstract void Child(Element element);

    public string? GetId() => id;
}
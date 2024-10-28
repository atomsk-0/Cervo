using System.Drawing;
using System.Numerics;
using Cervo.Type.Enum;
using Cervo.Type.Interface;
using Cervo.Util;
using Mochi.DearImGui;
using Mochi.DearImGui.Internal;

namespace Cervo.Elements;

public unsafe class Div : Element, IDisposable
{
    public Div()
    {
        Cervo.AddElement(this);
    }

    private Color backgroundColor = Color.Transparent;
    internal override void Render()
    {
        ImGuiWindow* window = ImGuiInternal.GetCurrentWindow();
        var pos = window->DC.CursorPos;
        ImRect rect = new(pos.X, pos.Y, pos.X + LocalSize.X, pos.Y + LocalSize.Y);
        window->DrawList->AddRectFilled(rect.Min, rect.Max, backgroundColor.ToUint32());
    }

    internal override void SetBg(Color color)
    {
        backgroundColor = color;
    }

    internal override void SetDisplay(Display display)
    {

    }

    public override void Child(Element element)
    {

    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    ~Div()
    {
        Dispose();
    }
}
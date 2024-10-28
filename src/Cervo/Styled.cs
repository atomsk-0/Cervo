using System.Drawing;
using System.Numerics;
using Cervo.Elements;
using Cervo.Type.Enum;
using Cervo.Type.Interface;
using Mochi.DearImGui;

namespace Cervo;

public static class Styled
{
    public static T Flex<T>(this T element) where T: Element
    {
        element.SetDisplay(Display.Flex);
        return element;
    }
    public static T Bg<T>(this T element, Color color) where T: Element
    {
        element.SetBg(color);
        return element;
    }

    public static T SizeFull<T>(this T element) where T: Element
    {
        Vector2 newSize = Vector2.Zero;
        if (element.Parent != null)
        {
            newSize = element.Parent.LocalSize;
        }
        else if (Cervo.CurrentWindow != null)
        {
            var viewportSize = Cervo.CurrentWindow.Backend.GetViewportSize();
            newSize = new Vector2(viewportSize.Width, viewportSize.Height);
        }
        element.SetSize(newSize);
        return element;
    }
}
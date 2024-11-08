using System.Numerics;
using Cervo.Data.Style;
using Cervo.Util;
using Mochi.DearImGui;
using Mochi.DearImGui.Internal;

namespace Cervo.Components.Internal;

public static unsafe class Titlebar
{
    private static TitlebarStyle style;

    internal static bool IsDragging;

    internal static void WindowsTitlebar()
    {
        ImGuiWindow* window = ImGuiInternal.GetCurrentWindow();
        //if (window->SkipItems) return;

        ImRect titlebarRect = new ImRect(window->Pos, window->Pos + new Vector2(window->Size.X, style.Height));
        window->DrawList->AddRectFilled(titlebarRect.Min, titlebarRect.Max, style.BackgroundColor.ToUint32());

        if (style.BorderThickness > 0)
        {
            ImRect borderRect = new ImRect(titlebarRect.Min with{Y = titlebarRect.Max.Y}, titlebarRect.Max);
            window->DrawList->AddLine(borderRect.Min, borderRect.Max, style.BorderColor.ToUint32(), style.BorderThickness);
        }

        uint titlebarId = ImGui.GetID("title_bar");
        ImGui.SetNextItemAllowOverlap();
        ImGuiInternal.ItemSize(titlebarRect, 0);
        //if (ImGuiInternal.ItemAdd(titlebarRect, titlebarId) == false) return;

        bool hovered, held;
        ImGuiInternal.ButtonBehavior(titlebarRect, titlebarId, &hovered, &held);
        if (hovered && held && IsDragging == false)
        {
            IsDragging = true;
        }
        else if (held == false)
        {
            IsDragging = false;
        }
    }

    public static void SetStyle(in TitlebarStyle newStyle)
    {
        style = newStyle;
    }
}
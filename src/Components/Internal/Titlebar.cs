using System.Drawing;
using System.Numerics;
using Cervo.Data.Style;
using Cervo.Type.Interface;
using Cervo.Util;
using Mochi.DearImGui;
using Mochi.DearImGui.Internal;

namespace Cervo.Components.Internal;

public static unsafe class Titlebar
{
    private static TitlebarStyle style;

    internal static bool IsDragging;

    internal static void WindowsTitlebar(IWindow context)
    {
        ImGuiWindow* window = ImGuiInternal.GetCurrentWindow();
        if (window->SkipItems) return;

        float yPoint = OperatingSystem.IsWindows() && context.IsMaximized() ? Platform.Windows.Manager.MAXIMIZED_PADDING.Y : 0;

        Vector2 pos = new Vector2(0, yPoint);

        ImRect titlebarRect = new ImRect(pos, pos + new Vector2(window->Size.X, style.Height));
        window->DrawList->AddRectFilled(titlebarRect.Min, titlebarRect.Max, style.BackgroundColor.ToUint32());

        if (style.BorderThickness > 0)
        {
            ImRect borderRect = new ImRect(titlebarRect.Min with{Y = titlebarRect.Max.Y}, titlebarRect.Max);
            window->DrawList->AddLine(borderRect.Min, borderRect.Max, style.BorderColor.ToUint32(), style.BorderThickness);
        }

        ImGui.SetCursorPosX(window->Size.X - titlebarRect.Max.Y);
        if (windowsTitlebarButton("close_caption_btn", Platform.Windows.Manager.CLOSE_ICON, false, new Vector2(style.Height)))
        {
            context.Close();
        }
        ImGui.SameLine();
        ImGui.SetCursorPosX(window->Size.X - titlebarRect.Max.Y * 2);
        if (windowsTitlebarButton("toggle_state_caption_btn", context.IsMaximized() ? Platform.Windows.Manager.RESTORE_ICON : Platform.Windows.Manager.MAXIMIZE_ICON, false, new Vector2(style.Height)))
        {
            if (context.IsMaximized())
                context.Restore();
            else
                context.Maximize();
        }
        ImGui.SameLine();
        ImGui.SetCursorPosX(window->Size.X - titlebarRect.Max.Y * 3);
        if (windowsTitlebarButton("minimize_caption_btn", Platform.Windows.Manager.MINIMIZE_ICON, false, new Vector2(style.Height)))
        {
            context.Minimize();
        }

        uint titlebarId = ImGui.GetID("title_bar");
        ImGui.SetNextItemAllowOverlap();
        ImGuiInternal.ItemSize(titlebarRect, 0);
        if (ImGuiInternal.ItemAdd(titlebarRect, titlebarId) == false) return;

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

    internal static float GetHeight(IWindow context)
    {
        float height = style.Height + style.BorderThickness;
        if (OperatingSystem.IsWindows() && context.IsMaximized())
        {
            height += Platform.Windows.Manager.MAXIMIZED_PADDING.Y;
        }
        return height;
    }

    public static void SetStyle(in TitlebarStyle newStyle)
    {
        style = newStyle;
    }


    private static bool windowsTitlebarButton(string id, string icon, bool disabled, Vector2 size)
    {
        ImGuiWindow* window = ImGuiInternal.GetCurrentWindow();
        if (window->SkipItems) return false;

        ImGuiContext* g = *ImGuiInternal.GImGui;
        uint uId = window->GetID(id);
        Vector2 position = window->DC.CursorPos;

        ImGui.PushFont(Platform.Windows.Manager.SystemFont.GetImFont());
        Vector2 labelSize = ImGui.CalcTextSize(icon);

        Vector2 buttonSize = size;

        if (size != default) buttonSize = size;
        ImRect rect = new ImRect(position, position + buttonSize);

        ImGuiInternal.ItemSize(size, 0);
        if (ImGuiInternal.ItemAdd(rect, uId) == false)
        {
            ImGui.PopFont();
            return false;
        }

        bool hovered, held;
        bool pressed = ImGuiInternal.ButtonBehavior(rect, uId, &hovered, &held);

        if (disabled)
        {
            hovered = hovered = pressed = false;
        }

        Color backgroundColor = Color.Transparent;
        Color iconColor = Color.White;

        if (hovered)
        {
            backgroundColor = icon == Platform.Windows.Manager.CLOSE_ICON ? Color.FromArgb(235, 64, 52) : Color.FromArgb(10, 255, 255, 255);
        }

        window->DrawList->AddRectFilled(rect.Min, rect.Max, backgroundColor.ToUint32(), 0f);
        Vector2 centerIconPos = new Vector2(rect.Min.X + (rect.Max.X - rect.Min.X) / 2 - labelSize.X / 2, rect.Min.Y + (rect.Max.Y - rect.Min.Y) / 2 - labelSize.Y / 2);
        window->DrawList->AddText(centerIconPos, iconColor.ToUint32(), icon);

        ImGui.PopFont();

        return pressed;
    }
}
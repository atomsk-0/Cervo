using System.Numerics;
using Cervo.Data.Style;
using Cervo.Type.Enum;
using Cervo.Util;
using Mochi.DearImGui;
using Mochi.DearImGui.Internal;

namespace Cervo.Components;

public static unsafe class Panel
{
    public static void BeginNormal(string id, in NormalPanelStyle style, Vector2 size = default)
    {
        ImGui.PushStyleColor(ImGuiCol.ChildBg, style.BackgroundColor.ToVector4());
        ImGui.PushStyleColor(ImGuiCol.Border, style.BorderColor.ToVector4());
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, style.BorderThickness);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, style.Padding);
        ImGuiWindow* parentWindow = ImGuiInternal.GetCurrentWindow();
        if (style.Display == Display.Flex)
        {
            size.X = parentWindow->Size.X - parentWindow->WindowPadding.X * 2;
        }
        else if (style.Display == Display.Fill)
        {
            size.X = parentWindow->Size.X - parentWindow->WindowPadding.X * 2;
            size.Y = parentWindow->Size.Y - parentWindow->WindowPadding.Y * 2;
        }
        ImGui.BeginChild(id, size, ImGuiChildFlags.Borders | ImGuiChildFlags.AlwaysUseWindowPadding);
    }

    public static void End()
    {
        ImGui.EndChild();
        ImGui.PopStyleVar(2);
        ImGui.PopStyleColor(2);
    }
}
using System.Drawing;
using System.Numerics;
using Mochi.DearImGui;

namespace Cervo.Util;

public static unsafe class ImUtils
{
    /* Internal Stuff */
    internal static void SetupImGui()
    {
        ImGui.CreateContext();

        ImGuiIO* io = ImGui.GetIO();
        io->IniFilename = null;
        io->WantSaveIniSettings = false;

        ImGuiStyle* style = ImGui.GetStyle();
        style->Colors[(int)ImGuiCol.WindowBg] = Color.FromArgb(0, 0, 0, 0).ToVector4();
        style->WindowRounding = 0;
        style->WindowBorderSize = 0;
        style->WindowPadding = new Vector2(0, 0);
        style->FramePadding = new Vector2(0, 0);
        style->ItemSpacing = new Vector2(0, 0);
    }

    /* Public Stuff */

    public static Vector4 ToVector4(this Color color)
    {
        return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
    }

    public static Vector4 ToVector4(this Color color, byte alpha)
    {
        return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, alpha / 255f);
    }

    public static uint ToUint32(this Color color)
    {
        return ImGui.GetColorU32(color.ToVector4());
    }

    public static uint ToUint32(this Color color, byte alpha)
    {
        return ImGui.GetColorU32(color.ToVector4(alpha));
    }
}
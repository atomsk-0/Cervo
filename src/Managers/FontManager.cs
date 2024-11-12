using Cervo.Data;
using Mochi.DearImGui;

namespace Cervo.Managers;

public static unsafe class FontManager
{
    private static readonly List<Font> fonts_to_load = [];

    public static void AddFont(in Font font)
    {
        fonts_to_load.Add(font);
    }

    internal static void LoadFonts()
    {
        ImGuiIO* io = ImGui.GetIO();
        foreach (var font in fonts_to_load)
        {
            char* ranges = null;
            if (font.Ranges != null)
            {
                fixed (char* pRanges = font.Ranges)
                {
                    ranges = pRanges;
                }
            }
            font.ImFont = io->Fonts->AddFontFromFileTTF(font.Path, font.Size, font.Config, ranges);
        }
    }
}
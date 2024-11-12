﻿using Mochi.DearImGui;

namespace Cervo.Data;

public unsafe class Font
{
    public string Path { get; set; }
    public float Size { get; set; }
    internal char[]? Ranges { get; set; }
    internal ImFontConfig* Config { get; set; }

    internal ImFont* ImFont { get; set; }

    internal ImFont* GetImFont()
    {
        return ImFont == null ? ImGui.GetFont() : ImFont;
    }

    public Font(string path, float size)
    {
        Path = path;
        Size = size;
    }

    public Font(string path, float size, char[] ranges)
    {
        Path = path;
        Size = size;
        Ranges = ranges;
    }

    public Font(string path, float size, ImFontConfig* config)
    {
        Path = path;
        Size = size;
        Config = config;
    }
}
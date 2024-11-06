using Mochi.DearImGui;

namespace Cervo.Managers;

//TODO: Write tests and finish summary documentation

public static unsafe class FontManager
{
    private static readonly Dictionary<string, nint> font_handles = [];

    /// <summary>
    /// Add TTF/OTF font from file, with size
    /// </summary>
    /// <param name="id">id for font</param>
    /// <param name="path">file path</param>
    /// <param name="size">font size in pixels (recommended to round down to nearest integer)</param>
    /// <exception cref="ArgumentException">Thrown when a font with the specified id already exists.</exception>
    public static void AddFont(string id, string path, float size)
    {
        ImGuiIO* io = ImGui.GetIO();
        ImFont* font = io->Fonts->AddFontFromFileTTF(path, size);
        if (font_handles.TryAdd(id, (nint)font) == false)
        {
            throw new ArgumentException($"Font with id {id} already exists");
        }
    }

    public static void AddFont(string id, string path, float size, char[] iconRanges)
    {
        fixed (char* pIconRanges = iconRanges)
        {
            ImGuiIO* io = ImGui.GetIO();
            ImFont* font = io->Fonts->AddFontFromFileTTF(path, size, null, pIconRanges);
            if (font_handles.TryAdd(id, (nint)font) == false)
            {
                throw new ArgumentException($"Font with id {id} already exists");
            }
        }
    }

    public static void AddFont(string id, string path, float size, ImFontConfig* config)
    {
        ImGuiIO* io = ImGui.GetIO();
        ImFont* font = io->Fonts->AddFontFromFileTTF(path, size, config);
        if (font_handles.TryAdd(id, (nint)font) == false)
        {
            throw new ArgumentException($"Font with id {id} already exists");
        }
    }

    public static void AddFont(string id, string path, float size, ImFontConfig* config, char[] iconRanges)
    {
        fixed (char* pIconRanges = iconRanges)
        {
            ImGuiIO* io = ImGui.GetIO();
            ImFont* font = io->Fonts->AddFontFromFileTTF(path, size, config, pIconRanges);
            if (font_handles.TryAdd(id, (nint)font) == false)
            {
                throw new ArgumentException($"Font with id {id} already exists");
            }
        }
    }

    /// <summary>
    /// ID of the default font
    /// </summary>
    /// <param name="id">font id</param>
    /// <exception cref="ArgumentException">Thrown when a font with the id doesn't exist</exception>
    public static void SetDefaultFont(string id)
    {
        if (font_handles.TryGetValue(id, out nint font) == false)
        {
            throw new ArgumentException($"Font with id {id} does not exist");
        }
        ImGuiIO* io = ImGui.GetIO();
        io->FontDefault = (ImFont*)font;
    }

    /// <summary>
    /// Get ImFont* of given font id
    /// </summary>
    /// <param name="id">font id</param>
    /// <returns>ImFont* of given font id</returns>
    /// <exception cref="ArgumentException">Thrown when a font with the id doesn't exist</exception>
    public static ImFont* GetFont(string id)
    {
        if (font_handles.TryGetValue(id, out nint font) == false)
        {
            throw new ArgumentException($"Font with id {id} does not exist");
        }
        return (ImFont*)font;
    }
}
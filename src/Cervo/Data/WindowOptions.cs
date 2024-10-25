using Cervo.Type.Enum;

namespace Cervo.Data;

public struct WindowOptions
{
    public string Title { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }

    public BackendApi BackendApi { get; set; }
}
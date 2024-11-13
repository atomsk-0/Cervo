using System.Drawing;
using Cervo.Type.Enum;

namespace Cervo.Data;

public struct WindowOptions
{
    /// <summary>
    /// Window Title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Window initial size
    /// </summary>
    public Size Size { get; set; }

    /// <summary>
    /// Minimum window size
    /// </summary>
    public Size MinSize { get; set; }

    /// <summary>
    /// Start position of the window (-1, -1) will center the window
    /// </summary>
    public Point StartPosition { get; set; }

    /// <summary>
    /// If set to true, the window allows transparent background
    /// </summary>
    public bool Transparent { get; set; } //TODO: Implement

    /// <summary>
    /// If set to true, the window will be resizable
    /// </summary>
    public bool AllowResize { get; set; }

    /// <summary>
    /// If set to true, the window has a native stroke borders
    /// </summary>
    public bool NativeBorders { get; set; }

    /// <summary>
    /// If set to true, the window will have no titlebar
    /// </summary>
    public bool NoTitlebar { get; set; } //TODO: Implement

    /// <summary>
    /// If set to true, the window will use the native titlebar (not recommended to be true) unless need because of legacy reasons
    /// </summary>
    public bool NativeTitlebar { get; set; } //TODO: Implement

    /// <summary>
    /// Backend API to use
    /// </summary>
    public BackendApi BackendApi { get; set; }
}
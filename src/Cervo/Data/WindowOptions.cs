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
    /// If set to true, the window "background" will be transparent
    /// </summary>
    public bool Transparent { get; set; }

    /// <summary>
    /// If set to true, the window will be resizable
    /// </summary>
    public bool AllowResize { get; set; }

    /// <summary>
    /// Backend API to use
    /// </summary>
    public BackendApi BackendApi { get; set; }
}
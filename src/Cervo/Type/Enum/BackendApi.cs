using System.Diagnostics.CodeAnalysis;

namespace Cervo.Type.Enum;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum BackendApi
{
    /// <summary>
    ///  Should be default for Windows if DX11 is not supported
    /// </summary>
    DirectX9,
    /// <summary>
    /// Should be default for Windows and use DX9 as fallback
    /// </summary>
    DirectX11,
    /// <summary>
    /// Default for MacOS
    /// </summary>
    Metal,
    /// <summary>
    /// Default for Linux and optionally for Windows and MacOS
    /// </summary>
    OpenGL
}
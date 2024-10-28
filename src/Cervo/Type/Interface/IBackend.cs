using System.Drawing;
using Cervo.Data;
using TerraFX.Interop.DirectX;

namespace Cervo.Type.Interface;

public interface IBackend
{
    /// <summary>
    /// Set up the backend
    /// </summary>
    /// <param name="window">instance of window interface to initialize for</param>
    /// <returns>true if no problems occurred</returns>
    internal bool Setup(IWindow window);

    /// <summary>
    /// Reset the backend (if possible)
    /// </summary>
    internal void Reset();

    /// <summary>
    /// Render the backend
    /// </summary>
    internal void Render();

    /// <summary>
    /// Destroy the backend (release resources)
    /// </summary>
    internal void Destroy();

    internal void OnResize(int width, int height);
    internal Action OnRender { get; set; }

    /* Util methods */

    /// <summary>
    /// Requests the backend to try load a texture from a file
    /// </summary>
    /// <param name="path">Path to texture file</param>
    /// <param name="texture">Outputs texture struct</param>
    /// <returns>true if backend loaded texture successfully</returns>
    public bool TryLoadTextureFromFile(string path, out Texture texture);
    // TODO: Add load texture from memory method

    internal Size GetViewportSize();
}
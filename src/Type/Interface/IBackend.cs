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

    /// <summary>
    /// [Internal] Set backend buffer width and height
    /// </summary>
    /// <param name="width">new width</param>
    /// <param name="height">new height</param>
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

    /// <summary>
    /// Requests the backend to try load a texture from a memory stream
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="texture"></param>
    /// <returns></returns>
    public bool TryLoadTextureFromMemory(in MemoryStream stream, uint width, uint height, out Texture texture);

    /// <summary>
    /// Requests the backend to try load a texture from memory
    /// </summary>
    /// <param name="data"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="length"></param>
    /// <param name="texture"></param>
    /// <returns></returns>
    public unsafe bool TryLoadTextureFromMemory(byte* data, uint width, uint height, nuint length, out Texture texture);

    /// <summary>
    /// Get the viewport size of the backend (Includes part of titlebar too as it's part of the window viewport)
    /// </summary>
    /// <returns></returns>
    internal Size GetViewportSize();
}
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
    public bool Setup(IWindow window);

    /// <summary>
    /// Reset the backend (if possible)
    /// </summary>
    public void Reset();

    /// <summary>
    /// Render the backend
    /// </summary>
    public void Render();

    /// <summary>
    /// Destroy the backend (release resources)
    /// </summary>
    public void Destroy();


    // Util methods
    public bool TryLoadTextureFromFile(string path, out Texture texture);
    // TODO: Add load texture from memory method
}
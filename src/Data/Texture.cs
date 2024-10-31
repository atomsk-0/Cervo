namespace Cervo.Data;

public readonly struct Texture(IntPtr handle, uint width, uint height)
{
    public readonly IntPtr Handle = handle;
    public readonly uint Width = width;
    public readonly uint Height = height;
}
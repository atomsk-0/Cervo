using System.Drawing;
using Cervo.Type.Interface;
using Cervo.Util;
using Hexa.NET.StbImage;
using Mochi.DearImGui;
using Mochi.DearImGui.Backends.Glfw;
using Mochi.DearImGui.Backends.OpenGL3;
using Mochi.DearImGui.Backends.Win32;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;
using TerraFX.Interop.Windows;
using GL = Silk.NET.OpenGL.GL;
using Texture = Cervo.Data.Texture;

#pragma warning disable CA1416 Disables warning -> "CA1416: Validate platform compatibility"

namespace Cervo.Backend;

public unsafe class OpenGl : IBackend
{
    private GL gl = null!;
    private HDC hdc;
    private HGLRC hrc;
    private IntPtr windowHandle;

    private uint backendWidth, backendHeight;

    public bool Setup(IWindow window)
    {
        gl = GL.GetApi(new DefaultNativeContext("opengl32"));
        //gl = new GL(new DefaultNativeContext("opengl32"));
        windowHandle = window.GetHandle();
        ImUtils.SetupImGui();
        if (OperatingSystem.IsWindows())
        {
            if (createDeviceWgl((HWND)windowHandle) == false)
            {
                cleanupDeviceWgl();
                return false;
            }
            Windows.wglMakeCurrent(hdc, hrc);
            Win32ImBackend.InitForOpenGL((void*)windowHandle);
        }
        else
        {
            GlfwImBackend.InitForOpenGL((GLFWwindow*)windowHandle, true);
        }

        OpenGL3ImBackend.Init();

        return true;
    }

    public void Reset() {} // Currently not needed for OpenGL

    public void Render()
    {
        OpenGL3ImBackend.NewFrame();
        if (OperatingSystem.IsWindows())
            Win32ImBackend.NewFrame();
        else
            GlfwImBackend.NewFrame();
        ImGui.NewFrame();

        ImGui.ShowDemoWindow();

        ImGui.Render();

        gl.Viewport(0, 0, backendWidth, backendHeight);
        gl.ClearColor(Color.Black);
        gl.Clear(ClearBufferMask.ColorBufferBit);
        OpenGL3ImBackend.RenderDrawData(ImGui.GetDrawData());

        if (OperatingSystem.IsWindows())
            Windows.SwapBuffers(hdc);

        // GLFW Window will do the swap buffers on linux and MacOS
    }

    public void Destroy()
    {
        OpenGL3ImBackend.Shutdown();
        if (OperatingSystem.IsWindows())
            Win32ImBackend.Shutdown();
        else
            GlfwImBackend.Shutdown();
        ImGui.DestroyContext();

        if (OperatingSystem.IsWindows() == false) return;

        cleanupDeviceWgl();
        Windows.wglDeleteContext(hrc);
    }

    public void OnResize(int width, int height)
    {
        backendWidth = (uint)width;
        backendHeight = (uint)height;
    }

    public Action OnRender { get; set; } = null!;

    //TODO: Test these methods
    public bool TryLoadTextureFromFile(string path, out Texture texture)
    {
        int width, height, channels;
        byte* data = StbImage.Load(path, &width, &height, &channels, 0);
        if (data == null)
        {
            texture = new Texture(0, 0, 0);
            return false;
        }

        return TryLoadTextureFromMemory(data, (uint)width, (uint)height, (nuint)(width * height * channels), out texture);
    }

    public bool TryLoadTextureFromMemory(in MemoryStream stream, uint width, uint height, out Texture texture)
    {
        fixed (byte* pData = stream.ToArray())
        {
            return TryLoadTextureFromMemory(pData, width, height, (uint)stream.Length, out texture);
        }
    }

    public bool TryLoadTextureFromMemory(byte* data, uint width, uint height, UIntPtr length, out Texture texture)
    {
        // Bind the texture
        uint glTexture = gl.GenTexture();
        gl.BindTexture(TextureTarget.Texture2D, glTexture);

        // Set the texture wrapping/filtering options
        gl.TextureParameterI(glTexture, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        gl.TextureParameterI(glTexture, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        gl.TextureParameterI(glTexture, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        gl.TextureParameterI(glTexture, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

        // Load the texture
        // TODO - For now format is hardcoded to RGBA, this should be optional in the future,
        gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
        gl.GenerateMipmap(TextureTarget.Texture2D);
        StbImage.ImageFree(data);

        texture = new Texture((nint)glTexture, width, height);

        return true;
    }


    public Size GetViewportSize()
    {
        return new Size((int)backendWidth, (int)backendHeight);
    }

    private bool createDeviceWgl(HWND hWnd)
    {
        hdc = Windows.GetDC(hWnd);
        PIXELFORMATDESCRIPTOR pfd = new PIXELFORMATDESCRIPTOR
        {
            nSize = (ushort)sizeof(PIXELFORMATDESCRIPTOR),
            nVersion = 1,
            dwFlags = PFD.PFD_DRAW_TO_WINDOW | PFD.PFD_SUPPORT_OPENGL | PFD.PFD_DOUBLEBUFFER,
            iPixelType = PFD.PFD_TYPE_RGBA,
            cColorBits = 32,
        };
        int pf = Windows.ChoosePixelFormat(hdc, &pfd);
        if (pf == 0) return false;
        if (Windows.SetPixelFormat(hdc, pf, &pfd) == false) return false;
        Windows.ReleaseDC(hWnd, hdc);
        hrc = Windows.wglCreateContext(hdc);
        return true;
    }

    private void cleanupDeviceWgl()
    {
        Windows.wglMakeCurrent(HDC.NULL, HGLRC.NULL);
        Windows.ReleaseDC((HWND)windowHandle, hdc);
    }
}
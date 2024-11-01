using System.Drawing;
using Cervo.Backend;
using Cervo.Data;
using Cervo.Type.Enum;
using Cervo.Type.Interface;
using Silk.NET.GLFW;

//TODO: Proper error handling
// Currently window (this) is untested.

namespace Cervo.Platform.Linux;

public unsafe class Window : IWindow
{
    public IBackend Backend { get; set; } = null!;

    private Glfw glfw = null!;
    private WindowOptions options;
    private WindowHandle* windowHandle;

    public void Create(in WindowOptions aOptions, Action onRender)
    {
        options = aOptions;
        if (OperatingSystem.IsLinux())
        {
            if (options.BackendApi is (BackendApi.DirectX11 or BackendApi.DirectX9 or BackendApi.Metal))
            {
                throw new PlatformNotSupportedException("DirectX11, DirectX9 and Metal are not supported on Linux");
            }
        }

        // Set backend api to given option
        switch (options.BackendApi)
        {
            case BackendApi.OpenGL:
                Backend = new OpenGl();
                break;
        }

        glfw = Glfw.GetApi();
        glfw.SetErrorCallback(onGlfwError);
        if (glfw.Init() == false)
        {
            throw new Exception("Failed to initialize GLFW");
        }

        windowHandle = glfw.CreateWindow(options.Size.Width, options.Size.Height, options.Title, null, null);
        glfw.MakeContextCurrent(windowHandle);
        glfw.SwapInterval(1);

        if (Backend.Setup(this) == false)
        {
            Console.WriteLine("Failed to setup backend");
        }

        Backend.OnRender = onRender;
    }


    private void onGlfwError(ErrorCode error, string description)
    {
        Console.WriteLine($"GLFW->{error}: {description}");
    }

    public void Render()
    {
        while (glfw.WindowShouldClose(windowHandle) == false)
        {
            glfw.PollEvents();
            if (glfw.GetWindowAttrib(windowHandle, WindowAttributeGetter.Iconified))
            {
                Thread.Sleep(10);
                continue;
            }

            Backend.Render();

            glfw.SwapBuffers(windowHandle);
        }
        Destroy();
    }

    public void Destroy()
    {
        Backend.Destroy();
        glfw.DestroyWindow(windowHandle);
        glfw.Terminate();
    }

    public IntPtr GetHandle()
    {
        return (IntPtr)windowHandle;
    }

    public Size GetSize()
    {
        glfw.GetWindowSize(windowHandle, out int width, out int height);
        return new Size(width, height);
    }
}
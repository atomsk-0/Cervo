using System.Runtime.InteropServices;
using Cervo.Backend;
using Cervo.Data;
using Cervo.Type.Enum;
using Cervo.Type.Interface;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

#pragma warning disable CA1416 Disables warning -> "CA1416: Validate platform compatibility"

namespace Cervo.Platform.Windows;

public unsafe partial class W32Window : IWindow
{
    #region Additional Imports
    private const string library = "Mochi.DearImGui.Native.dll";

    // ReSharper disable once InconsistentNaming
    [LibraryImport(library, EntryPoint = "ImGui_ImplWin32_WndProcHandler")]
    private static partial LRESULT ImGuiImplWin32WndProcHandler(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam);
    #endregion

    private const string window_class_name = "cervo::window";

    private IBackend backend;
    private WNDCLASSEXW wndClass;
    private HWND handle;

    private bool running;

    private delegate LRESULT WndProcDelegate(HWND window, uint msg, WPARAM wParam, LPARAM lParam);
    private WndProcDelegate managedWndProc = null!;

    /// <summary>
    /// Initializes a new win32 window.
    /// </summary>
    /// <param name="options">Window Options</param>
    public void Create(in WindowOptions options)
    {
        running = true;

        managedWndProc = wndProc;

        switch (options.BackendApi)
        {
            case BackendApi.DirectX9:
                backend = new D3D9();
                break;
        }

        // Register and create window
        fixed (char* pClassName = window_class_name)
        {
            var lWndClass = new WNDCLASSEXW
            {
                cbSize = (uint)sizeof(WNDCLASSEXW),
                style = CS.CS_CLASSDC,
                lpfnWndProc = (delegate* unmanaged<HWND, uint, WPARAM, LPARAM, LRESULT>)Marshal.GetFunctionPointerForDelegate(managedWndProc),
                hInstance = GetModuleHandleW(null),
                hCursor = HCURSOR.NULL,
                hbrBackground = HBRUSH.NULL,
                lpszClassName = pClassName
            };
            RegisterClassExW(&lWndClass);
            wndClass = lWndClass;

            fixed (char* pTitle = options.Title)
            {
                handle = CreateWindowExW(0, pClassName, pTitle, WS.WS_POPUP, 0, 0, options.Width, options.Height, HWND.NULL, HMENU.NULL, wndClass.hInstance, null);
            }
        }

        if (backend.Setup(this) == false)
        {
            //TODO: error handling
            return;
        }

        ShowWindow(handle, SW.SW_SHOWDEFAULT);
        UpdateWindow(handle);
    }


    public void Render()
    {
        while (running)
        {
            MSG msg;
            while (PeekMessageW(&msg, HWND.NULL, 0, 0, PM.PM_REMOVE))
            {
                TranslateMessage(&msg);
                DispatchMessageW(&msg);
                if (msg.message == WM.WM_QUIT) running = false;
            }
            if (running == false) break;
            backend.Render();
        }

        Destroy();
    }


    public void Destroy()
    {
        backend.Destroy();
        DestroyWindow(handle);
        UnregisterClassW(wndClass.lpszClassName, wndClass.hInstance);
    }

    public IntPtr GetHandle()
    {
        return handle;
    }

    private LRESULT wndProc(HWND window, uint msg, WPARAM wParam, LPARAM lParam)
    {
        if (ImGuiImplWin32WndProcHandler(window, msg, wParam, lParam) > 0) return TRUE;
        switch (msg)
        {
            case WM.WM_DESTROY:
            {
                PostQuitMessage(0);
                return FALSE;
            }
        }
        return DefWindowProcW(window, msg, wParam, lParam);
    }
}
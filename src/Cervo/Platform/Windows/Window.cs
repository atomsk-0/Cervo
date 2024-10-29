using System.Drawing;
using System.Runtime.InteropServices;
using Cervo.Backend;
using Cervo.Data;
using Cervo.Type.Enum;
using Cervo.Type.Interface;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

#pragma warning disable CA1416 Disables warning -> "CA1416: Validate platform compatibility"

namespace Cervo.Platform.Windows;

public unsafe partial class Window : IWindow
{
    #region Additional Imports
    private const string library = "Mochi.DearImGui.Native.dll";

    // ReSharper disable once InconsistentNaming
    [LibraryImport(library, EntryPoint = "ImGui_ImplWin32_WndProcHandler")]
    private static partial LRESULT ImGuiImplWin32WndProcHandler(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam);
    #endregion
    public IBackend Backend { get; set; } = null!;

    private const string window_class_name = "cervo::window";
    private const byte loop_timer_id = 1;
    private const byte border_width = 8;
    private WNDCLASSEXW wndClass;
    private HWND handle;

    private WindowOptions options;

    private bool running;

    private delegate LRESULT WndProcDelegate(HWND window, uint msg, WPARAM wParam, LPARAM lParam);
    private WndProcDelegate managedWndProc = null!;


    /// <summary>
    /// Initializes a new win32 window.
    /// </summary>
    /// <param name="aOptions">Window Options</param>
    /// <param name="onRender">Render func</param>
    public void Create(in WindowOptions aOptions, Action onRender)
    {
        running = true;
        options = aOptions;

        managedWndProc = wndProc;

        // Set backend api to given option
        switch (options.BackendApi)
        {
            case BackendApi.DirectX9:
                Backend = new D3D9();
                break;
            case BackendApi.DirectX11:
                Backend = new D3D11();
                break;
        }

        Manager.GetScreenSize(out int monitorViewWidth, out int monitorViewHeight);

        int x = options.StartPosition.X;
        int y = options.StartPosition.Y;

        // Center window if x and y are -1
        if (x == -1 && y == -1)
        {
            x = (monitorViewWidth - options.Size.Width) / 2;
            y = (monitorViewHeight - options.Size.Height) / 2;
        }

        // Check if window position is out of bounds and reset to 0 if so
        if (x > monitorViewWidth || x < 0) x = 0;
        if (y > monitorViewHeight || y < 0) y = 0;

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
                Console.WriteLine($"Creating window: {options.Title}, size: {options.Size.Width}x{options.Size.Height}, initial position: {x}, {y}");
                handle = CreateWindowExW(0, pClassName, pTitle, options.AllowResize ? WS.WS_OVERLAPPEDWINDOW : WS.WS_POPUP, x, y, options.Size.Width, options.Size.Height, HWND.NULL, HMENU.NULL, wndClass.hInstance, null);
            }
        }

        if (Backend.Setup(this) == false)
        {
            Console.WriteLine("Failed to setup backend");
            //TODO: error handling
            return;
        }

        Backend.OnRender = onRender;

        ShowWindow(handle, SW.SW_SHOWDEFAULT);
        UpdateWindow(handle);

        Cervo.CurrentWindow = this;
    }

    public void Render()
    {
        MSG msg;
        while (running)
        {
            if (PeekMessageW(&msg, HWND.NULL, 0, 0, PM.PM_REMOVE) != 0)
            {
                TranslateMessage(&msg);
                DispatchMessageW(&msg);
                if (msg.message == WM.WM_QUIT) running = false;
            }
            else
            {
                Backend.Render();
            }
        }
        Destroy();
    }

    public void Destroy()
    {
        Backend.Destroy();
        DestroyWindow(handle);
        UnregisterClassW(wndClass.lpszClassName, wndClass.hInstance);
    }

    public IntPtr GetHandle()
    {
        return handle;
    }

    public Size GetSize()
    {
        RECT rect;
        GetClientRect(handle, &rect);
        return new Size(rect.right - rect.left, rect.bottom - rect.top);
    }

    private LRESULT wndProc(HWND window, uint msg, WPARAM wParam, LPARAM lParam)
    {
        if (ImGuiImplWin32WndProcHandler(window, msg, wParam, lParam) > 0) return 1;
        switch (msg)
        {
            case WM.WM_SIZE:
            {
                if (wParam == SIZE_MINIMIZED) return 0;
                Backend.OnResize(LOWORD(lParam), HIWORD(lParam));
                return 0;
            }
            case WM.WM_GETMINMAXINFO:
            {
                MINMAXINFO* minmax = (MINMAXINFO*)lParam;
                minmax->ptMinTrackSize.x = options.MinSize.Width;
                minmax->ptMinTrackSize.y = options.MinSize.Height;
                return 0;
            }
            case WM.WM_NCCALCSIZE:
            {
                return 0;
            }
            case WM.WM_SYSCOMMAND:
            {
                if (wParam == SC.SC_MOVE || wParam == SC.SC_SIZE)
                {
                    nint style = GetWindowLongPtrW(handle, GWL.GWL_STYLE);
                    SetWindowLongPtrW(handle, GWL.GWL_STYLE, style | WS.WS_CAPTION);
                    DefWindowProcW(handle, msg, wParam, lParam);
                    SetWindowLongPtrW(handle, GWL.GWL_STYLE, style);
                    return 0;
                }
                if ((wParam & 0xFFF0) == SC.SC_KEYMENU) return 0; // Disable ALT application menu
                break;
            }
            case WM.WM_ENTERSIZEMOVE | WM.WM_ENTERMENULOOP:
            {
                nuint ret = SetTimer(handle, loop_timer_id, USER_TIMER_MINIMUM, null);
                if (ret == 0)
                {
                    throw new Exception("Failed to set timer");
                }
                return 0;
            }
            case WM.WM_EXITSIZEMOVE | WM.WM_EXITMENULOOP:
            {
                KillTimer(handle, loop_timer_id);
                return 0;
            }
            case WM.WM_TIMER:
            {
                if (wParam == loop_timer_id)
                {
                    Backend.Render();
                    return 0;
                }
                return 0;
            }
            case WM.WM_NCLBUTTONDOWN:
            {
                break;
            }
            case WM.WM_NCHITTEST:
            {
                if (options.AllowResize)
                {
                    POINT point = new POINT(GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam));
                    RECT rc;
                    GetWindowRect(handle, &rc);
                    if (point.y >= rc.top && point.y < rc.top + border_width) {
                        if (point.x >= rc.left && point.x < rc.left + border_width) {
                            return HTTOPLEFT;
                        }
                        if (point.x >= rc.right - border_width && point.x < rc.right) {
                            return HTTOPRIGHT;
                        }
                        return HTTOP;
                    }

                    if (point.y >= rc.bottom - border_width && point.y < rc.bottom) {
                        if (point.x >= rc.left && point.x < rc.left + border_width) {
                            return HTBOTTOMLEFT;
                        }
                        if (point.x >= rc.right - border_width && point.x < rc.right) {
                            return HTBOTTOMRIGHT;
                        }
                        return HTBOTTOM;
                    }

                    if (point.x >= rc.left && point.x < rc.left + border_width) {
                        return HTLEFT;
                    }
                    if (point.x >= rc.right - border_width && point.x < rc.right) {
                        return HTRIGHT;
                    }
                }
                return HTCLIENT;
            }
            case WM.WM_DESTROY:
            {
                PostQuitMessage(0);
                return 0;
            }
        }
        return DefWindowProcW(window, msg, wParam, lParam);
    }
}
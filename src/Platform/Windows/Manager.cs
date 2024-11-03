using System.Drawing;
using Cervo.Data;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

#pragma warning disable CA1416 Disables warning -> "CA1416: Validate platform compatibility"

namespace Cervo.Platform.Windows;

internal static unsafe class Manager
{
    private const ushort windows_11_min_build = 22000;

    /// <summary>
    /// Get primary screen size
    /// </summary>
    /// <param name="width">width of screen</param>
    /// <param name="height">height of screen</param>
    internal static void GetScreenSize(out int width, out int height)
    {
        width = GetSystemMetrics(SM.SM_CXSCREEN);
        height = GetSystemMetrics(SM.SM_CYSCREEN);
    }

    /// <summary>
    /// Check if the current Windows OS is version 11
    /// </summary>
    internal static bool IsWindows11 => Environment.OSVersion.Version.Build >= windows_11_min_build;

    /// <summary>
    /// Sets the window style based on the current Windows version
    /// </summary>
    internal static void SetWindowStyle(in HWND hWnd, in WindowOptions options)
    {
        // Check if the current Windows version is 11, Windows 10 doesn't require any changes
        if (!IsWindows11) return;
        // Set window style to rounded corners
        uint val = (uint)DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
        DwmSetWindowAttribute(hWnd, (uint)DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, &val, sizeof(uint));

        if (options.NativeBorders == false)
        {
            // Remove window border
            uint val2 = 0xFFFFFFFE;
            DwmSetWindowAttribute(hWnd, (uint)DWMWINDOWATTRIBUTE.DWMWA_BORDER_COLOR, &val2, sizeof(uint));
        }
    }
}
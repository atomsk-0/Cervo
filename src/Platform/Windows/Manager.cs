using System.Drawing;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

#pragma warning disable CA1416 Disables warning -> "CA1416: Validate platform compatibility"

namespace Cervo.Platform.Windows;

internal static class Manager
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
}
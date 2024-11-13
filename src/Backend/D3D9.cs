using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Cervo.Data;
using Cervo.Type.Interface;
using Cervo.Util;
using Mochi.DearImGui;
using Mochi.DearImGui.Backends.Direct3D9;
using Mochi.DearImGui.Backends.Win32;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using IDirect3DDevice9 = TerraFX.Interop.DirectX.IDirect3DDevice9;

/* DirectX9 Backend Only for Windows platform */
#pragma warning disable CA1416 Disables warning -> "CA1416: Validate platform compatibility"

namespace Cervo.Backend;

// Currently outdated implementation compared to D3D11Backend TODO: Update this

public unsafe partial class D3D9 : IBackend
{
    #region Additional Imports
    private const string library = "d3dx9_43.dll";

    // ReSharper disable once InconsistentNaming
    [LibraryImport(library)]
    private static partial HRESULT D3DXCreateTextureFromFileW(IDirect3DDevice9* pDevice, [MarshalAs(UnmanagedType.LPWStr)] string pSrcFile, IDirect3DTexture9** ppTexture);

    // ReSharper disable once InconsistentNaming
    [LibraryImport(library)]
    private static partial HRESULT D3DXCreateTextureFromFileInMemory(IDirect3DDevice9* pDevice, byte* pSrcData, uint srcDataSize, IDirect3DTexture9** ppTexture);
    #endregion

    private IDirect3D9* d3d9;
    private D3DPRESENT_PARAMETERS presentParameters;
    private IDirect3DDevice9* device;
    private HWND windowHandle;

    private bool deviceLost;

    private uint backendWidth, backendHeight;

    public bool Setup(IWindow window)
    {
        if ((d3d9 = DirectX.Direct3DCreate9(D3D.D3D_SDK_VERSION)) == null) return false;

        windowHandle = (HWND)window.GetHandle();

        var lPresentParameters = new D3DPRESENT_PARAMETERS
        {
            Windowed = true,
            SwapEffect = D3DSWAPEFFECT.D3DSWAPEFFECT_DISCARD,
            BackBufferFormat = D3DFORMAT.D3DFMT_UNKNOWN,
            EnableAutoDepthStencil = true,
            AutoDepthStencilFormat = D3DFORMAT.D3DFMT_D16,
            PresentationInterval = D3DPRESENT.D3DPRESENT_INTERVAL_ONE, // ONE -> V-Sync on -- IMMEDIATE -> V-Sync off
        };

        IDirect3DDevice9* lDevice = null;

        if (d3d9->CreateDevice(DirectX.D3DADAPTER_DEFAULT, D3DDEVTYPE.D3DDEVTYPE_HAL, windowHandle, D3DCREATE.D3DCREATE_HARDWARE_VERTEXPROCESSING, &lPresentParameters, &lDevice) < 0) return false;

        // Set device and present parameters from local variables
        device = lDevice;
        presentParameters = lPresentParameters;

        ImUtils.SetupImGui();
        Win32ImBackend.Init(windowHandle);
        Direct3D9ImBackend.Init((Mochi.DearImGui.Backends.Direct3D9.IDirect3DDevice9*)device);

        backendWidth = presentParameters.BackBufferWidth;
        backendHeight = presentParameters.BackBufferHeight;

        return true;
    }


    public void Reset()
    {
        Direct3D9ImBackend.InvalidateDeviceObjects();
        var lPresentParameters = presentParameters;
        if (device->Reset(&lPresentParameters) == D3DERR.D3DERR_INVALIDCALL)
        {
            // Log error, maybe for now we don't need to throw an exception
        }
        lPresentParameters = presentParameters;
        Direct3D9ImBackend.CreateDeviceObjects();
    }


    public void Render()
    {
        device->Clear(0, null, D3DCLEAR.D3DCLEAR_TARGET, 0, 1.0f, 0);
        device->BeginScene();
        device->EndScene();
        device->Present(null, null, HWND.NULL, null);
        // If device is lost, we need to check if it's ready to be reset
        if (deviceLost)
        {
            HRESULT hr = device->TestCooperativeLevel();
            if (hr == D3DERR.D3DERR_DEVICELOST)
            {
                Thread.Sleep(10);
                return;
            }
            if (hr == D3DERR.D3DERR_DEVICENOTRESET) Reset();
            deviceLost = false;
        }

        if (backendWidth != presentParameters.BackBufferWidth || backendHeight != presentParameters.BackBufferHeight)
        {
            presentParameters.BackBufferWidth = backendWidth;
            presentParameters.BackBufferHeight = backendHeight;
            Reset();
        }

        Direct3D9ImBackend.NewFrame();
        Win32ImBackend.NewFrame();
        ImGui.NewFrame();

        ImGui.ShowDemoWindow();

        ImGui.End();

        ImGui.EndFrame();

        device->SetRenderState(D3DRENDERSTATETYPE.D3DRS_ZENABLE, 0);
        device->SetRenderState(D3DRENDERSTATETYPE.D3DRS_ALPHABLENDENABLE, 0);
        device->SetRenderState(D3DRENDERSTATETYPE.D3DRS_SCISSORTESTENABLE, 0);
        device->Clear(0, null, D3DCLEAR.D3DCLEAR_TARGET | D3DCLEAR.D3DCLEAR_ZBUFFER, 0, 1.0f, 0);

        if (device->BeginScene() >= 0)
        {
            ImGui.Render();
            Direct3D9ImBackend.RenderDrawData(ImGui.GetDrawData());
            device->EndScene();
        }

        HRESULT result = device->Present(null, null, HWND.NULL, null);
        deviceLost = result == D3DERR.D3DERR_DEVICELOST;
    }


    public void Destroy()
    {
        if (device != null)
        {
            device->Release();
            device = null;
        }

        if (d3d9 != null)
        {
            d3d9->Release();
            d3d9 = null;
        }
    }

    public void OnResize(int width, int height)
    {
        backendWidth = (uint)width;
        backendHeight = (uint)height;
    }


    public Action OnRender { get; set; } = null!;

    public bool TryLoadTextureFromFile(string path, out Texture texture)
    {
        IDirect3DTexture9* d3dTexture;
        HRESULT hresult = D3DXCreateTextureFromFileW(device, path, &d3dTexture);
        if (hresult != S.S_OK)
        {
            texture = default;
            return false;
        }

        D3DSURFACE_DESC desc;
        d3dTexture->GetLevelDesc(0, &desc);

        texture = new Texture((nint)d3dTexture, desc.Width, desc.Height);
        return true;
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
        IDirect3DTexture9* d3dTexture;
        HRESULT hresult = D3DXCreateTextureFromFileInMemory(device, data, (uint)length, &d3dTexture);
        if (hresult != S.S_OK)
        {
            texture = default;
            return false;
        }

        D3DSURFACE_DESC desc;
        d3dTexture->GetLevelDesc(0, &desc);

        texture = new Texture((nint)d3dTexture, desc.Width, desc.Height);
        return true;
    }


    public Size GetViewportSize()
    {
        return new Size((int)presentParameters.BackBufferWidth, (int)presentParameters.BackBufferHeight);
    }
}
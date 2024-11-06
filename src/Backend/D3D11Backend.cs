using System.Drawing;
using Cervo.Data;
using Cervo.Type.Interface;
using Cervo.Util;
using Hexa.NET.DirectXTex;
using Mochi.DearImGui;
using Mochi.DearImGui.Backends.Direct3D11;
using Mochi.DearImGui.Backends.Win32;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using ID3D11Device = TerraFX.Interop.DirectX.ID3D11Device;
using ID3D11DeviceContext = TerraFX.Interop.DirectX.ID3D11DeviceContext;
using ID3D11Resource = TerraFX.Interop.DirectX.ID3D11Resource;

/* DirectX11 Backend Only for Windows platform */
#pragma warning disable CA1416 Disables warning -> "CA1416: Validate platform compatibility"

namespace Cervo.Backend;

public unsafe class D3D11Backend : IBackend
{
    private ID3D11Device* device;
    private ID3D11DeviceContext* context;
    private IDXGISwapChain* swapChain;
    private ID3D11RenderTargetView* renderTargetView;
    private HWND windowHandle;

    private uint backendWidth, backendHeight;
    private bool swapChainOccluded;

    public bool Setup(IWindow window)
    {
        windowHandle = (HWND)window.GetHandle();

        DXGI_SWAP_CHAIN_DESC sd = default;
        sd.BufferCount = 2;
        sd.BufferDesc.Width = 0;
        sd.BufferDesc.Height = 0;
        sd.BufferDesc.Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
        sd.BufferDesc.RefreshRate.Numerator = 60;
        sd.BufferDesc.RefreshRate.Denominator = 1;
        sd.Flags = (uint)DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH;
        sd.BufferUsage = DXGI.DXGI_USAGE_RENDER_TARGET_OUTPUT;
        sd.OutputWindow = windowHandle;
        sd.SampleDesc.Count = 1;
        sd.SampleDesc.Quality = 0;
        sd.Windowed = true;
        sd.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_DISCARD;

        if (createDeviceAndSwapChain(sd) == false) return false;

        createRenderTarget();

        ImUtils.SetupImGui();
        Win32ImBackend.Init(windowHandle);
        Direct3D11ImBackend.Init((Mochi.DearImGui.Backends.Direct3D11.ID3D11Device*)device, (Mochi.DearImGui.Backends.Direct3D11.ID3D11DeviceContext*)context);

        return true;
    }


    private bool createDeviceAndSwapChain(DXGI_SWAP_CHAIN_DESC sd)
    {
        const uint create_device_flags = 0;
        D3D_FEATURE_LEVEL[] featureLevelArray = [D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_0];
        fixed (D3D_FEATURE_LEVEL* pFeatureLevelArr = featureLevelArray)
        {
            D3D_FEATURE_LEVEL featureLevel;
            ID3D11DeviceContext* lContext;
            ID3D11Device* lDevice;
            IDXGISwapChain* lSwapChain;
            HRESULT res = DirectX.D3D11CreateDeviceAndSwapChain(null,
                D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE,
                HMODULE.NULL,
                create_device_flags,
                pFeatureLevelArr,
                2,
                TerraFX.Interop.DirectX.D3D11.D3D11_SDK_VERSION,
                &sd,
                &lSwapChain,
                &lDevice,
                &featureLevel,
                &lContext);
            if (res == DXGI.DXGI_ERROR_UNSUPPORTED)
            {
                res = DirectX.D3D11CreateDeviceAndSwapChain(null,
                    D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_WARP,
                    HMODULE.NULL,
                    create_device_flags,
                    pFeatureLevelArr,
                    2,
                    TerraFX.Interop.DirectX.D3D11.D3D11_SDK_VERSION,
                    &sd,
                    &lSwapChain,
                    &lDevice,
                    &featureLevel,
                    &lContext);
            }
            if (res != S.S_OK) return false;

            device = lDevice;
            context = lContext;
            swapChain = lSwapChain;
        }

        return true;
    }


    private void createRenderTarget()
    {
        using ComPtr<ID3D11Resource> backBuffer = null;
        swapChain->GetBuffer(0, Windows.__uuidof<ID3D11Texture2D>(), (void**)backBuffer.GetAddressOf());
        ID3D11RenderTargetView* lRenderTargetView;
        device->CreateRenderTargetView(backBuffer.Get(), null, &lRenderTargetView);
        renderTargetView = lRenderTargetView;
    }

    private void cleanUpRenderTarget()
    {
        if (renderTargetView == null) return;
        renderTargetView->Release();
        renderTargetView = null;
    }

    public void Reset() {} // Not implemented for D3D11 for now

    public void Render()
    {
        if (swapChainOccluded && swapChain->Present(0, DXGI.DXGI_PRESENT_TEST) == DXGI.DXGI_STATUS_OCCLUDED)
        {
            Thread.Sleep(10);
            return;
        }
        swapChainOccluded = false;

        var viewPortSize = GetViewportSize();
        if (backendWidth != viewPortSize.Width || backendHeight != viewPortSize.Height)
        {
            cleanUpRenderTarget();
            swapChain->ResizeBuffers(0, backendWidth, backendHeight, DXGI_FORMAT.DXGI_FORMAT_UNKNOWN, 0);
            createRenderTarget();
        }

        Direct3D11ImBackend.NewFrame();
        Win32ImBackend.NewFrame();
        ImGui.NewFrame();

        ImGui.Render();
        ID3D11RenderTargetView* lRenderTargetView = renderTargetView;
        context->OMSetRenderTargets(1, &lRenderTargetView, null);
        renderTargetView = lRenderTargetView;
        float[] clearColor = [0.0f, 0.0f, 0.0f, 0.0f];
        fixed (float* pClearColor = clearColor) context->ClearRenderTargetView(renderTargetView, pClearColor);
        Direct3D11ImBackend.RenderDrawData(ImGui.GetDrawData());

        HRESULT hr = swapChain->Present(1, 0);
        swapChainOccluded = hr == DXGI.DXGI_STATUS_OCCLUDED;
    }

    public void Destroy()
    {
        cleanUpRenderTarget();
        if (swapChain != null) swapChain->Release();
        if (context != null) context->Release();
        if (device != null) device->Release();
    }

    public void OnResize(int width, int height)
    {
        backendWidth = (uint)width;
        backendHeight = (uint)height;
    }

    public Action OnRender { get; set; } = null!;

    public bool TryLoadTextureFromFile(string path, out Texture texture)
    {
        texture = default;
        ScratchImage image = DirectXTex.CreateScratchImage();
        TexMetadata metadata = default;

        FileInfo fileInfo = new FileInfo(path);
        if (fileInfo.Exists == false) return false;
        switch (fileInfo.Extension)
        {
            case ".png":
                if (DirectXTex.LoadFromPNGFile(path, &metadata, &image) != S.S_OK) return false;
                break;
            case ".jpeg" or ".jpg":
                if (DirectXTex.LoadFromJPEGFile(path, &metadata, &image) != S.S_OK) return false;
                break;
            default:
                return false;
        }
        texture = new Texture(image.Handle, (uint)metadata.Width, (uint)metadata.Height);
        return true;
    }

    public bool TryLoadTextureFromMemory(in MemoryStream stream, uint width, uint height, out Texture texture)
    {
        fixed (byte* pData = stream.ToArray())
        {
            return TryLoadTextureFromMemory(pData, width, height, (uint)stream.Length, out texture);
        }
    }

    /// <summary>
    /// Load TGA texture from memory
    /// </summary>
    /// <param name="data"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="length"></param>
    /// <param name="texture"></param>
    /// <returns></returns>
    public bool TryLoadTextureFromMemory(byte* data, uint width, uint height, nuint length, out Texture texture)
    {
        texture = default;
        ScratchImage image = DirectXTex.CreateScratchImage();
        TexMetadata metadata = default;

        if (DirectXTex.LoadFromWICMemory(data, length, WICFlags.None, &metadata, &image, default) != S.S_OK) return false;

        texture = new Texture(image.Handle, (uint)metadata.Width, (uint)metadata.Height);
        return true;
    }

    public Size GetViewportSize()
    {
        DXGI_SWAP_CHAIN_DESC desc;
        swapChain->GetDesc(&desc);
        return new Size((int)desc.BufferDesc.Width, (int)desc.BufferDesc.Height);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Bindings.Metal;
using Bindings.ObjectiveC;
using Aetherium.Configuration.Internal;
using Aetherium.Hooking;
using Aetherium.Interface.Style;
using Aetherium.Utility.Timing;
using ImGuiNET;
using ReShadeRuntime;
using Serilog;

namespace Aetherium.Interface.Internal;

/// <summary>
/// This class manages interaction with the ImGui interface.
/// </summary>
[ServiceManager.BlockingEarlyLoadedService]
internal partial class InterfaceManager : IDisposable, IServiceType
{
    [LibraryImport("libAetherium")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ImGui_ImplMetal_Init(nint device);

    [LibraryImport("libAetherium")]
    private static partial void ImGui_ImplMetal_NewFrame(nint renderPassDescriptor);

    [LibraryImport("libAetherium")]
    private static partial void ImGui_ImplMetal_RenderDrawData(nint drawData, nint commandBuffer, nint commandEncoder);

    [LibraryImport("libAetherium")]
    private static partial void ImGui_ImplMetal_DeInit();
    
    [LibraryImport("libAetherium")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ImGui_ImplMacOS_Init(nint view);
    
    [LibraryImport("libAetherium")]
    private static partial void ImGui_ImplMacOS_DeInit();
    
    [LibraryImport("libAetherium")]
    private static partial void ImGui_ImplMacOS_NewFrame(nint view);
    
    [LibraryImport("libAetherium")]
    private static partial void ImGui_ImplMacOS_NewViewportFrame();
    
    [DllImport("libAetherium", EntryPoint = "getMainWindow")]
    private static extern NSWindow GetMainWindow();
    
    private const float DefaultFontSizePt = 12.0f;
    private const float DefaultFontSizePx = DefaultFontSizePt * 4.0f / 3.0f;
    
    public delegate void BuildUIDelegate();

    public MTLDevice MetalDevice { get; }
    private MTLTexture depthTexture;
    private NSWindow mainWindow;
    private NSView mainView;

    public Runtime? ReShadeRuntime { get; private set; }

    private Hook<PresentDrawableDelegate> metalPresentHook;
    private Hook<NextDrawableDelegate> nextDrawableHook;
    private readonly Hook<SetDepthTextureDelegate> setDepthTextureHook;
    
    private delegate void SetDepthTextureDelegate(nint id, Selector selector, MTLTexture texture);
    private delegate void PresentDrawableDelegate(MTLCommandBuffer id, Selector selector, CAMetalDrawable drawable);
    private delegate nint NextDrawableDelegate(CAMetalLayer id, Selector selector);

    
    private void SetDepthTextureDetour(nint id, Selector selector, MTLTexture texture)
    {
        setDepthTextureHook.Original(id, selector, texture);
        if (!texture.IsNull)
            depthTexture = texture;
    }

    [ServiceManager.ServiceConstructor]
    private InterfaceManager()
    {
        MetalDevice = MTLDevice.MTLCreateSystemDefaultDevice();
        mainWindow = new NSWindow(nint.Zero);
        mainView = new NSView(nint.Zero);
        depthTexture = new MTLTexture(nint.Zero);
        ImGui.CreateContext();
        ImGui_ImplMetal_Init(MetalDevice.NativePtr);
        setDepthTextureHook =
            Hook<SetDepthTextureDelegate>.FromObjCMethod("MTLRenderPassDepthAttachmentDescriptorInternal",
                "setTexture:",
                SetDepthTextureDetour);
        CheckViewportState();
        setDepthTextureHook.Enable();
    }

    /// <summary>
    /// This event gets called each frame to facilitate ImGui drawing.
    /// </summary>
    public event BuildUIDelegate Draw;

    /// <summary>
    /// Gets or sets the pointer to ImGui.IO(), when it was last used.
    /// </summary>
    public ImGuiIOPtr LastImGuiIoPtr { get; set; }

    /// <summary>
    /// Gets a value indicating whether the Aetherium interface ready to use.
    /// </summary>
    public bool IsReady => mainView != nint.Zero;

    /// <summary>
    /// Gets or sets a value indicating whether or not Draw events should be dispatched.
    /// </summary>
    public bool IsDispatchingEvents { get; set; } = true;

    /// <summary>
    /// Gets the default ImGui font.
    /// </summary>
    public static ImFontPtr DefaultFont { get; private set; }

    /// <summary>
    /// Gets an included FontAwesome icon font.
    /// </summary>
    public static ImFontPtr IconFont { get; private set; }

    /// <summary>
    /// Gets an included monospaced font.
    /// </summary>
    public static ImFontPtr MonoFont { get; private set; }

    /// <summary>
    /// Dispose of managed and unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        setDepthTextureHook.Dispose();
        
        ImGui_ImplMacOS_DeInit();
        ImGui_ImplMetal_DeInit();
        ImGui.DestroyContext();
    }

#nullable enable
/*
    /// <summary>
    /// Load an image from disk.
    /// </summary>
    /// <param name="filePath">The filepath to load.</param>
    /// <returns>A texture, ready to use in ImGui.</returns>
    public TextureWrap? LoadImage(string filePath)
    {
        if (this.scene == null)
            throw new InvalidOperationException("Scene isn't ready.");

        try
        {
            var wrap = this.scene?.LoadImage(filePath);
            return wrap != null ? new AetheriumTextureWrap(wrap) : null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to load image from {filePath}");
        }

        return null;
    }

    /// <summary>
    /// Load an image from an array of bytes.
    /// </summary>
    /// <param name="imageData">The data to load.</param>
    /// <returns>A texture, ready to use in ImGui.</returns>
    public TextureWrap? LoadImage(byte[] imageData)
    {
        if (this.scene == null)
            throw new InvalidOperationException("Scene isn't ready.");

        try
        {
            var wrap = this.scene?.LoadImage(imageData);
            return wrap != null ? new AetheriumTextureWrap(wrap) : null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to load image from memory");
        }

        return null;
    }

    /// <summary>
    /// Load an image from an array of bytes.
    /// </summary>
    /// <param name="imageData">The data to load.</param>
    /// <param name="width">The width in pixels.</param>
    /// <param name="height">The height in pixels.</param>
    /// <param name="numChannels">The number of channels.</param>
    /// <returns>A texture, ready to use in ImGui.</returns>
    public TextureWrap? LoadImageRaw(byte[] imageData, int width, int height, int numChannels)
    {
        if (this.scene == null)
            throw new InvalidOperationException("Scene isn't ready.");

        try
        {
            var wrap = this.scene?.LoadImageRaw(imageData, width, height, numChannels);
            return wrap != null ? new AetheriumTextureWrap(wrap) : null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to load image from raw data");
        }

        return null;
    }
*/
#nullable restore

    private bool fixedFrameBuffer;
    
    private nint NextDrawableDetour(CAMetalLayer caMetalLayer, Selector selector)
    {
        if (fixedFrameBuffer) return nextDrawableHook.Original(caMetalLayer, selector);
        caMetalLayer.framebufferOnly = false;
        fixedFrameBuffer = true;
        return nextDrawableHook.Original(caMetalLayer, selector);
    }

    private void InitScene()
    {
        if (!IsReady) return;
        
        using (Timings.Start("IM Scene Init"))
        {
            ImGui_ImplMacOS_Init(mainView);

            var startInfo = Service<AetheriumStartInfo>.Get();
            var configuration = Service<AetheriumConfiguration>.Get();

            var iniFileInfo = new FileInfo(Path.Combine(Path.GetDirectoryName(startInfo.ConfigurationPath), "aetheriumUI.ini"));

            try
            {
                if (iniFileInfo.Length > 1200000)
                {
                    Log.Warning("AetheriumUI.ini was over 1mb, deleting");
                    iniFileInfo.CopyTo(Path.Combine(iniFileInfo.DirectoryName,
                        $"AetheriumUI-{DateTimeOffset.Now.ToUnixTimeSeconds()}.ini"));
                    iniFileInfo.Delete();
                }
            }
            catch (FileNotFoundException ex)
            {
                Log.Warning(ex, "Could not find Aetherium.UI");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not delete AetheriumUI.ini");
            }

            StyleModel.TransferOldModels();

            if (configuration.SavedStyles == null || configuration.SavedStyles.All(x => x.Name != StyleModelV1.AetheriumStandard.Name))
            {
                configuration.SavedStyles = new List<StyleModel> { StyleModelV1.AetheriumStandard, StyleModelV1.AetheriumClassic };
                configuration.ChosenStyle = StyleModelV1.AetheriumStandard.Name;
            }
            else if (configuration.SavedStyles.Count == 1)
            {
                configuration.SavedStyles.Add(StyleModelV1.AetheriumClassic);
            }
            else if (configuration.SavedStyles[1].Name != StyleModelV1.AetheriumClassic.Name)
            {
                configuration.SavedStyles.Insert(1, StyleModelV1.AetheriumClassic);
            }

            configuration.SavedStyles[0] = StyleModelV1.AetheriumStandard;
            configuration.SavedStyles[1] = StyleModelV1.AetheriumClassic;

            var style = configuration.SavedStyles.FirstOrDefault(x => x.Name == configuration.ChosenStyle);
            if (style == null)
            {
                style = StyleModelV1.AetheriumStandard;
                configuration.ChosenStyle = style.Name;
                configuration.QueueSave();
            }

            style.Apply();

            ImGui.GetIO().FontGlobalScale = configuration.GlobalUiScale;

            SetupFonts();

            if (!configuration.IsDocking)
            {
                ImGui.GetIO().ConfigFlags &= ~ImGuiConfigFlags.DockingEnable;
            }
            else
            {
                ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            }

            // NOTE (Chiv) Toggle gamepad navigation via setting
            if (!configuration.IsGamepadNavigationEnabled)
            {
                ImGui.GetIO().BackendFlags &= ~ImGuiBackendFlags.HasGamepad;
                ImGui.GetIO().ConfigFlags &= ~ImGuiConfigFlags.NavEnableSetMousePos;
            }
            else
            {
                ImGui.GetIO().BackendFlags |= ImGuiBackendFlags.HasGamepad;
                ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NavEnableSetMousePos;
            }

            // NOTE (Chiv) Explicitly deactivate on Aetherium boot
            ImGui.GetIO().ConfigFlags &= ~ImGuiConfigFlags.NavEnableGamepad;

            ImGuiHelpers.MainViewport = ImGui.GetMainViewport();

            Log.Information("[IM] Scene & ImGui setup OK!");
        }
        
        Service<InterfaceManagerWithScene>.Provide(new InterfaceManagerWithScene(this));
    }

    private void PresentDetour(MTLCommandBuffer commandBuffer, Selector selector, CAMetalDrawable drawable)
    {
        if (!IsReady)
        {
            metalPresentHook.Original(commandBuffer, selector, drawable);
            return;
        }

        ReShadeRuntime ??= new Runtime(MetalDevice);
        ReShadeRuntime.Render(commandBuffer, drawable.texture, depthTexture);
        
/*
        if (shaderEnabled)
        {
            var commandEncoder = commandBuffer.renderCommandEncoderWithDescriptor(frameBufferDescriptor);
            commandEncoder.setRenderPipelineState(pipelineState);

            commandEncoder.setFragmentTexture(drawable.texture, 0);
            commandEncoder.setFragmentSamplerState(sampler, 0);
            
            commandEncoder.drawPrimitives(MTLPrimitiveType.Triangle, 0, 3);
            commandEncoder.endEncoding();
        }
*/
        RenderImGui(commandBuffer, drawable);

        metalPresentHook.Original(commandBuffer, selector, drawable);

        if (ImGui.GetIO().ConfigFlags.HasFlag(ImGuiConfigFlags.ViewportsEnable))
        {
            ImGui_ImplMacOS_NewViewportFrame();
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void RenderImGui(MTLCommandBuffer commandBuffer, CAMetalDrawable drawable)
    {
        ImGuiHelpers.NewFrame();
        var io = ImGui.GetIO();
        LastImGuiIoPtr = io;
        io.DisplaySize.X = (float)mainView.bounds.size.width;
        io.DisplaySize.Y = (float)mainView.bounds.size.height;
        var framebufferScale = mainView.window?.screen.backingScaleFactor ?? NSScreen.mainScreen.backingScaleFactor;
        io.DisplayFramebufferScale = new Vector2(framebufferScale, framebufferScale);
        var renderPassDescriptor = MTLRenderPassDescriptor.New();
        var colorAttachment = renderPassDescriptor.colorAttachments[0];
        colorAttachment.texture = drawable.texture;
        colorAttachment.loadAction = MTLLoadAction.Load;
        ImGui_ImplMetal_NewFrame(renderPassDescriptor);
        ImGui_ImplMacOS_NewFrame(mainView);
        ImGui.NewFrame();
        io.MouseDrawCursor = io.WantCaptureMouse;
        Draw();
        ImGui.Render();
        var drawData = ImGui.GetDrawData();
        var renderEncoder = commandBuffer.renderCommandEncoderWithDescriptor(renderPassDescriptor);
        renderPassDescriptor.Release();
        ImGui_ImplMetal_RenderDrawData(new nint(drawData), commandBuffer.NativePtr, renderEncoder.NativePtr);
        renderEncoder.endEncoding();
    }

    private void CheckViewportState()
    {
        var configuration = Service<AetheriumConfiguration>.Get();

        if (configuration.IsDisableViewport || mainView.window == null || ImGui.GetPlatformIO().Monitors.Size == 1)
        {
            ImGui.GetIO().ConfigFlags &= ~ImGuiConfigFlags.ViewportsEnable;
            return;
        }

        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
    }

    /// <summary>
    /// Loads font for use in ImGui text functions.
    /// </summary>
    private void SetupFonts()
    {
        var io = ImGui.GetIO();
        var ioFonts = io.Fonts;

        ioFonts.Clear();
        ioFonts.TexDesiredWidth = 4096;
        DefaultFont = ioFonts.AddFontFromFileTTF("/Library/Fonts/SF-Pro-Display-Medium.otf", DefaultFontSizePx * io.FontGlobalScale);
        MonoFont = ioFonts.AddFontFromFileTTF("/Library/Fonts/SF-Mono-Regular.otf", DefaultFontSizePx * io.FontGlobalScale);
    }

    [ServiceManager.CallWhenServicesReady]
    private void ContinueConstruction()
    {
        Log.Information("Searching for game window...");
        mainWindow = GetMainWindow();
        mainView = mainWindow.contentView;
        Log.Information("Found game window");
        InitScene();
        Log.Verbose("===== P R E S E N T D R A W A B L E =====");
        metalPresentHook =
            Hook<PresentDrawableDelegate>.FromObjCMethod("_MTLCommandBuffer", "presentDrawable:", PresentDetour);
        nextDrawableHook =
            Hook<NextDrawableDelegate>.FromObjCMethod("CAMetalLayer", "nextDrawable", NextDrawableDetour);
        Log.Verbose($"Metal present address 0x{metalPresentHook.Address.ToInt64():X}");
        Log.Verbose($"Next drawable address 0x{nextDrawableHook.Address.ToInt64():X}");
        LastImGuiIoPtr = ImGui.GetIO();
        CheckViewportState();
        metalPresentHook.Enable();
        nextDrawableHook.Enable();
    }

    /// <summary>
    /// Represents an instance of InstanceManager with scene ready for use.
    /// </summary>
    public class InterfaceManagerWithScene : IServiceType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterfaceManagerWithScene"/> class.
        /// </summary>
        /// <param name="interfaceManager">An instance of <see cref="InterfaceManager"/>.</param>
        internal InterfaceManagerWithScene(InterfaceManager interfaceManager)
        {
            Manager = interfaceManager;
        }

        /// <summary>
        /// Gets the associated InterfaceManager.
        /// </summary>
        public InterfaceManager Manager { get; }
    }
}

using System;
using Aetherium.Bindings.Metal;

namespace Aetherium.Interface.Internal;

/// <summary>
/// Safety harness for ImGuiScene textures that will defer destruction until
/// the end of the frame.
/// </summary>
public class TextureWrap
{
    private readonly MTLTexture texture;

    /// <summary>
    /// Initializes a new instance of the <see cref="AetheriumTextureWrap"/> class.
    /// </summary>
    /// <param name="wrappingWrap">The texture wrap to wrap.</param>
    internal TextureWrap(MTLTexture texture)
    {
        this.texture = texture;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="AetheriumTextureWrap"/> class.
    /// </summary>
    ~TextureWrap()
    {
        Dispose(false);
    }

    /// <summary>
    /// Gets the ImGui handle of the texture.
    /// </summary>
    public IntPtr ImGuiHandle => this.texture.NativePtr;

    /// <summary>
    /// Gets the width of the texture.
    /// </summary>
    public int Width => (int)this.texture.Width;

    /// <summary>
    /// Gets the height of the texture.
    /// </summary>
    public int Height => (int)this.texture.Height;

    /// <summary>
    /// Queue the texture to be disposed once the frame ends.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Actually dispose the wrapped texture.
    /// </summary>
    internal void RealDispose()
    {
    }

    private void Dispose(bool disposing)
    {
    }
}

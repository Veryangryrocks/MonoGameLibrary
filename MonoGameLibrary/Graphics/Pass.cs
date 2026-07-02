using System.Text.Json.Serialization;

namespace MonoGameLibrary.Graphics;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(RasterPass), "raster")]
[JsonDerivedType(typeof(MaterialPass), "material")]
[JsonDerivedType(typeof(CompositePass), "composite")]
[JsonDerivedType(typeof(BlitPass), "blit")]
[JsonDerivedType(typeof(DuplicatePass), "duplicate")]
[JsonDerivedType(typeof(PresentPass), "present")]
public abstract class Pass;

public sealed class RasterPass : Pass
{
    [JsonPropertyName("layers")]
    public string[] LayerKeysArray { get; init; }
    [JsonPropertyName("width")]
    public int Width { get; init; }
    [JsonPropertyName("height")]
    public int Height { get; init; }
    [JsonPropertyName("output")]
    public string OutputKey { get; init; }
}

public sealed class MaterialPass : Pass
{
    [JsonPropertyName("input")]
    public string InputKey { get; init; }

    [JsonPropertyName("materials")]
    public string[] MaterialKeysArray { get; init; }
    [JsonPropertyName("output")]
    public string OutputKey { get; init; }
}

public sealed class CompositePass : Pass
{
    [JsonPropertyName("input_back")]
    public string InputKeyBack{ get; init; }
    [JsonPropertyName("input_front")]
    public string InputKeyFront { get; init; }
    [JsonPropertyName("output")]
    public string OutputKey { get; init; }
}

public sealed class BlitPass : Pass
{
    [JsonPropertyName("input")]
    public string InputKey { get; init; }
    [JsonPropertyName("width")]
    public int TargetWidth { get; init; }
    [JsonPropertyName("height")]
    public int TargetHeight  { get; init; }
    [JsonPropertyName("source_width")]
    public int SourceWidth { get; init; }
    [JsonPropertyName("source_height")]
    public int SourceHeight  { get; init; }
    [JsonPropertyName("source_x")]
    public int SourceX { get; init; }
    [JsonPropertyName("source_y")]
    public int SourceY { get; init; }
    [JsonPropertyName("dest_width")]
    public int DestWidth { get; init; }
    [JsonPropertyName("dest_height")]
    public int DestHeight  { get; init; }
    [JsonPropertyName("dest_x")]
    public int DestX { get; init; }
    [JsonPropertyName("dest_y")]
    public int DestY { get; init; }
    [JsonPropertyName("output")]
    public string OutputKey { get; init; }
}

public sealed class DuplicatePass : Pass
{
    [JsonPropertyName("input")]
    public string InputKey { get; init; }
    [JsonPropertyName("outputs")]
    public string[] OutputKeys { get; init; }
}

public sealed class PresentPass : Pass
{
    [JsonPropertyName("input")]
    public string InputKey { get; init; }
}
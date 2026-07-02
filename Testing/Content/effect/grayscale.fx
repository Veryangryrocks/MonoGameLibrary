#if OPENGL
    #define PS_SHADERMODEL ps_3_0
#else
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

texture Texture;

sampler2D TextureSampler = sampler_state
{
    Texture = <Texture>;
};

float4 MainPS(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(TextureSampler, texCoord) * color;

    float gray = dot(c.rgb, float3(0.299, 0.587, 0.114));

    c.rgb = gray;

    return c;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
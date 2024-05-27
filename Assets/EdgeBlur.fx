sampler tex0 : register(s0);

float2 resolution;
float4 outer;
float4 inner;

float GetLerpValue(float from, float to, float t)
{
    if (from == to)
        return 1;
    if (from < to)
    {
        if (t < from)
            return 0;

        if (t > to)
            return 1;
    }
    else
    {
        if (t < to)
            return 1;

        if (t > from)
            return 0;
    }
    return (t - from) / (to - from);
}

float CheckPos(float2 pos)
{
    float2 center = (inner.xy + inner.zw) / 2;
    float x, y;
    
    if (pos.x < center.x)
        x = GetLerpValue(outer.x, inner.x, pos.x);
    else
        x = GetLerpValue(outer.z, inner.z, pos.x);
    
    if (pos.y < center.y)
        y = GetLerpValue(outer.y, inner.y, pos.y);
    else
        y = GetLerpValue(outer.w, inner.w, pos.y);
    return min(x, y);
}

float4 EdgeBlur(float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(tex0, coords);
    float2 pos = coords * resolution;
    return c * CheckPos(pos);
}

technique T
{
    pass EdgeBlur
    {
        PixelShader = compile ps_3_0 EdgeBlur();
    }
}
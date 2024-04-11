sampler tex0 : register(s0);

int type;
float2 resolution;
float4 outer;
float4 inner;

float CheckPos(float2 pos)
{
    // ÉÏÏÂ±ßÔµ
    if (type == 0)
    {
        if (pos.y < outer.y || pos.y > outer.w)
            return 0;
        if (pos.y > inner.y && pos.y < inner.w)
            return 1;

        if (pos.y >= outer.y && pos.y <= inner.y)
            return lerp(outer.y, inner.y, pos.y);

        if (pos.y <= outer.w && pos.y >= inner.w)
            return lerp(outer.w, inner.w, pos.y);
    }
    // ×óÓÒ±ßÔµ
    else if (type == 1)
    {
        if (pos.x < outer.x || pos.x > outer.z)
            return 0;
        if (pos.x > inner.x && pos.x < inner.z)
            return 1;
            
        if (pos.x >= outer.x && pos.x <= inner.x)
            return lerp(outer.x, inner.x, pos.x);

        if (pos.x <= outer.z && pos.x >= inner.z)
            return lerp(outer.z, inner.z, pos.x);
    }
    // È«±ßÔµ
    else if (type == 2)
    {
        float edgeBlurX = 1, edgeBlurY = 1;
        
        if (pos.x < outer.x || pos.x > outer.z || pos.y < outer.y || pos.y > outer.w)
            return 0;
            
        if (pos.x > inner.x && pos.x < inner.z)
            edgeBlurX = 1;
        else
        {
            if (pos.x >= outer.x && pos.x <= inner.x)
                edgeBlurX = lerp(outer.x, inner.x, pos.x);
            if (pos.x >= inner.z && pos.x <= outer.z)
                edgeBlurX = lerp(outer.z, inner.z, pos.x);
        }
            
        if (pos.y > inner.y && pos.y < inner.w)
            edgeBlurY = 1;
        else
        {
            if (pos.y >= outer.y && pos.y <= inner.y)
                edgeBlurY = lerp(outer.y, inner.y, pos.y);
            if (pos.y >= inner.w && pos.y <= outer.w)
                edgeBlurY = lerp(outer.w, inner.w, pos.y);
        }
            
        return min(edgeBlurX, edgeBlurY);
    }
    return 0;
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
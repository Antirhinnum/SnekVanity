sampler2D topImage : register(s0);
sampler2D bottomImage : register(s1);
float2 resolution : register(c1);
float4 sourceRectangle : register(c2);

float4 PixelShaderFunction(float2 coords : TEXCOORD) : COLOR
{
	float4 topColor = tex2D(topImage, coords);
	float4 bottomColor = tex2D(bottomImage, coords);
	float2 localCoords = frac((coords * resolution) / float2(sourceRectangle.z, sourceRectangle.w));
	
	float lerpValue = localCoords.y < 0.5 ? (4 * localCoords.y * localCoords.y * localCoords.y) : (1 - pow(-2 * localCoords.y + 2, 3) / 2);
	return lerp(topColor, bottomColor, lerpValue);
}

technique Technique1
{
	pass VerticalImageGradientEffect
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
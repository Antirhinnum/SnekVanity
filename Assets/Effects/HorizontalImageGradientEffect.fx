sampler2D leftImage : register(s0);
sampler2D rightImage : register(s1);
float2 resolution : register(c1);
float4 sourceRectangle : register(c2);

float4 PixelShaderFunction(float2 coords : TEXCOORD) : COLOR
{
	float4 leftColor = tex2D(leftImage, coords);
	float4 rightColor = tex2D(rightImage, coords);
	float2 localCoords = frac((coords * resolution) / float2(sourceRectangle.z, sourceRectangle.w));
	
	float lerpValue = localCoords.x < 0.5 ? 4 * localCoords.x * localCoords.x * localCoords.x : 1 - pow(-2 * localCoords.x + 2, 3) / 2;
	
	return lerp(leftColor, rightColor, lerpValue);
}

technique Technique1
{
	pass HorizontalImageGradientEffect
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
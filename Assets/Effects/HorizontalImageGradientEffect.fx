sampler2D image0 : register(s0);
sampler2D image1 : register(s1);
sampler2D image2 : register(s2);
sampler2D image3 : register(s3);
float2 resolution : register(c1);
float4 sourceRectangle : register(c2);

float4 PixelShaderFunction2(float2 coords : TEXCOORD) : COLOR
{
	float4 leftColor = tex2D(image0, coords);
	float4 rightColor = tex2D(image1, coords);
	float2 localCoords = frac((coords * resolution) / float2(sourceRectangle.z, sourceRectangle.w));
	float lerpValue = localCoords.x < 0.5 ? 4 * localCoords.x * localCoords.x * localCoords.x : 1 - pow(-2 * localCoords.x + 2, 3) / 2;
	return lerp(leftColor, rightColor, lerpValue);
}

float4 PixelShaderFunction3(float2 coords : TEXCOORD) : COLOR
{
	float4 leftColor = tex2D(image0, coords);
	float4 middleColor = tex2D(image1, coords);
	float4 rightColor = tex2D(image2, coords);
	float2 localCoords = frac((coords * resolution) / float2(sourceRectangle.z, sourceRectangle.w));
	float lerpValue = localCoords.x < 0.5 ? 4 * localCoords.x * localCoords.x * localCoords.x : 1 - pow(-2 * localCoords.x + 2, 3) / 2;
	if (lerpValue < 0.5)
	{
		return lerp(leftColor, middleColor, lerpValue * 2.0);
	}
	else
	{
		return lerp(middleColor, rightColor, (lerpValue * 2.0) - 1.0);
	}
}

float4 PixelShaderFunction4(float2 coords : TEXCOORD) : COLOR
{
	float4 leftColor = tex2D(image0, coords);
	float4 middleColor = tex2D(image1, coords);
	float4 rightColor = tex2D(image2, coords);
	float4 rightierColor = tex2D(image3, coords);
	float2 localCoords = frac((coords * resolution) / float2(sourceRectangle.z, sourceRectangle.w));
	float lerpValue = localCoords.x < 0.5 ? 4 * localCoords.x * localCoords.x * localCoords.x : 1 - pow(-2 * localCoords.x + 2, 3) / 2;
	if (lerpValue < 1.0 / 3.0)
	{
		return lerp(leftColor, middleColor, lerpValue * 3.0);
	}
	else if (lerpValue < 2.0 / 3.0)
	{
		return lerp(middleColor, rightColor, (lerpValue * 3.0) - 1.0);
	}
	else
	{
		return lerp(rightColor, rightierColor, (lerpValue * 3.0) - 2.0);
	}
}

technique Technique1
{
	pass HorizontalImageGradientEffect2
	{
		PixelShader = compile ps_2_0 PixelShaderFunction2();
	}
	pass HorizontalImageGradientEffect3
	{
		PixelShader = compile ps_2_0 PixelShaderFunction3();
	}
	pass HorizontalImageGradientEffect4
	{
		PixelShader = compile ps_2_0 PixelShaderFunction4();
	}
}
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace SnekVanity.Content.OtherShadersAsDyes;

public sealed class HairDyeColorLayer : PlayerDrawLayer
{
	public override Position GetDefaultPosition()
	{
		return PlayerDrawLayers.AfterLastVanillaLayer;
	}

	protected override void Draw(ref PlayerDrawSet drawInfo)
	{
		for (int i = 0; i < drawInfo.DrawDataCache.Count; i++)
		{
			DrawData data = drawInfo.DrawDataCache[i];
			PlayerDrawHelper.UnpackShader(data.shader, out int localShaderIndex, out PlayerDrawHelper.ShaderConfiguration shaderType);
			if (shaderType == PlayerDrawHelper.ShaderConfiguration.HairShader && localShaderIndex != 0)
			{
				data.color = GameShaders.Hair.GetColor(localShaderIndex, drawInfo.drawPlayer, data.color);
				drawInfo.DrawDataCache[i] = data;
			}
		}
	}
}
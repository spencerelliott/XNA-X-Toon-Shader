float EdgeWidth = 1.0;
float EdgeIntensity = 1.0;
float NormalSensitivity = 1.0;
float DepthSensitivity = 10.0;

float NormalThreshold = 0.5;
float DepthThreshold = 1.0;

float2 ScreenResolution;

texture SceneTexture;

bool UseToon = true;
bool DrawOutline = true;

sampler SceneSampler : register(s0) = sampler_state
{
	Texture = (SceneTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

texture NormalDepthTexture;

sampler NormalDepthSampler : register(s1) = sampler_state
{
	Texture = (NormalDepthTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

float4 PixelShaderFunction(float2 TexCoord : TEXCOORD0) : COLOR0 
{
	float3 scene = tex2D(SceneSampler, TexCoord);

	if (UseToon == false || DrawOutline == false) {
		return float4(scene, 1);
	}

	float2 edgeOffset = EdgeWidth / ScreenResolution;
	
	float4 n1 = tex2D(NormalDepthSampler, TexCoord + float2(-1, -1) * edgeOffset);
	float4 n2 = tex2D(NormalDepthSampler, TexCoord + float2(1, 1) * edgeOffset);
	float4 n3 = tex2D(NormalDepthSampler, TexCoord + float2(-1, 1) * edgeOffset);
	float4 n4 = tex2D(NormalDepthSampler, TexCoord + float2(1, -1) * edgeOffset);


	float4 diagonalDelta = abs(n1 - n2) + abs(n3 - n4);
	float normalDelta = dot(diagonalDelta.xyz, 1);
	float depthDelta = diagonalDelta.w;

	normalDelta = saturate((normalDelta - NormalThreshold) * NormalSensitivity);
	depthDelta = saturate((depthDelta - DepthThreshold) * DepthSensitivity);

	float edgeAmount = saturate(normalDelta + depthDelta) * EdgeIntensity;

	scene *= (1 - edgeAmount);

	return float4(scene, 1);
}

Technique EdgeDetect
{
	pass P0 
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
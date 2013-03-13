float4x4 World;
float4x4 View;
float4x4 Projection;
float3 LightDirection = normalize(float3(1,1,1));

float ToonThresholds[2] = { 0.8, 0.4 };
float ToonBrightnessLevels[3] = { 1.3, 0.9, 0.5 };

texture ToneTexture;
bool Use2D = false;
bool UseTexture = true;
float DetailAdjustment = 1.0f;

sampler ToonSampler = sampler_state {
	Texture = (ToneTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

bool TextureEnabled;
texture Texture;

bool UseToon = true;

sampler	Sampler = sampler_state {
	Texture = (Texture);
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

// Shader Inputs

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TexCoord : TEXCOORD0;
};

struct PixelShaderInput 
{
	float2 TexCoord : TEXCOORD0;
	float LightAmount : TEXCOORD1;
};

// Shader Outputs

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float LightAmount : TEXCOORD1;
	float Z : TEXCOORD2;
};

struct NormalDepthVertexShaderOutput 
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
};

// Shader code

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	float4 worldNormal = mul(input.Normal, World);
	output.LightAmount = dot(worldNormal, LightDirection);

	output.TexCoord = input.TexCoord;

	output.Z = output.Position.z;

    return output;
}

float4 ToonPixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = TextureEnabled ? tex2D(Sampler, input.TexCoord) : 0;

	if (UseToon == false) {
		return color;
	}

    float4 light;

	float x = (input.LightAmount * 31)/600;
	float y = (input.Z/(5000+DetailAdjustment));

	float4 texSample = tex2D(ToonSampler, float2(x, Use2D ? y : 0));

	light = texSample.rgba;

	if(UseTexture) {
		color.rgb *= light.rgb;
		color.a = light.a;
	} else {
		color.rgba = texSample.rgba;
	}

    return color;
}

NormalDepthVertexShaderOutput NormalDepthVertexShaderFunction(VertexShaderInput input) 
{
	NormalDepthVertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	float3 worldNormal = mul(input.Normal, World);
	output.Color.rgb = (worldNormal + 1) / 2;
	output.Color.a = output.Position.z / output.Position.w;

	return output;
}

float4 NormalDepthPixelShaderFunction(float4 color : COLOR0) : COLOR0
{
	return color;
}

Technique ToonShader
{
	pass P0
	{
		VertexShader = compile vs_1_1 VertexShaderFunction();
		PixelShader = compile ps_2_0 ToonPixelShaderFunction();
	}
}

Technique NormalDepth
{
	pass P0
	{
		VertexShader = compile vs_1_1 NormalDepthVertexShaderFunction();
		PixelShader = compile ps_2_0 NormalDepthPixelShaderFunction();
	}
}
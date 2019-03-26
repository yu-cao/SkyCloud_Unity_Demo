Shader "Unlit/SoftAlphaBlend"
{
	Properties
	{
		_CloudTexture("Cloud Texture", 2D) = "white" {}
		_JitterTex("JitterTex", 2D) = "white" {}

		_AmbientColor("Ambient Color", Color) = (0.2078431,0.2588235,0.2980392,1)
		_SunColor("Sun Color", Color) = (0.972549,0.9215686,0.8784314,1)
		_ButtomColor("Buttom Color", Color) = (0.972549,0.9215686,0.8784314,1)

		_JitterAngle("JitterAngle", Color) = (0,0,0,0)
		_JitterAll("JitterAll", Range(0, 1)) = 1
		_JitterIntensity("JitterIntensity" , Range(0, 5)) = 1
		_CloudGrowth("Cloud Growth", Range(1, 0)) = 0.2620265 //Range(1, -1)
		_CloudContrast("Cloud Contrast", Range(0.1, 3)) = 1
		_Bias("Bias", Range(0.25, 0.95)) = 0.75

		_TransmitExp("TransmitRange" ,  Range(0.0001, 500)) = 1
		_TransmitPow("TransmitPow" ,  Range(1, 3)) = 1
		_TransmitIntensity("TransmitIntensity" , Range(0, 1)) = 1

		_Alpha("Alpha",  Range(0, 5)) = 3
		_AlphaExp("AlphaExp",  Range(0, 2)) = 1
		[HideInInspector]_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}
	SubShader{
		Tags 
		{
			"IgnoreProjector" = "True"
			"Queue" = "Background+30"
			"RenderType" = "Transparent"
		}
		Pass {
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma multi_compile_fwdbase
			#pragma only_renderers d3d9 d3d11 glcore gles gles3 metal xboxone ps4 psp2 n3ds wiiu 
			#pragma target 3.0

			float4 _LightColor0;
			float4 _AmbientColor;
			float4 _SunColor;
			float4 _ButtomColor;
			uniform sampler2D _CloudTexture; uniform float4 _CloudTexture_ST;

			half2 _JitterAngle;
			half _JitterAll;
			half _JitterIntensity;
			sampler2D _JitterTex; float4 _JitterTex_ST;
			float _CloudGrowth;
			float _CloudContrast;
			float _Bias;
			float _Alpha;
			float _AlphaExp;

			float _TransmitExp;
			float _TransmitPow;
			float _TransmitIntensity;

			struct VertexInput
			{
				float4 vertex : POSITION;
				float2 texcoord0 : TEXCOORD0;
			};
			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
			};
			VertexOutput vert(VertexInput v)
			{
				VertexOutput o = (VertexOutput)0;
				o.uv0 = v.texcoord0;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);

				return o;
			}

			float LightTransmit(float3 V, float3 L, float exponent)
			{
				return pow(saturate(dot(V, -L)), exponent);
			}

			float4 frag(VertexOutput i) : COLOR {
				float4 finalColor = float4(1,1,1,1);

				float4 _CloudTexture_var = tex2D(_CloudTexture,TRANSFORM_TEX(i.uv0, _CloudTexture));
				float2 jitterUV = TRANSFORM_TEX(i.uv0, _JitterTex);
				_JitterAngle = (_JitterAngle - 0.5) * 2;
				jitterUV.x += _Time / 3 * _JitterAngle.x;
				jitterUV.y += _Time / 3 * _JitterAngle.y;
				float4 jitterMask = tex2D(_JitterTex, jitterUV);

				float jitterVal = lerp(_CloudTexture_var.r, 1.0, _JitterAll);

				_AmbientColor.rgb = lerp(_ButtomColor.rgb, _AmbientColor.rgb , saturate((_CloudTexture_var.g + 0.5) * 1.4));
				float cloudGradient = saturate(pow(saturate(((_CloudTexture_var.r) + _CloudGrowth - 0.5) * _CloudContrast + 0.5), (-0.3) / log(_Bias)) + jitterMask.r * _JitterIntensity * jitterVal); //* _CloudTexture_var.g
				float3 cloudCol = lerp(_AmbientColor.rgb, _SunColor.rgb, cloudGradient * _CloudTexture_var.g); // _CloudTexture_var.gµ×²¿±ä°µ

				//transmit
				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				float3 ViewDir = normalize(_WorldSpaceCameraPos - i.posWorld);
				float3 transmit = _TransmitIntensity * _LightColor0.xyz * LightTransmit(ViewDir, lightDir, _TransmitExp) * pow(cloudGradient, _TransmitPow);


				finalColor.rgb = cloudCol + transmit;
				finalColor.a = saturate(pow(((1.0 - _CloudTexture_var.r) - _CloudGrowth) * _Alpha, _AlphaExp) - jitterMask.r * _JitterIntensity * jitterVal);

				return finalColor;
			}
			ENDCG
		}
	}
	FallBack "Transparent/Diffuse"
	CustomEditor "ShaderForgeMaterialInspector"
}

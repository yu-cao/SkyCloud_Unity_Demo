Shader "Unlit/Skybox_Cloud"
{
	Properties
	{
		[Header(Cloud)]
		_CloudColor("_CloudColor", Color) = (1, 1, 1, 1)
		_CloudDarkColor("_CloudDarkColor", Color) = (1, 1, 1, 1)
		_CloudMap("_CloudMap", 2D) = "white" {}
		_CloudAmount("_CloudAmount", Range(0, 0.99)) = 0.6
		_CloudPower("_CloudPower", Range(0, 5)) = 1
		_CloudRange("_CloudRange", Range(0, 10)) = 5
		_CloudSpeed("_CloudSpeed", Range(0, 2)) = 0.1
		_CloudContrast("Cloud Contrast", Range(0.1, 3)) = 1
		_Bias("Bias", Range(0.25, 0.95)) = 0.75
		_CloudHeight("_CloudHeight",Range(0.01, 2)) = 1

		_TransmitExp("_TransmitRange" ,  Range(0.0001, 500)) = 100
		_TransmitIntensity("_TransmitIntensity" , Range(0, 10)) = 1
		_TransmitEdge("_TransmitEdge", Range(0, 5)) = 0.6
		_TransmitBase("_TransmitBase", Range(0, 1)) = 0.5

		[Header(CloudLayer)]
		[Toggle] _OnlyCloud("OnlyCloud?",float) = 0
	}
	SubShader
	{
		Tags{
			"IgnoreProjector" = "True"
			"Queue" = "Background+20"
			"RenderType" = "Transparent"
		}

		Pass
		{
			Tags{"LightMode" = "ForwardBase"}
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull OFF

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma multi_compile_fwdbase
			#pragma target 3.0

			#pragma shader_feature  _ONLYCLOUD_OFF _ONLYCLOUD_ON

			struct appdata
			{
				float4 vertex : POSITION;
				fixed2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				float3 localVertex : TEXCOORD1;
				float2 cloudUV : Texcoord2;
			};

			uniform float4 _LightColor0;

			sampler2D _RayleighMap;
			sampler2D _MieMap;

			sampler2D _CloudMap;
			float4 _CloudMap_ST;
			half4 _CloudColor;
			half4 _CloudDarkColor;
			half _CloudAmount;
			half _CloudPower;
			half _CloudRange;

			half _CloudSpeed;
			half _CloudContrast;
			half _Bias;

			half _CloudHeight;

			float _TransmitExp;
			float _TransmitIntensity;
			half _TransmitEdge;
			half _TransmitBase;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.localVertex = v.vertex.xyz;
				o.cloudUV = TRANSFORM_TEX((v.vertex.xz / max(pow(v.vertex.y,_CloudHeight)   , 0.001)), _CloudMap); // max(v.vertex.y, 0.001)  v.vertex.y后边乘/pow个数
				return o;
			}

			// https://www.shadertoy.com/view/ltdXRS
			float cloudyFbm(float2 uv)
			{
				float f = 0.0;
				float2 rotator = (float2(.91, 1.5));

				float2 tmp = uv;
				uv.x = tmp.x * rotator.x - tmp.y * rotator.y;
				uv.y = tmp.x * rotator.y + tmp.y * rotator.x;
				f += tex2D(_CloudMap, uv) * 0.5;
				return f;
			}

			half4 frag(v2f i) : SV_Target
			{
				half4 color;
				color.a = 1.0;

				fixed3 viewDir = normalize(i.localVertex);
				fixed VdotL = dot(-_WorldSpaceLightPos0.xyz, viewDir);
				fixed VdotL2 = VdotL * VdotL;

				//Cloud
				float time = _Time.y * _CloudSpeed;
				float2 uv = i.cloudUV;

				float x = 0.0;
				x += cloudyFbm(0.5 * uv + float2(.1,  -.01) * time) * 0.5;

				//x += cloudyFbm( 1.0 * uv + float2(.12,  .03) * time) * 0.5 * 0.5;
				x += cloudyFbm(2.0 * uv + float2(.15, -.02) * time) * 0.5 * 0.5 * 0.5;
				//x += cloudyFbm( 4.0 * uv + float2(.2,   .01) * time) * 0.5 * 0.5 * 0.5 * 0.5;
				//x += cloudyFbm( 8.0 * uv + float2(.15, -.01) * time) * 0.5 * 0.5 * 0.5 * 0.5 * 0.5;

				float f = 1 - _CloudAmount;
				x = smoothstep(0.0, f, x);
				x = (x - f) / (1.0 - f);
				x *= smoothstep(0.4, 0.55, x);
				x *= saturate((i.localVertex.y - 0.1) * _CloudRange);


				half cloudGradient = pow(saturate((x - 0.5) * _CloudContrast + 0.5), (-0.3) / log(_Bias)); // saturate(pow(x * _CloudContrast, (-0.3) / log(_Bias))) ;
				fixed3 cloudColor = lerp(_CloudColor.rgb, _CloudDarkColor.rgb, cloudGradient);  // _CloudColor.rgb * (x * _CloudPower);

				//transmit	
				half3 transmitCol = pow((-VdotL + 1) / 2, _TransmitExp) * _LightColor0.rgb * _TransmitIntensity;

				//final
				color.rgb = cloudColor + transmitCol * saturate(1 - x * _TransmitEdge + _TransmitBase);
				color.a = clamp(x * 1.5, 0, 1);

				return color;
			}
			ENDCG
		}
	}
}

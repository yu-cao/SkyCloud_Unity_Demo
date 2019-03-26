Shader "Unlit/Skybox"
{
	Properties
	{
		_RayleighMap("RayleighMap", 2D) = "black" {}
		_MieMap("MieMap", 2D) = "white" {}
		_PartialRayleighInScattering("_PartialRayleighInScattering", Color) = (1, 1, 1, 0.1)
		_PartialMieInScattering("_PartialMieInScattering", Color) = (0.15, 0.15, 0.15, 0.8)
		_NightSkyColBase("_NightSkyColBase", Color) = (0, 0.7, 1, 1)
		_NightSkyColDelta("_NightSkyColDelta", Color) = (0.6, 0.75, 0.82, 0.4)

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

		_SunIntensity("_SunIntensity", Range(0, 10)) = 1
		_SunScale("_SunScale", Range(1000, 9000)) = 2000
		_SunHaloIntensity("_SunHaloIntensity", Range(0, 2)) = 0.5
		_TransmitExp("TransmitRange" ,  Range(0.0001, 500)) = 100
		_TransmitIntensity("TransmitIntensity" , Range(0, 10)) = 1
		_TransmitEdge("_TransmitEdge", Range(0, 5)) = 0.6
		_TransmitBase("_TransmitBase", Range(0, 1)) = 0.5

		[Header(CloudLayer)]
		[Toggle] _HasCloud("HasCloud?",float) = 0
		[Toggle] _HasSun("HasSun?",float) = 0
	}
	SubShader
	{
		Tags { "RenderType" = "Background" "Queue" = "Background" "PreviewType" = "Skybox" }
		Cull Off
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma shader_feature  _FLIPLIGHTDIR_OFF _FLIPLIGHTDIR_ON
			#pragma shader_feature  _HASCLOUD_OFF _HASCLOUD_ON
			#pragma shader_feature  _HASSUN_OFF _HASSUN_ON

			#include "UnityCG.cginc"

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

			uniform float4 _SunLightDir;
			uniform float4 _LightColor0;

			sampler2D _RayleighMap;
			sampler2D _MieMap;
			fixed4 _PartialRayleighInScattering;
			fixed4 _PartialMieInScattering;
			fixed4 _NightSkyColBase;
			fixed4 _NightSkyColDelta;

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

			half _SunIntensity;
			half _SunScale;
			half _SunHaloIntensity;

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
				o.cloudUV = TRANSFORM_TEX((v.vertex.xz / max(v.vertex.y, 0.001)), _CloudMap);
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

				fixed4 rayleigh = tex2D(_RayleighMap, i.uv); //地平线贴图
				rayleigh.rgb = rayleigh.rgb * (rayleigh.a * rayleigh.a * _PartialRayleighInScattering.a);
				fixed4 mie = tex2D(_MieMap, i.uv);
				mie.rgb *= (mie.a * mie.a * _PartialRayleighInScattering.a); //地平线颜色变化

				fixed3 viewDir = normalize(i.localVertex);

				fixed VdotL = dot(-_WorldSpaceLightPos0.xyz, viewDir);
				fixed VdotL2 = VdotL * VdotL;

				fixed VdotLSun = dot(-_SunLightDir.xyz, viewDir);
				fixed VdotL2Sun = VdotLSun * VdotLSun;


				half3 horizonCol = _PartialRayleighInScattering.rgb * rayleigh.rgb * ((VdotL2Sun + 1.0) * 0.75);  //地平线变亮颜色 * 太阳前后两方向   *_SunIntensity

				fixed viewY = saturate(viewDir.y * _NightSkyColBase.a - _NightSkyColDelta.a);
				half3 skyCol = horizonCol + lerp(_NightSkyColBase.rgb, _NightSkyColDelta.rgb, viewY * (2.0 - viewY));
				skyCol = clamp(skyCol,0 ,_PartialMieInScattering.a);  //SkyColor

				//Sun
				half3 sunCol = pow((-VdotL + 1) / 2, _SunScale) * _SunIntensity * _LightColor0.rgb; //horizonMulti  //   * lerp( 1.0 , horizonCol, saturate(rayleigh.r *((VdotL2 + 1.0)) * 20) 
				half3 sunHalo = pow((-VdotL + 1) / 2, _SunScale / 10) * _SunIntensity * _SunHaloIntensity * _LightColor0.rgb; //horizonMulti *

				//Cloud
				float time = _Time.y * _CloudSpeed;
				float2 uv = i.cloudUV;

				#if _HASCLOUD_ON
					float x = 0.0;
					x += cloudyFbm(0.5 * uv + float2(.1, -.01) * time) * 0.5;

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
				#endif

					//final	
				#if _HASCLOUD_ON
					color.rgb = lerp(skyCol + (sunCol + sunHalo) * (1 - x), cloudColor + transmitCol * saturate(1 - x * _TransmitEdge + _TransmitBase), clamp(x * 1.5, 0, 1));
				#else
					#if _HASSUN_ON
						color.rgb = skyCol + (sunCol + sunHalo);
					#else
						color.rgb = skyCol + sunHalo;
					#endif
				#endif


				return color;
			}
			ENDCG
		}
	}
}

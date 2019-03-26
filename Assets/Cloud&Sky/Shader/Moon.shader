Shader "Unlit/Moon"
{
	Properties
	{
		_MoonTex("_MoonTex", 2D) = "white" {}
		_SunIntensity("_SunIntensity", Range(0, 10)) = 1
		_MoonColor("_MoonColor", color) = (1,1,1,1)
	}

	SubShader
	{
		Tags{
			"IgnoreProjector" = "True"
			"Queue" = "Background+10"
			"RenderType" = "Transparent"
		}
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull OFF

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma multi_compile_fwdbase
			#pragma target 3.0

			uniform half4 _MoonColor;
			sampler2D _MoonTex;

			half _SunIntensity;
			struct appdata
			{
				float4 vertex : POSITION;
				fixed2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				float2 cloudUV : Texcoord2;
			};

			v2f vert(appdata v)
			{
				v2f o; UNITY_INITIALIZE_OUTPUT(v2f, o);

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			half4 frag(v2f i) : SV_Target
			{
				half4 color;
				color.a = 1.0;

				//half3 = light0. rotation.x 
				//half worldY = saturate(i.posWorld.g / 10000) ; //saturate(i.posWorld.g / moonDistance) 

				half4 MoonCol = tex2D(_MoonTex, i.uv);
				//color.rgb = MoonCol.rgb * _SunIntensity * _LightColor0.rgb; //  lerp( fixed3(1,1,1) , _PartialRayleighInScattering.rgb, pow(1 - worldY, 3))  //改用灯光颜色直接控制
				color.rgb = MoonCol.rgb * _SunIntensity * _MoonColor.rgb; //改用TimeOfDay脚本控制

				color.a = MoonCol.a;
				return color;

				//color.rgb = lerp(skyCol + (sunCol + sunHalo)*(1 - x), cloudColor + transmitCol * saturate(1 - x * _TransmitEdge + _TransmitBase), clamp(x*1.5, 0, 1));

				return color;
			}
			ENDCG
		}
	}
}
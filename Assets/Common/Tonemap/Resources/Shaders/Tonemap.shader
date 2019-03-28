Shader "Hidden/ScreenDroplets"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f
			{
				float4 vertex 		: SV_POSITION;
				float2 texcoord		: TEXCOORD0;
				float4 screenPos	: TEXCOORD1;

				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D 	_MainTex;
			float4		_MainTex_TexelSize;

			float		_Exposure;
			float		_Gamma;

			v2f vert (appdata_full v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.vertex 	= UnityObjectToClipPos(v.vertex);
				o.texcoord 	= v.texcoord;
				o.screenPos = ComputeScreenPos(o.vertex);

				return o;
			}

			float3 Tonemap (float3 color)
			{
				color = max(0, color - 0.004);
				color = (color * (6.2 * color + 0.5)) / (color * (6.2 * color + 1.7)+ 0.06);

				return pow(color, _Gamma);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				#if UNITY_SINGLE_PASS_STEREO
					float2 uv = i.screenPos.xy / i.screenPos.w;
				#else
					float2 uv = i.texcoord;
				#endif

				float3 color = tex2D(_MainTex, uv).rgb;

				color = Tonemap(color * _Exposure);
				
				return float4(color, 1);
			}
			ENDCG
		}
	}
}
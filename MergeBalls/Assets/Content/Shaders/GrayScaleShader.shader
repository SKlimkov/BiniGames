Shader "Sprites/GrayScaleTurnable" 
{
	Properties 
	{
		[PerRendererData] _MainTex("Sprite Texture (RGB)", 2D) = "white" {}
		_IsColorized("IsColorized", Float) = 1
	}

	SubShader 
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		// required for UI.Mask
		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Mode Off }
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			sampler2D	_MainTex;
			fixed		_IsColorized;

			float GetLuminance(fixed4 tex)
			{
				return (dot(tex, fixed4(0.2126, 0.7152, 0.0722, 0)));
			}

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif
				return OUT;
			}

			fixed4 frag(v2f IN) : COLOR
			{
				fixed4 output;
				fixed4 mainTex = tex2D(_MainTex, IN.texcoord) * IN.color;
				fixed luminance = GetLuminance(mainTex);
				fixed4 lumColor = fixed4(luminance, luminance, luminance, mainTex.a);
				//output = fixed4(mainTex.x * luminance, mainTex.y * luminance, mainTex.z * luminance, mainTex.a);
				//output = mainTex * fixed4(luminance, luminance, luminance, mainTex.a);
				//output = fixed4(mainTex.x, mainTex.y, mainTex.z, mainTex.a);
				output = lerp(lumColor, mainTex, _IsColorized);

				output.rgb *= output.a;
				return output;
			}

			ENDCG
		}		
	}
	FallBack "Sprites/Default"
}
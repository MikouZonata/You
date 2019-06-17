Shader "Unlit/BillboardShader Maan"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_OverlayTex("Overlay", 2D) = "white" {}
		_Intensity("Intensity", Range(0,5)) = 1
		_Tint("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "DisableBatching"="True" }
 
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag      
            #include "UnityCG.cginc"
 
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
 
            sampler2D _MainTex;
			sampler2D _OverlayTex;
			float _Intensity;
			float4 _Tint;
            float4 _MainTex_ST;
           
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = mul(UNITY_MATRIX_P,
                mul(UNITY_MATRIX_MV, float4(0, 0, 0, 1)) + float4(v.vertex.x, v.vertex.y, 0, 0)) ;
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
           
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * tex2D(_OverlayTex, i.uv) * _Intensity + _Tint;
				col.a = tex2D(_MainTex, i.uv).a;
				if (col.a == 0.0)
					discard;
                return col;
            }
            ENDCG
        }
    }
}
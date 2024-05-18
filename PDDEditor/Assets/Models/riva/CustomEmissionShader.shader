Shader "HDRP/CustomEmissionShader"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _EmissionMap ("Emission Map", 2D) = "white" {}
        _HighlightColor ("Highlight Color", Color) = (1,1,1,1)
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _EmissionIntensity ("Emission Intensity", Range(0, 10)) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
             
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _HighlightColor;
            float4 _EmissionColor;
            sampler2D _BaseMap;
            sampler2D _EmissionMap;
            float _EmissionIntensity;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 baseMapColor = tex2D(_BaseMap, i.uv);
                fixed4 emissionMapColor = tex2D(_EmissionMap, i.uv);
                fixed4 highlightColor = emissionMapColor == _HighlightColor ? _EmissionColor * _EmissionIntensity : emissionMapColor;

                return baseMapColor * highlightColor;
            }
            ENDCG
        }
    }
}

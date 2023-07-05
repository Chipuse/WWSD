Shader "Unlit/GemGlow"
{
    Properties
    {
        _GemTex ("Gem Texture", 2D) = "white" {}
        _BehindGlow ("Behind Glow", 2D) = "white" {}
        _FrontGlow ("Front Glow", 2D) = "white" {}
        _FrontGlowRange ("Front Glow Range", Range(0,0.5)) = 0.2
        _FrontGlowRangeMin ("Front Glow Range Min", Range(0,0.5)) = 0.1
        _GlowCol ("Glow Color", Color)= (1,1,1,1)
        _TintVal ("Tint Value", Range(0,1))=0.5

    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _GemTex;
                        float4 _GemTex_ST;

            sampler2D _BehindGlow;
            sampler2D _FrontGlow;
            fixed4 _GlowCol;
            float _FrontGlowRange;
            float _FrontGlowRangeMin;
            float _TintVal;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _GemTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 gemCol = tex2D(_GemTex, i.uv);
                fixed4 bGlow=tex2D(_BehindGlow,i.uv);
                float minPulse= 0;
                float maxPulse=1-minPulse;
                float pulseVal=maxPulse*((sin(_Time.y)+1)/2)+minPulse;
                float tintVal=_TintVal;
                
                fixed4 pulseCol= tex2D(_FrontGlow,i.uv);
                pulseCol*=_FrontGlowRangeMin;
                tintVal+=lerp(pulseCol.r,_FrontGlowRange,pulseVal);;
                
                _GlowCol=lerp(fixed4(0,0,0,0),_GlowCol,tintVal);
                gemCol+=fixed4(_GlowCol.r,_GlowCol.g,_GlowCol.b,0);
                
                

                
                
                if(gemCol.a>0){
                return gemCol;}
                else{
                
                bGlow=fixed4(_GlowCol.r,_GlowCol.g,_GlowCol.b,bGlow.r*pulseVal*tintVal*2);
                return bGlow;
                }
            }
            ENDCG
        }
    }
}

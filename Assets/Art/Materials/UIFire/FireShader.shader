Shader "Unlit/FireShader"
{
    Properties
    {
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _FadeoutTex ("Fade Out Texture", 2D) = "white" {}
        _GradientTex ("Gradient", 2D) = "white" {}
        _MaskCutOff ("MaskCutOff", float) = 0.5
        _Speed ("Speed", float) =0.5
        _TempSlider ("Temperature K", Range(0,12000)) =2000
        _Intensity ("Intensity", Range(0,10)) =1
        _WobbleStrength ("Wobble Strength", float)=1
        _Color1 ("Color 1", Color) =(1,1,1,1)
        _Color2 ("Color 2", Color) =(1,1,1,1)
                
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        
        ZWrite Off
        Blend One One

        Pass
        {
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MaskTex;
            sampler2D _FadeoutTex;
            sampler2D _GradientTex;
            float4 _MaskTex_ST;
            float _MaskCutOff;
            float _Speed;
            float _TempSlider;
            float _Intensity;
            float _WobbleStrength;
            
            fixed4 _Color1;
            fixed4 _Color2;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
            
            
            float t= _Time.y;
            float wobble= sin(t*3-i.uv.y*10)*_WobbleStrength*0.1*i.uv.y;
            float fade=tex2D (_FadeoutTex,i.uv+ float2(wobble,0)).r;
            float2 maskUV =i.uv + float2(0,t*0.04*_Speed);
            float2 maskUV2 =i.uv + float2(0,t*0.07*_Speed);
            
                // sample the texture
                fixed4 col = tex2D(_MaskTex, float2 (maskUV.x+wobble,maskUV.y));
                col *= tex2D(_MaskTex, float2 (-maskUV2.x-wobble,-maskUV2.y+0.4));

                
//               if(col.r <_MaskCutOff*fade){
//                col =1-col;
//                }
//                else{
//                col=0;
//                }
                
                col*= fade;
                /*

                
                float middle=wobble+0.5;
                float lerpFac=i.uv.x+middle;
                if(lerpFac>1){
                lerpFac=2-lerpFac;
                }
                lerpFac=(lerpFac-middle)*2;
                col*= lerp(col,fade,lerpFac);
                
                */
                
//                float tempscale= _TempSlider/12000;
                float gradientUV= col.r;
             float4 col2 = lerp(_Color1, _Color2, gradientUV);
//                if (col.a <= 0){
//                return 1;
//                }
                return col2*col.r*_Intensity;
            }
            ENDCG
        }
    }
}

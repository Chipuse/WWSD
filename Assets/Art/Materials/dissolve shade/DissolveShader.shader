Shader "Unlit/DissolveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PaperTex ("Paper Texture", 2D) = "white" {}
        _TimeSinceLoad ("Time since level has loaded", float) =0
        _Speed ("Dissolve Speed", float) = 0.5
        [HDR]_HDRCol ("HDR Color", Color) = (1,1,1,1)
        _Border ("HDR Border", float) =0.1
        _OGSprite ("Original Sprite", 2D) = "white" {}
        _OGSpriteMask ("Transparency Reference", 2D)="white"{}
        _BlackBorder ("Black Border", Range(0,1))=0.2
        _CenterPointX ("Center Point X", float) =0.5
        _CenterPointY ("Center Point Y", float) =0.5
        _TilingRatio ("Tiling Ratio", float) = 1
        
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

            sampler2D _MainTex;
            sampler2D _PaperTex;
            float4 _MainTex_ST;
            float _TimeSinceLoad;
            float _Speed;
            float _Border;
            fixed4 _HDRCol;
            sampler2D _OGSprite;
            sampler2D _OGSprite_ST;
            sampler2D _OGSpriteMask;
            float _BlackBorder;
            float _CenterPointX;
            float _CenterPointY;
            float _TilingRatio;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, float2(i.uv.x,i.uv.y*_TilingRatio)+0.5)*tex2D(_MainTex,float2(i.uv.x,i.uv.y*_TilingRatio)*2);
                //col*=tex2D(_PaperTex, float2(i.uv.x,i.uv.y*_TilingRatio));
                fixed4 returnCol= tex2D(_OGSprite, i.uv);
                fixed4 transCheck = tex2D(_OGSpriteMask,i.uv);
                
                float t =(_Time.y-_TimeSinceLoad)*_Speed;
                
                returnCol.a=transCheck.a;
                
                float dis= distance(i.uv, float2(_CenterPointX,_CenterPointY));
                if(dis<0){
                dis*=-1;
                }
                
                float checkValue=col.r+(dis*(1-t))+_BlackBorder;
                
                if(checkValue<t||returnCol.a==0){
return fixed4(0,0,0,0);
                }
                else if(checkValue<(t+_Border)){
                
                returnCol= _HDRCol;
                returnCol*=(_Border+t-checkValue)*(1/_Border);

                returnCol.a=(checkValue-t)*(1/(_Border*0.4));

                }
                else if(checkValue<(t+_Border+_BlackBorder)){
                returnCol*=(checkValue-t)*(1/(_BlackBorder*1.5));
                returnCol.a=1;
                }
                
                
                
                return returnCol;
            }
            ENDCG
        }
    }
}

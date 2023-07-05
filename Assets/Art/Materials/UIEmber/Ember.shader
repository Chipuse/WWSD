Shader "Unlit/Ember"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex("Noise texture", 2D)="white"{}
        [HDR]_HDRCol ("Ember Color", Color)=(1,1,1,1)
        _Col2 ("Darker Color", Color)=(1,1,1,1)
        _Speed ("Speed", float)=0.2
        _Intensity ("Intensity", float)=5.0
        _Size("Size",float)=1
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            fixed4 _HDRCol;
            fixed4 _Col2;
            float _Speed;
            float _Intensity;
            float _Size;
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
            
            float t= _Time.y*_Speed;
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float alpha =col.a;
                
                
                fixed4 embCol= tex2D(_NoiseTex, float2(i.uv.x*cos(t)-i.uv.y*sin(t)+t,i.uv.x*sin(t)+i.uv.y*cos(t)+t)*_Size);
                embCol*=tex2D(_NoiseTex, float2(i.uv.x*sin(t)+i.uv.y*cos(t)-t,i.uv.x*cos(t)-i.uv.y*sin(t)-t)*_Size)*_Intensity;
                
                
                
                //embCol*=_HDRCol;
                
                embCol=lerp(_Col2,_HDRCol,embCol.r);
                
                
                embCol.a=alpha;



                return embCol;
            }
            ENDCG
        }
    }
}

Shader "Unlit/UI_SV_Gradient"
{
    Properties
    {
        _Hue ("Hue", Range(0, 1)) = 0.0

        [IntRange] _Stencil("Stencil ID", Range(0,255)) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilComp("Stencil Comparison", Float) = 8
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
 
        Stencil 
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        ColorMask [_ColorMask]

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

            float _Hue;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            void Remap(float In, float2 InMinMax, float2 OutMinMax, out float Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }

            void HsvToLinear(float3 In, out float3 Out)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
                float3 RGB = In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
                float3 linearRGBLo = RGB / 12.92;
                float3 linearRGBHi = pow(max(abs((RGB + 0.055) / 1.055), 1.192092896e-07), float3(2.4, 2.4, 2.4));
                Out = float3(RGB <= 0.04045) ? linearRGBLo : linearRGBHi;
            }

            float3 frag (v2f i) : SV_Target
            {
                float3 col;

                float2 o1 = float2(0.0, 1.0);
                
                float s;
                Remap(i.uv.x, float2(0.5, 1.0), o1, s);
                s = 1 - clamp(s, o1.x, o1.y);

                float v;
                Remap(i.uv.x, float2(0.0, 0.5), o1, v);
                v = clamp(v, o1.x, o1.y);


                HsvToLinear(float3(_Hue, s, v), col);

                return col;
            }
            ENDCG
        }
    }
}

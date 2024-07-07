Shader "Custom/UIArcadeEffectWithBadTV"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _ScanLineIntensity ("Scan Line Intensity", Range(0, 1)) = 0.1
        _FlickerIntensity ("Flicker Intensity", Range(0, 1)) = 0.05
        _CurvatureIntensity ("Curvature Intensity", Range(0, 1)) = 0.1
        _ColorShift ("Color Shift", Range(0, 0.1)) = 0.005
        _BadTVIntensity ("Bad TV Intensity", Range(0, 1)) = 0.1
        _BadTVSpeed ("Bad TV Speed", Range(0, 10)) = 1
        _BadTVFrequency ("Bad TV Frequency", Range(0, 50)) = 10

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;

            sampler2D _MainTex;
            float _ScanLineIntensity;
            float _FlickerIntensity;
            float _CurvatureIntensity;
            float _ColorShift;
            float _BadTVIntensity;
            float _BadTVSpeed;
            float _BadTVFrequency;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.worldPosition = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            float2 badTVEffect(float2 uv, float time)
            {
                float distortion = sin(uv.y * _BadTVFrequency + time * _BadTVSpeed) * _BadTVIntensity;
                return float2(uv.x + distortion, uv.y);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half2 uv = IN.texcoord;
                
                // Bad TV effect
                uv = badTVEffect(uv, _Time.y);
                
                // Curvature effect
                float2 curvedUV = (uv - 0.5) * (1.0 - _CurvatureIntensity * (uv.y - 0.5) * (uv.y - 0.5)) + 0.5;
                
                // Sample the texture
                half4 color = (tex2D(_MainTex, curvedUV) + _TextureSampleAdd) * IN.color;
                
                // Scan lines
                float scanLine = sin(curvedUV.y * 800.0) * 0.5 + 0.5;
                color *= 1.0 - _ScanLineIntensity * scanLine;
                
                // Flicker
                float flicker = random(float2(_Time.y, _Time.y));
                color *= 1.0 - _FlickerIntensity * flicker;
                
                // Color shift
                float2 shift = float2(_ColorShift * sin(_Time.y * 5.0), _ColorShift * cos(_Time.y * 3.0));
                half4 shiftedColor = tex2D(_MainTex, curvedUV + shift) * IN.color;
                color.r = lerp(color.r, shiftedColor.r, 0.5);
                color.b = lerp(color.b, shiftedColor.b, 0.5);
                
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                
                return color;
            }
            ENDCG
        }
    }
}
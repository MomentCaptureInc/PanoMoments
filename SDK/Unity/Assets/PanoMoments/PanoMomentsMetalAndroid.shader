// 
//   PanoMoments
//   Copyright (c) 2019 Moment Capture Inc.
//

Shader "PanoMoments/PanoMomentsMetalAndroid" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
    }

    // For Android
    SubShader {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Fog { Mode off }
		LOD 100
        ZTest Always
		ZWrite Off
		Lighting Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        

        Pass {
            GLSLPROGRAM
            #pragma only_renderers gles gles3
            #extension GL_OES_EGL_image_external : require
            #extension GL_OES_EGL_image_external_essl3 : enable
            #include "UnityCG.glslinc"

            #ifdef VERTEX
            uniform mat4 _SampleTransform;
            varying vec2 uv;

            void main() {
                gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
                uv = (_SampleTransform * gl_MultiTexCoord0.xyzw).xy;
            }
            #endif  // VERTEX


            #ifdef FRAGMENT
            uniform samplerExternalOES _MainTex;
            uniform lowp vec4 _Color;
            varying vec2 uv;

            void main() {
                gl_FragColor = texture2D(_MainTex, uv) * _Color;
            }
            #endif // FRAGMENT

            ENDGLSL
        }
    }

    // For every other platform
    SubShader {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Fog { Mode off }
        LOD 100
        ZTest Always
        ZWrite Off
        Lighting Off
        Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma exclude_renderers gles3 gles
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv.y = 1.0 - o.uv.y;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                return tex2D(_MainTex, i.uv) * _Color;
            }
            ENDCG
        }
    }
    Fallback "Unlit/Texture"
}
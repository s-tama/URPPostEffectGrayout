Shader "Custom/Grayout"
{
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    struct VSInput
    {
        float4 pos : POSITION;
        float2 uv : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct PSInput
    {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
        UNITY_VERTEX_OUTPUT_STEREO
    };

    TEXTURE2D_X(_MainTex);
    SAMPLER(sampler_MainTex);
    half _GrayoutPower;

    PSInput GrayoutVert(VSInput i) 
    {
        PSInput o = (PSInput)0;
        UNITY_SETUP_INSTANCE_ID(i);
        UNITY_TRANSFER_INSTANCE_ID(i, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        o.pos = TransformObjectToHClip(i.pos.xyz);
        o.uv = i.uv;
        return o;
    }

    half4 GrayoutFrag(PSInput i) : SV_TARGET
    {
        UNITY_SETUP_INSTANCE_ID(i);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

        half4 col = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, i.uv);
        col.rgb *= _GrayoutPower;
        return col;
    }
    ENDHLSL

    Properties
    {
        [HideInInspector] _MainTex("Base Texture", 2D) = "white" {}
        [HideInInspector] _GrayoutPower("Grayout Power", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Cull Off 
        ZWrite Off 
        ZTest Always

        Pass
        {
            Name "Grayout"

            HLSLPROGRAM
            #pragma vertex GrayoutVert
            #pragma fragment GrayoutFrag
            ENDHLSL
        }
    }
}

namespace Aetherium.Interface;

public static class Shaders
{
    public const string MCG_FRAG = """
#pragma clang diagnostic ignored "-Wmissing-prototypes"

#include <metal_stdlib>
#include <simd/simd.h>

using namespace metal;

constant float _92_tmp [[function_constant(0)]];
constant float _92 = is_function_constant_defined(_92_tmp) ? _92_tmp : 1.39999997615814208984375;
constant float _93_tmp [[function_constant(1)]];
constant float _93 = is_function_constant_defined(_93_tmp) ? _93_tmp : 1.0;
constant float _94_tmp [[function_constant(2)]];
constant float _94 = is_function_constant_defined(_94_tmp) ? _94_tmp : 1.0;
constant float _95_tmp [[function_constant(3)]];
constant float _95 = is_function_constant_defined(_95_tmp) ? _95_tmp : 1.0;
constant float3 _97 = float3(_93, _94, _95);
constant float _98_tmp [[function_constant(4)]];
constant float _98 = is_function_constant_defined(_98_tmp) ? _98_tmp : 1.39999997615814208984375;
constant float _99_tmp [[function_constant(5)]];
constant float _99 = is_function_constant_defined(_99_tmp) ? _99_tmp : 1.39999997615814208984375;
constant int _101_tmp [[function_constant(6)]];
constant int _101 = is_function_constant_defined(_101_tmp) ? _101_tmp : 2;

struct _102
{
    float2 _m0;
};

struct F_MainPS_out
{
    float4 m_243 [[color(0)]];
};

struct F_MainPS_in
{
    float2 m_240 [[user(locn0)]];
};

static inline __attribute__((always_inline))
float3 _136(thread const float3& _138)
{
    return (_138.xyz * float3(exp2(_92))) * _97;
}

static inline __attribute__((always_inline))
float _104(thread const float3& _107)
{
    switch (_101)
    {
        case 0:
        {
            return dot(_107, float3(0.333000004291534423828125));
        }
        case 1:
        {
            return fast::max(_107.x, fast::max(_107.y, _107.z));
        }
        case 2:
        {
            return dot(_107, float3(0.2989999949932098388671875, 0.58700001239776611328125, 0.114000000059604644775390625));
        }
        case 3:
        {
            return exp2(length(log2(_107)));
        }
    }
    return 0.0;
}

static inline __attribute__((always_inline))
float3 _146(thread const float3& _147)
{
    float3 _149 = _147.xyz;
    float _153 = _104(_149);
    return float3(_153) + ((_147.xyz - float3(_153)) * float3(_98));
}

static inline __attribute__((always_inline))
float3 _164(thread float3& _165)
{
    _165 = log2(_165 + float3(9.9999997473787516355514526367188e-06));
    _165 = float3(0.180000007152557373046875) + ((_165 - float3(0.180000007152557373046875)) * float3(_99));
    return fast::max(float3(0.0), exp2(_165) - float3(9.9999997473787516355514526367188e-06));
}

static inline __attribute__((always_inline))
float4 _184(thread const float4& _186, thread const float2& _187, texture2d<float> _91, sampler _91Smplr)
{
    float4 _192 = _91.sample(_91Smplr, _187);
    float3 _193 = _192.xyz;
    float3 _196 = _136(_193);
    _192 = float4(_196.x, _196.y, _196.z, _192.w);
    float3 _199 = _192.xyz;
    float3 _202 = _146(_199);
    _192 = float4(_202.x, _202.y, _202.z, _192.w);
    float3 _205 = _192.xyz;
    float3 _208 = _164(_205);
    _192 = float4(_208.x, _208.y, _208.z, _192.w);
    return _192;
}

fragment F_MainPS_out F_MainPS(F_MainPS_in in [[stage_in]], texture2d<float> _91 [[texture(0)]], sampler _91Smplr [[sampler(0)]], float4 gl_FragCoord [[position]])
{
    float4 _234 = float4(0.0);
    float2 _238 = float2(0.0);
    F_MainPS_out out = {};
    _234 = gl_FragCoord;
    _238 = in.m_240;
    out.m_243 = _184(_234, _238, _91, _91Smplr);
    return out;
}


""";
    
    public const string MCG_VERTEX = """
#pragma clang diagnostic ignored "-Wmissing-prototypes"

#include <metal_stdlib>
#include <simd/simd.h>

using namespace metal;

//constant float _92_tmp [[function_constant(0)]];
constant float _92 =  1.39999997615814208984375;
//constant float _93_tmp [[function_constant(1)]];
constant float _93 =  1.0;
//constant float _94_tmp [[function_constant(2)]];
constant float _94 =  1.0;
//constant float _95_tmp [[function_constant(3)]];
constant float _95 =  1.0;
constant float3 _97 = float3(_93, _94, _95);
//constant float _98_tmp [[function_constant(4)]];
constant float _98 = 1.39999997615814208984375;
//constant float _99_tmp [[function_constant(5)]];
constant float _99 =  1.39999997615814208984375;
//constant int _101_tmp [[function_constant(6)]];
constant int _101 =  2;

struct _102
{
    float2 _m0;
};

struct F_PostProcessVS_out
{
    float2 m_226 [[user(locn0)]];
    float4 gl_Position [[position]];
    float gl_PointSize [[point_size]];
};

static inline __attribute__((always_inline))
void _62(thread const uint& _66, thread float4& _67, thread float2& _68)
{
    _68.x = (_66 == 2u) ? 2.0 : 0.0;
    _68.y = (_66 == 1u) ? 2.0 : 0.0;
    _67 = float4((_68 * float2(2.0, -2.0)) + float2(-1.0, 1.0), 0.0, 1.0);
}

vertex F_PostProcessVS_out F_PostProcessVS(uint gl_VertexIndex [[vertex_id]])
{
    uint _215 = 0u;
    float4 _220 = float4(0.0);
    float2 _224 = float2(0.0);
    F_PostProcessVS_out out = {};
    _215 = gl_VertexIndex;
    _62(_215, _220, _224);
    out.gl_Position = _220;
    out.m_226 = _224;
    out.gl_PointSize = 1.0;
    return out;
}
""";
    
}
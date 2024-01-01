
sampler2D _MainTex;
sampler3D _DetailTex;
float jitter;
float _NoiseScale;
half4 _Color;

float3 _SunDir;
float _ShadowIntensity;
float _DeepObscurance;
float _LightDiffusionIntensity, _LightDiffusionPower;
float3 _WindDirection;
half4 _LightColor;
half  _Density;

float4 _BoundsBorder;
float _BoundsVerticalOffset;

#define BORDER_SIZE_SPHERE _BoundsBorder.x
#define BORDER_START_SPHERE _BoundsBorder.y
#define BORDER_SIZE_BOX _BoundsBorder.xz
#define BORDER_START_BOX _BoundsBorder.yw

half4 _DetailData; // x = strength, y = offset, z = scale, w = importance
half4 _DetailColor;
#define DETAIL_STRENGTH _DetailData.x
#define DETAIL_OFFSET _DetailData.y
#define DETAIL_SCALE _DetailData.z
#define USE_BASE_NOISE _DetailData.w

float4 _RayMarchSettings;
  
#define FOG_STEPPING _RayMarchSettings.x
#define DITHERING _RayMarchSettings.y
#define JITTERING _RayMarchSettings.z
#define MIN_STEPPING _RayMarchSettings.w

float loop_t;

#include "PointLights.cginc"
#include "FogVoids.cginc"
#include "FogOfWar.cginc"
#include "FogDistance.cginc"
#include "Surface.cginc"


void SetJitter(float4 scrPos) {
    //Jitter = frac(dot(float2(2.4084507, 3.2535211), (scrPos.xy / scrPos.w) * _ScreenParams.xy));

    float2 uv = (scrPos.xy / scrPos.w) * _ScreenParams.xy;
    const float3 magic = float3( 0.06711056, 0.00583715, 52.9829189 );
    jitter = frac( magic.z * frac( dot( uv, magic.xy ) ) );
}


inline float3 ProjectOnPlane(float3 v, float3 planeNormal) {
    float sqrMag = dot(planeNormal, planeNormal);
    float dt = dot(v, planeNormal);
	return v - planeNormal * dt / sqrMag;
}

inline float3 GetRayStart(float3 wpos) {
    float3 cameraPosition = GetCameraPositionWS();
    #if defined(ORTHO_SUPPORT)
	    float3 cameraForward = UNITY_MATRIX_V[2].xyz;
	    float3 rayStart = ProjectOnPlane(wpos - cameraPosition, cameraForward) + cameraPosition;
        return lerp(cameraPosition, rayStart, unity_OrthoParams.w);
    #else
        return cameraPosition;
    #endif
}


half4 SampleDensity(float3 wpos) {

    float3 boundsCenter = _BoundsCenter;
    float3 boundsExtents = _BoundsExtents;

    SurfaceApply(boundsCenter, boundsExtents);

#if V2F_DETAIL_NOISE
    half detail = tex3Dlod(_DetailTex, float4(wpos * DETAIL_SCALE + _WindDirection, 0)).a;
    half4 density = _DetailColor;
    if (USE_BASE_NOISE) {
        wpos.xyz -= boundsCenter;
        wpos.y /= boundsExtents.y;
        density = tex2Dlod(_MainTex, float4(wpos.xz * _NoiseScale + _WindDirection.xz, 0, 0));
        density.a -= abs(wpos.y);
    }
    density.a += (detail + DETAIL_OFFSET) * DETAIL_STRENGTH;
#else
    wpos.xyz -= boundsCenter;
    wpos.y /= boundsExtents.y;
    half4 density = tex2Dlod(_MainTex, float4(wpos.xz * _NoiseScale + _WindDirection.xz, 0, 0));
    density.a -= abs(wpos.y);
#endif

    return density;
}


#define dot2(x) dot(x,x)

void AddFog(float3 rayStart, float3 wpos, float rs, half4 baseColor, inout half4 sum) {

   half4 density = SampleDensity(wpos);

   #if VF2_VOIDS
        density.a -= ApplyFogVoids(wpos);
   #endif

   #if V2F_SHAPE_SPHERE
        float3 delta = wpos - _BoundsCenter;
        delta.y += _BoundsVerticalOffset;
        float distSqr = dot2(delta);
        float border = 1.0 - saturate( (distSqr - BORDER_START_SPHERE) / BORDER_SIZE_SPHERE );
        density.a *= border * border;
   #else
        float2 dist2 = abs(wpos.xz - _BoundsCenter.xz);
        float2 border2 = saturate( (dist2 - BORDER_START_BOX) / BORDER_SIZE_BOX );
        float border = 1.0 - max(border2.x, border2.y);
        density.a *= border * border;
   #endif


   if (density.a > 0) {
        half4 fgCol = baseColor * half4((1.0 - density.a * _DeepObscurance).xxx, density.a);
        //fgCol.rgb += GetPointLights(wpos);
        #if VF2_RECEIVE_SHADOWS
            half shadowAtten = GetLightAttenuation(wpos);
            fgCol.rgb *= lerp(1.0, shadowAtten, _ShadowIntensity);
        #endif
        fgCol.rgb *= density.rgb * fgCol.aaa;
        #if VF2_FOW
            fgCol *= ApplyFogOfWar(wpos);
        #endif
		#if VF2_DISTANCE
			fgCol *= ApplyFogDistance(rayStart, wpos);
		#endif

        fgCol *= min(1.0, _Density * rs);
        sum += fgCol * (1.0 - sum.a);
   }
}


half4 GetFogColor(float3 rayStart, float3 viewDir, float t0, float t1) {

    float rs = MIN_STEPPING + max(log(t1-t0), 0) / FOG_STEPPING;     // stepping ratio with atten detail with distance
	float t = t0 + jitter * JITTERING;
    half4 sum = half4(0,0,0,0);
    float diffusion = 1.0 + pow(max(dot(viewDir, _SunDir.xyz), 0), _LightDiffusionPower) * _LightDiffusionIntensity;
    half3 diffusionColor = _LightColor.rgb * diffusion;
    half4 lightColor = half4(diffusionColor, 1.0);

    float3 wpos = rayStart + viewDir * t;

    SurfaceComputeEndPoints(wpos, rayStart + viewDir * t1);

    wpos.y -= _BoundsVerticalOffset;
    viewDir *= rs;

    #if V2F_SURFACE
        float stepEnergy = rs;
        t /= t1;
        rs /= t1;
        // Uncomment the UNITY_UNROLLX line below to support shadows on WebGL 2.0 and also adjust 50 number (increase if needed)
        // UNITY_UNROLLX(50)
        while (t < 1.0) {
            loop_t = t;
            AddFog(rayStart, wpos, stepEnergy, lightColor, sum);
            if (sum.a > 0.99) break;
            t += rs;
            wpos += viewDir;
        }
	    AddFog(rayStart, wpos, t1 * (rs - (t-1.0)), lightColor, sum);
    #else
        // Uncomment the UNITY_UNROLLX line below to support shadows on WebGL 2.0 and also adjust 50 number (increase if needed)
        // UNITY_UNROLLX(50)
        while (t < t1) {
            AddFog(rayStart, wpos, rs, lightColor, sum);
            if (sum.a > 0.99) break;
            t += rs;
            wpos += viewDir;
        }
	    AddFog(rayStart, wpos, rs - (t-t1), lightColor, sum);
    #endif
	sum += (jitter - 0.5) * DITHERING;
    sum *= _LightColor.a;
    return sum;
}
//------------------------------------------------------------------------------------------------------------------
// Volumetric Fog & Mist 2
// Created by Kronnect
//------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace VolumetricFogAndMist2 {

    public enum VolumetricFogShape {
        Box,
        Sphere
    }

    [ExecuteInEditMode]
    public partial class VolumetricFog : MonoBehaviour {

        public VolumetricFogProfile profile;

        public bool enablePointLights;
        public bool enableVoids;

        [Tooltip("Fades in/out fog effect when reference object enters the fog volume.")]
        public bool enableFade;
        public float fadeDistance = 1;
        [Tooltip("The controller (player or camera) to check if enters the fog volume.")]
        public Transform fadeController;
        [Tooltip("Enable sub-volume blending.")]
        public bool enableSubVolumes;
        [Tooltip("Allowed subVolumes. If no subvolume is specified, any subvolume entered by this controller will affect this fog volume.")]
        public List<VolumetricFogSubVolume> subVolumes;
        [Tooltip("Shows the fog volume boundary in Game View")]
        public bool showBoundary;


        const string SKW_SHAPE_BOX = "V2F_SHAPE_BOX";
        const string SKW_SHAPE_SPHERE = "V2F_SHAPE_SPHERE";
        const string SKW_POINT_LIGHTS = "VF2_POINT_LIGHTS";
        const string SKW_VOIDS = "VF2_VOIDS";
        const string SKW_FOW = "VF2_FOW";
        const string SKW_RECEIVE_SHADOWS = "VF2_RECEIVE_SHADOWS";
        const string SKW_DISTANCE = "VF2_DISTANCE";
        const string SKW_DETAIL_NOISE = "V2F_DETAIL_NOISE";
        const string SKW_SURFACE = "V2F_SURFACE";

        Renderer r;
        Material fogMat, noiseMat, turbulenceMat;
        Material fogMat2D, noiseMat2D, turbulenceMat2D;
        RenderTexture rtNoise, rtTurbulence;
        float turbAcum;
        float windAcum;
        Vector3 sunDir;
        float dayLight;
        List<string> shaderKeywords;
        Texture3D detailTex, refDetailTex;
        Mesh debugMesh;
        Material fogDebugMat;
        VolumetricFogProfile activeProfile, lerpProfile;
        Vector3 lastControllerPosition;
        float alphaMultiplier = 1f;

        bool profileIsInstanced;
        bool requireUpdateMaterial;


        /// <summary>
        /// This property will return an instanced copy of the profile and use it for this volumetric fog from now on. Works similarly to Unity's material vs sharedMaterial.
        /// </summary>
        public VolumetricFogProfile settings {
            get {
                if (!profileIsInstanced && profile != null) {
                    profile = Instantiate(profile);
                    profileIsInstanced = true;
                }
                requireUpdateMaterial = true;
                return profile;
            }
            set {
                profile = value;
                profileIsInstanced = false;
            }
        }

        void OnEnable() {
            VolumetricFogManager manager = Tools.CheckMainManager();
            gameObject.layer = manager.fogLayer;
            FogOfWarInit();
            CheckSurfaceCapture();
            UpdateMaterialProperties();
        }

        private void OnDisable() {
            if (profile != null) {
                profile.onSettingsChanged -= UpdateMaterialProperties;
            }
            DisableSurfaceCapture();
        }

        private void OnValidate() {
            UpdateMaterialProperties();
        }

        private void OnDestroy() {
            if (rtNoise != null) {
                rtNoise.Release();
            }
            if (rtTurbulence != null) {
                rtTurbulence.Release();
            }
            if (fogMat != null) {
                DestroyImmediate(fogMat);
                fogMat = null;
            }
            FogOfWarDestroy();
            DisposeSurfaceCapture();
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = new Color(1, 1, 0, 0.75F);
            Gizmos.DrawWireCube(transform.position, transform.lossyScale);
        }

        void LateUpdate() {
            if (fogMat == null || r == null || profile == null) return;

            if (requireUpdateMaterial) {
                requireUpdateMaterial = false;
                UpdateMaterialProperties();
            }

            Bounds bounds = r.bounds;
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;

            ComputeActiveProfile();

            if (enableFade || enableSubVolumes) {
                ApplyProfileSettings();
            }

            if (activeProfile.shape == VolumetricFogShape.Sphere) {
                Vector3 scale = transform.localScale;
                if (scale.z != scale.x) {
                    scale.z = scale.x;
                    transform.localScale = scale;
                    extents = r.bounds.extents;
                }
                extents.x *= extents.x;
            }

            Vector4 border = new Vector4(extents.x * activeProfile.border + 0.0001f, extents.x * (1f - activeProfile.border), extents.z * activeProfile.border + 0.0001f, extents.z * (1f - activeProfile.border));
            if (activeProfile.terrainFit) {
                extents.y = Mathf.Max(extents.y, activeProfile.terrainFogHeight);
            }
            fogMat.SetVector(ShaderParams.BoundsCenter, center);
            fogMat.SetVector(ShaderParams.BoundsExtents, extents);
            fogMat.SetVector(ShaderParams.BoundsBorder, border);
            fogMat.SetFloat(ShaderParams.BoundsVerticalOffset, activeProfile.verticalOffset);

            VolumetricFogManager globalManager = VolumetricFogManager.instance;
            Light sun = globalManager.sun;
            Color lightColor;
            if (sun != null) {
                sunDir = -sun.transform.forward;
                fogMat.SetVector(ShaderParams.SunDir, sunDir);
                dayLight = 1f + sunDir.y * 2f;
                if (dayLight < 0) dayLight = 0; else if (dayLight > 1f) dayLight = 1f;
                float brightness;
                float alpha;
                if (profile != null) {
                    brightness = activeProfile.brightness;
                    alpha = activeProfile.albedo.a;
                } else {
                    brightness = 1f;
                    alpha = 1f;
                }
                lightColor = sun.color * (sun.intensity * brightness * dayLight * 2f);
                lightColor.a = alpha;
            } else {
                lightColor = Color.white;
            }
            lightColor.a *= alphaMultiplier;
            fogMat.SetVector(ShaderParams.LightColor, lightColor);

            windAcum += Time.deltaTime;
            windAcum %= 10000;
            fogMat.SetVector(ShaderParams.WindDirection, activeProfile.windDirection * windAcum);

            transform.rotation = Quaternion.identity;

            UpdateNoise();

            if (enableFogOfWar) {
                UpdateFogOfWar();
            }

            if (showBoundary) {
                if (fogDebugMat == null) {
                    fogDebugMat = new Material(Shader.Find("Hidden/VolumetricFog2/VolumetricFogDebug"));
                }
                if (debugMesh == null) {
                    MeshFilter mf = GetComponent<MeshFilter>();
                    if (mf != null) {
                        debugMesh = mf.sharedMesh;
                    }
                }
                Matrix4x4 m = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Graphics.DrawMesh(debugMesh, m, fogDebugMat, 0);
            }

            SurfaceCaptureUpdate();
        }


        void UpdateNoise() {

            if (activeProfile == null) return;
            Texture noiseTex = activeProfile.noiseTexture;
            if (noiseTex == null) return;

            if (rtTurbulence == null || rtTurbulence.width != noiseTex.width) {
                RenderTextureDescriptor desc = new RenderTextureDescriptor(noiseTex.width, noiseTex.height, RenderTextureFormat.ARGB32, 0);
                rtTurbulence = new RenderTexture(desc);
                rtTurbulence.wrapMode = TextureWrapMode.Repeat;
            }
            turbAcum += Time.deltaTime * activeProfile.turbulence;
            turbAcum %= 10000;
            turbulenceMat.SetFloat(ShaderParams.TurbulenceAmount, turbAcum);
            turbulenceMat.SetFloat(ShaderParams.NoiseStrength, activeProfile.noiseStrength);
            turbulenceMat.SetFloat(ShaderParams.NoiseFinalMultiplier, activeProfile.noiseFinalMultiplier);
            Graphics.Blit(noiseTex, rtTurbulence, turbulenceMat);

            if (rtNoise == null || rtNoise.width != noiseTex.width) {
                RenderTextureDescriptor desc = new RenderTextureDescriptor(noiseTex.width, noiseTex.height, RenderTextureFormat.ARGB32, 0);
                rtNoise = new RenderTexture(desc);
                rtNoise.wrapMode = TextureWrapMode.Repeat;
            }
            noiseMat.SetColor(ShaderParams.SpecularColor, activeProfile.specularColor);
            noiseMat.SetFloat(ShaderParams.SpecularIntensity, activeProfile.specularIntensity);

            float spec = 1.0001f - activeProfile.specularThreshold;
            float nlighty = sunDir.y > 0 ? (1.0f - sunDir.y) : (1.0f + sunDir.y);
            float nyspec = nlighty / spec;

            noiseMat.SetFloat(ShaderParams.SpecularThreshold, nyspec);
            noiseMat.SetVector(ShaderParams.SunDir, sunDir);

            Color ambientColor = RenderSettings.ambientLight;
            float ambientIntensity = RenderSettings.ambientIntensity;
            Color ambientMultiplied = ambientColor * ambientIntensity;
            float fogIntensity = 1.15f;
            fogIntensity *= dayLight;
            Color textureBaseColor = Color.Lerp(ambientMultiplied, activeProfile.albedo * fogIntensity, fogIntensity);

            noiseMat.SetColor(ShaderParams.Color, textureBaseColor);
            Graphics.Blit(rtTurbulence, rtNoise, noiseMat);

            fogMat.SetTexture(ShaderParams.MainTex, rtNoise);

            Color detailColor = new Color(textureBaseColor.r * 0.5f, textureBaseColor.g * 0.5f, textureBaseColor.b * 0.5f, 0);
            fogMat.SetColor(ShaderParams.DetailColor, detailColor);
        }

        public void UpdateMaterialProperties() {

            if (!gameObject.activeInHierarchy) {
                DisableSurfaceCapture();
                return;
            }

            fadeDistance = Mathf.Max(0.1f, fadeDistance);

            r = GetComponent<Renderer>();

            if (profile == null) {
                if (fogMat == null && r != null) {
                    fogMat = new Material(Shader.Find("VolumetricFog2/Empty"));
                    fogMat.hideFlags = HideFlags.DontSave;
                    r.sharedMaterial = fogMat;
                }
                DisableSurfaceCapture();
                return;
            }
            profile.onSettingsChanged -= UpdateMaterialProperties;
            profile.onSettingsChanged += UpdateMaterialProperties;

            if (fogMat2D == null) {
                fogMat2D = new Material(Shader.Find("VolumetricFog2/VolumetricFog2DURP"));
                fogMat2D.hideFlags = HideFlags.DontSave;
            }
            fogMat = fogMat2D;
            if (turbulenceMat2D == null) {
                turbulenceMat2D = new Material(Shader.Find("VolumetricFog2/Turbulence2D"));
            }
            turbulenceMat = turbulenceMat2D;
            if (noiseMat2D == null) {
                noiseMat2D = new Material(Shader.Find("VolumetricFog2/Noise2DGen"));
            }
            noiseMat = noiseMat2D;

            if (r != null) {
                r.sharedMaterial = fogMat;
            }

            if (fogMat == null || profile == null) return;

            profile.ValidateSettings();

            lastControllerPosition.x = float.MaxValue;
            activeProfile = profile;

            ComputeActiveProfile();
            ApplyProfileSettings();
            SurfaceCaptureSupportCheck();
        }

        void ComputeActiveProfile() {

            if (maskEditorEnabled) alphaMultiplier = 0.85f;
            if (Application.isPlaying) {
                if (enableFade || enableSubVolumes) {
                    if (fadeController == null) {
                        Camera cam = Camera.main;
                        if (cam != null) {
                            fadeController = Camera.main.transform;
                        }
                    }
                    if (fadeController != null && lastControllerPosition != fadeController.position) {

                        lastControllerPosition = fadeController.position;
                        activeProfile = profile;
                        alphaMultiplier = 1f;

                        // Self volume
                        if (enableFade) {
                            float t = ComputeVolumeFade(transform, fadeDistance);
                            alphaMultiplier *= t;
                        }

                        // Check sub-volumes
                        if (enableSubVolumes) {
                            int subVolumeCount = VolumetricFogSubVolume.subVolumes.Count;
                            int allowedSubVolumesCount = subVolumes != null ? subVolumes.Count : 0;
                            for (int k = 0; k < subVolumeCount; k++) {
                                VolumetricFogSubVolume subVolume = VolumetricFogSubVolume.subVolumes[k];
                                if (subVolume == null || subVolume.profile == null) continue;
                                if (allowedSubVolumesCount > 0 && !subVolumes.Contains(subVolume)) continue;
                                float t = ComputeVolumeFade(subVolume.transform, subVolume.fadeDistance);
                                if (t > 0) {
                                    if (lerpProfile == null) {
                                        lerpProfile = ScriptableObject.CreateInstance<VolumetricFogProfile>();
                                    }
                                    lerpProfile.Lerp(activeProfile, subVolume.profile, t);
                                    activeProfile = lerpProfile;
                                }
                            }
                        }
                    }
                }
            }

            if (activeProfile == null) {
                activeProfile = profile;
            }
        }

        float ComputeVolumeFade(Transform transform, float fadeDistance) {
            Vector3 diff = transform.position - fadeController.position;
            diff.x = diff.x < 0 ? -diff.x : diff.x;
            diff.y = diff.y < 0 ? -diff.y : diff.y;
            diff.z = diff.z < 0 ? -diff.z : diff.z;
            Vector3 extents = transform.lossyScale * 0.5f;
            Vector3 gap = extents - diff;
            float minDiff = gap.x < gap.y ? gap.x : gap.y;
            minDiff = minDiff < gap.z ? minDiff : gap.z;
            fadeDistance += 0.0001f;
            float t = Mathf.Clamp01(minDiff / fadeDistance);
            return t;
        }


        void ApplyProfileSettings() {
            r.sortingLayerID = activeProfile.sortingLayerID;
            r.sortingOrder = activeProfile.sortingOrder;
            fogMat.renderQueue = activeProfile.renderQueue;
            float noiseScale = 0.1f / activeProfile.noiseScale;
            fogMat.SetFloat(ShaderParams.NoiseScale, noiseScale);
            fogMat.SetFloat(ShaderParams.DeepObscurance, activeProfile.deepObscurance);
            fogMat.SetFloat(ShaderParams.LightDiffusionPower, activeProfile.lightDiffusionPower);
            fogMat.SetFloat(ShaderParams.LightDiffusionIntensity, activeProfile.lightDiffusionIntensity);
            fogMat.SetFloat(ShaderParams.ShadowIntensity, activeProfile.shadowIntensity);
            fogMat.SetFloat(ShaderParams.Density, activeProfile.density);
            fogMat.SetVector(ShaderParams.RaymarchSettings, new Vector4(activeProfile.raymarchQuality, activeProfile.dithering * 0.01f, activeProfile.jittering, activeProfile.raymarchMinStep));

            if (activeProfile.useDetailNoise) {
                float detailScale = (1f / activeProfile.detailScale) * noiseScale;
                fogMat.SetVector(ShaderParams.DetailData, new Vector4(activeProfile.detailStrength, activeProfile.detailOffset, detailScale, activeProfile.noiseFinalMultiplier));
                fogMat.SetColor(ShaderParams.DetailColor, activeProfile.albedo);
                fogMat.SetFloat(ShaderParams.DetailOffset, activeProfile.detailOffset);
                if ((detailTex == null || refDetailTex != activeProfile.detailTexture) && activeProfile.detailTexture != null) {
                    refDetailTex = activeProfile.detailTexture;
                    Texture3D tex = new Texture3D(activeProfile.detailTexture.width, activeProfile.detailTexture.height, activeProfile.detailTexture.depth, TextureFormat.Alpha8, false);
                    tex.filterMode = FilterMode.Bilinear;
                    Color32[] colors = activeProfile.detailTexture.GetPixels32();
                    for (int k = 0; k < colors.Length; k++) { colors[k].a = colors[k].r; }
                    tex.SetPixels32(colors);
                    tex.Apply();
                    detailTex = tex;
                }
                fogMat.SetTexture(ShaderParams.DetailTexture, detailTex);
            }

            if (shaderKeywords == null) {
                shaderKeywords = new List<string>();
            } else {
                shaderKeywords.Clear();
            }

            if (activeProfile.distance > 0) {
                fogMat.SetVector(ShaderParams.DistanceData, new Vector4(0, 10f * (1f - activeProfile.distanceFallOff), 0, 1f / (0.0001f + activeProfile.distance * activeProfile.distance)));
                shaderKeywords.Add(SKW_DISTANCE);
            }
            if (activeProfile.shape == VolumetricFogShape.Box) shaderKeywords.Add(SKW_SHAPE_BOX); else shaderKeywords.Add(SKW_SHAPE_SPHERE);
            if (enablePointLights) shaderKeywords.Add(SKW_POINT_LIGHTS);
            if (enableVoids) shaderKeywords.Add(SKW_VOIDS);
            if (activeProfile.receiveShadows) shaderKeywords.Add(SKW_RECEIVE_SHADOWS);
            if (enableFogOfWar) {
                fogMat.SetTexture(ShaderParams.FogOfWarTexture, fogOfWarTexture);
                fogMat.SetVector(ShaderParams.FogOfWarCenter, fogOfWarCenter);
                fogMat.SetVector(ShaderParams.FogOfWarSize, fogOfWarSize);
                Vector3 ca = fogOfWarCenter - 0.5f * fogOfWarSize;
                fogMat.SetVector(ShaderParams.FogOfWarCenterAdjusted, new Vector3(ca.x / fogOfWarSize.x, 1f, ca.z / (fogOfWarSize.z + 0.0001f)));
                shaderKeywords.Add(SKW_FOW);
            }
            if (activeProfile.useDetailNoise) shaderKeywords.Add(SKW_DETAIL_NOISE);
            if (activeProfile.terrainFit) shaderKeywords.Add(SKW_SURFACE);
            fogMat.shaderKeywords = shaderKeywords.ToArray();
        }

        /// <summary>
        /// Issues a refresh of the depth pre-pass alpha clipping renderers list
        /// </summary>
        public static void FindAlphaClippingObjects() {
            DepthRenderPrePassFeature.DepthRenderPass.FindAlphaClippingRenderers();
        }
    }

}

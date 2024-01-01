using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumetricFogAndMist2 {

    public delegate void OnSettingsChanged();

    [CreateAssetMenu(menuName = "Volumetric Fog \x8B& Mist/Fog Profile", fileName = "VolumetricFogProfile", order = 1001)]
    public class VolumetricFogProfile : ScriptableObject {

        [Header("Rendering")]
        [Range(1, 16)] public int raymarchQuality = 6;
        [Tooltip("Determines the minimum step size. Increase to improve performance / decrease to improve accuracy. When increasing this value, you can also increase 'Jittering' amount to improve quality.")]
        public float raymarchMinStep = 0.1f;
        public float jittering = 0.5f;
        [Range(0, 2)] public float dithering = 1f;
        [Tooltip("The render queue for this renderer. By default, all transparent objects use a render queue of 3000. Use a lower value to render before all transparent objects.")]
        public int renderQueue = 3100;
        [Tooltip("Optional sorting layer Id (number) for this renderer. By default 0. Usually used to control the order with other transparent renderers, like Sprite Renderer.")]
        public int sortingLayerID;
        [Tooltip("Optional sorting order for this renderer. Used to control the order with other transparent renderers, like Sprite Renderer.")]
        public int sortingOrder;

        [Header("Density")]
        public Texture2D noiseTexture;
        [Range(0, 3)] public float noiseStrength = 1f;
        public float noiseScale = 15f;
        public float noiseFinalMultiplier = 1f;

        public bool useDetailNoise;
        public Texture3D detailTexture;
        public float detailScale = 0.35f;
        [Range(0, 1f)] public float detailStrength = 0.5f;
        public float detailOffset = -0.5f;

        public float density = 1f;

        [Header("Geometry")]
        public VolumetricFogShape shape = VolumetricFogShape.Box;
        [Range(0, 1f)] public float border = 0.05f;
        public float verticalOffset;
        [Tooltip("When enabled, makes fog appear at certain distance from a camera")]
        public float distance;
        [Range(0, 1)] public float distanceFallOff;

        [Tooltip("Fits the fog altitude to the terrain heightmap")]
        public bool terrainFit;
        public VolumetricFog.HeightmapCaptureResolution terrainFitResolution = VolumetricFog.HeightmapCaptureResolution._128;
        [Tooltip("Which objects will be included in the heightmap capture. By default all objects are included but you may want to restrict this to just the terrain.")]
        public LayerMask terrainLayerMask = -1;
        [Tooltip("The height of fog above terrain surface.")]
        public float terrainFogHeight = 25f;
        public float terrainFogMinAltitude = 0f;
        public float terrainFogMaxAltitude = 150f;


        [Header("Colors")]
        public Color albedo = new Color32(227, 227, 227, 255);
        public float brightness = 1f;
        [Range(0, 2)] public float deepObscurance = 1f;
        public Color specularColor = new Color(1, 1, 0.8f, 1);
        [Range(0, 1f)] public float specularThreshold = 0.637f;
        [Range(0, 1f)] public float specularIntensity = 0.428f;

        [Header("Animation")]
        public float turbulence = 0.73f;
        public Vector3 windDirection = new Vector3(0.02f, 0, 0);

        [Header("Directional Light")]
        [Range(0, 64)] public float lightDiffusionPower = 32;
        [Range(0, 1)] public float lightDiffusionIntensity = 0.4f;
        public bool receiveShadows;
        [Range(0, 1)] public float shadowIntensity = 0.5f;

        public event OnSettingsChanged onSettingsChanged;

        private void OnEnable() {
            if (noiseTexture == null) {
                noiseTexture = Resources.Load<Texture2D>("Textures/NoiseTex256");
            }
            if (detailTexture == null) {
                detailTexture = Resources.Load<Texture3D>("Textures/NoiseTex3D");
            }
            ValidateSettings();
        }

#if UNITY_EDITOR
        private void OnValidate() {
            ValidateSettings();
            if (onSettingsChanged != null) {
                UnityEditor.EditorApplication.delayCall += () => onSettingsChanged();
            }
        }
#endif

        public void ValidateSettings() {
            distance = Mathf.Max(0, distance);
            density = Mathf.Max(0, density);
            noiseScale = Mathf.Max(0.1f, noiseScale);
            noiseFinalMultiplier = Mathf.Max(0, noiseFinalMultiplier);
            detailScale = Mathf.Max(0.01f, detailScale);
            raymarchMinStep = Mathf.Max(0.1f, raymarchMinStep);
            jittering = Mathf.Max(0, jittering);
            terrainFogHeight = Mathf.Max(0, terrainFogHeight);
        }

        public void Lerp(VolumetricFogProfile p1, VolumetricFogProfile p2, float t) {
            float t0 = 1f - t;

            raymarchQuality = (int)(p1.raymarchQuality * t0 + p2.raymarchQuality * t);
            raymarchMinStep = p1.raymarchMinStep * t0 + p2.raymarchMinStep * t;
            jittering = p1.jittering * t0 + p2.jittering * t;
            dithering = p1.dithering * t0 + p2.dithering * t;
            renderQueue = t < 0.5f ? p1.renderQueue : p2.renderQueue;
            sortingLayerID = t < 0.5f ? p1.sortingLayerID : p2.sortingLayerID;
            sortingOrder = t < 0.5f ? p1.sortingOrder : p2.sortingOrder;
            noiseStrength = p1.noiseStrength * t0 + p2.noiseStrength * t;
            noiseScale = p1.noiseScale * t0 + p2.noiseScale * t;
            noiseFinalMultiplier = p1.noiseFinalMultiplier * t0 + p2.noiseFinalMultiplier * t;
            noiseTexture = t < 0.5f ? p1.noiseTexture : p2.noiseTexture;
            useDetailNoise = t < 0.5f ? p1.useDetailNoise : p2.useDetailNoise;
            detailTexture = t < 0.5f ? p1.detailTexture : p2.detailTexture;
            detailScale = p1.detailScale * t0 + p2.detailScale * t;
            detailStrength = p1.detailStrength * t0 + p2.detailStrength * t;
            detailOffset = p1.detailOffset * t0 + p2.detailOffset * t;
            density = p1.density * t0 + p2.density * t;
            shape = t < 0.5f ? p1.shape : p2.shape;
            border = p1.border * t0 + p2.border * t;
            verticalOffset = p1.verticalOffset * t0 + p2.verticalOffset * t;
            distance = p1.distance * t0 + p2.distance * t;
            distanceFallOff = p1.distanceFallOff * t0 + p2.distanceFallOff * t;
            albedo = p1.albedo * t0 + p2.albedo * t;
            brightness = p1.brightness * t0 + p2.brightness * t;
            deepObscurance = p1.deepObscurance * t0 + p2.deepObscurance * t;
            specularColor = p1.specularColor * t0 + p2.specularColor * t;
            specularThreshold = p1.specularThreshold * t0 + p2.specularThreshold * t;
            specularIntensity = p1.specularIntensity * t0 + p2.specularIntensity * t;
            turbulence = p1.turbulence * t0 + p2.turbulence * t;
            windDirection = p1.windDirection * t0 + p2.windDirection * t;
            lightDiffusionPower = p1.lightDiffusionPower * t0 + p2.lightDiffusionPower * t;
            lightDiffusionIntensity = p1.lightDiffusionIntensity * t0 + p2.lightDiffusionIntensity * t;
            receiveShadows = t < 0.5f ? p1.receiveShadows : p2.receiveShadows;
            shadowIntensity = p1.shadowIntensity * t0 + p2.shadowIntensity * t;
            terrainFit = t < 0.5f ? p1.terrainFit : p2.terrainFit;
            terrainFitResolution = t < 0.5 ? p1.terrainFitResolution : p2.terrainFitResolution;
            terrainFogHeight = p1.terrainFogHeight * t0 + p2.terrainFogHeight * t;
            terrainFogMinAltitude = p1.terrainFogMinAltitude * t0 + p2.terrainFogMinAltitude * t;
            terrainFogMaxAltitude = p1.terrainFogMaxAltitude * t0 + p2.terrainFogMaxAltitude * t;
            terrainLayerMask = t < 0.5f ? p1.terrainLayerMask : p2.terrainLayerMask;
        }

    }
}
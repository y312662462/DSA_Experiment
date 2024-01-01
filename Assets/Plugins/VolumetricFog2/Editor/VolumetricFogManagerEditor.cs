using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

namespace VolumetricFogAndMist2 {

    [CustomEditor(typeof(VolumetricFogManager))]
    public class VolumetricFogManagerEditor : Editor {

        SerializedProperty mainCamera, sun, fogLayer, includeTransparent, includeSemiTransparent, alphaCutOff, flipDepthTexture;

        private void OnEnable() {
            mainCamera = serializedObject.FindProperty("mainCamera");
            sun = serializedObject.FindProperty("sun");
            fogLayer = serializedObject.FindProperty("fogLayer");
            includeTransparent = serializedObject.FindProperty("includeTransparent");
            includeSemiTransparent = serializedObject.FindProperty("includeSemiTransparent");
            alphaCutOff = serializedObject.FindProperty("alphaCutOff");
            flipDepthTexture = serializedObject.FindProperty("flipDepthTexture");
        }


        public override void OnInspectorGUI() {

            EditorGUILayout.Separator();

            UniversalRenderPipelineAsset pipe = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
            if (pipe == null) {
                EditorGUILayout.HelpBox("Please assign the Universal Rendering Pipeline asset (go to Project Settings -> Graphics). You can use the UniversalRenderPipelineAsset included in the demo folder or create a new pipeline asset (check documentation for step by step setup).", MessageType.Error);
                return;
            }

            if (QualitySettings.renderPipeline != null) {
                pipe = QualitySettings.renderPipeline as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
            }

            if (!pipe.supportsCameraDepthTexture) {
                EditorGUILayout.HelpBox("Depth Texture option is required in Universal Rendering Pipeline asset!", MessageType.Error);
                if (GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                    Selection.activeObject = pipe;
                }
                EditorGUILayout.Separator();
                GUI.enabled = false;
            }

            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(mainCamera);
            EditorGUILayout.PropertyField(sun);
            fogLayer.intValue = EditorGUILayout.LayerField("Fog Layer", fogLayer.intValue);
            EditorGUILayout.PropertyField(flipDepthTexture);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(new GUIContent("Special Transparent Objects", "Support for transparent or semi-transparent objects"), EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(includeTransparent, new GUIContent("Transparent Objects", "Specify which layers contain transparent objects that should be covered by fog"));
            EditorGUILayout.PropertyField(includeSemiTransparent, new GUIContent("Alpha Clipping Objects", "Specify which smi-transparent objects (cutout materials) should be covered by fog."));
            if (includeSemiTransparent.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(alphaCutOff, new GUIContent("Alpha CutOff"), true);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent(""), GUIContent.none);
                if (GUILayout.Button("Refresh")) {
                    DepthRenderPrePassFeature.DepthRenderPass.FindAlphaClippingRenderers();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }

            if (includeTransparent.intValue != 0 || includeSemiTransparent.intValue != 0) {
                if (!DepthRenderPrePassFeature.installed) {
                    EditorGUILayout.HelpBox("Transparent option requires 'DepthRendererPrePass Feature' added to the Forward Renderer of the Universal Rendering Pipeline asset. Check the documentation for instructions.", MessageType.Warning);
                    if (pipe != null && GUILayout.Button("Show Pipeline Asset")) Selection.activeObject = pipe;
                }
            } else if (DepthRenderPrePassFeature.installed) {
                EditorGUILayout.HelpBox("No transparent objects included. Remove 'DepthRendererPrePass Feature' from the Forward Renderer of the Universal Rendering Pipeline asset to save performance.", MessageType.Warning);
                if (pipe != null && GUILayout.Button("Show Pipeline Asset")) Selection.activeObject = pipe;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Managers", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Point Light Manager", GUILayout.Width(EditorGUIUtility.labelWidth));
            if (GUILayout.Button("Open", GUILayout.Width(150))) {
                Selection.activeGameObject = VolumetricFogManager.pointLightManager.gameObject;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Fog Void Manager", GUILayout.Width(EditorGUIUtility.labelWidth));
            if (GUILayout.Button("Open", GUILayout.Width(150))) {
                Selection.activeGameObject = VolumetricFogManager.fogVoidManager.gameObject;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Noise Generator", GUILayout.Width(EditorGUIUtility.labelWidth));
            if (GUILayout.Button("Open", GUILayout.Width(150))) {
                NoiseGenerator window = EditorWindow.GetWindow<NoiseGenerator>("Noise Generator", true);
                window.minSize = new Vector2(400, 400);
                window.Show();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

        }
    }

}
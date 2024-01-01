using UnityEngine;

namespace VolumetricFogAndMist2 {

    [ExecuteInEditMode]
    public class FogVoid : MonoBehaviour {

        [Range(0, 1)] public float roundness = 0.5f;
        [Range(0, 1)] public float falloff = 0.5f;

        private void OnEnable() {
            VolumetricFogManager.fogVoidManager.Refresh();
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = new Color(1, 1, 0, 0.75F);
            Gizmos.DrawWireCube(transform.position, transform.lossyScale);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace VolumetricFogAndMist2 {

    public class VolumetricFogSubVolume : MonoBehaviour {

        public VolumetricFogProfile profile;
        public float fadeDistance = 1f;

        public static List<VolumetricFogSubVolume> subVolumes = new List<VolumetricFogSubVolume>();

        void OnDrawGizmos() {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }

        void OnEnable() {
            if (!subVolumes.Contains(this)) {
                subVolumes.Add(this);
            }
        }

        void OnDisable() {
            if (subVolumes.Contains(this)) {
                subVolumes.Remove(this);
            }
        }

    }

}
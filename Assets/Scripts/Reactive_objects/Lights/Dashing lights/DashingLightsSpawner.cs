using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;


public class DashingLightsSpawner : MonoBehaviour
{
    [EventID]
    public string eventID;
    public GameObject[] dashingCubes;
    public Transform[] dashingCubeSpawnTransform;

    private MusicPlayer musicPlayer;


    void Awake()
    {
        musicPlayer = FindObjectOfType<MusicPlayer>();

        Koreographer.Instance.RegisterForEvents(eventID, SpawnDashingCubes);


    }

    public void SpawnDashingCubes(KoreographyEvent evt)
    {
        int dashingCubeSpawnNum = Random.Range(0, dashingCubes.Length);
        int spawnTransformNum = Random.Range(0, dashingCubeSpawnTransform.Length);

        GameObject spawnedDashingCube = Instantiate(dashingCubes[dashingCubeSpawnNum], new Vector3(dashingCubeSpawnTransform[spawnTransformNum].position.x, dashingCubeSpawnTransform[spawnTransformNum].position.y, dashingCubeSpawnTransform[spawnTransformNum].position.z), Quaternion.identity, this.transform);



    }
}

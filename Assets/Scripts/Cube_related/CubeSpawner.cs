using System.Collections;
using UnityEngine;
using SonicBloom.Koreo;

public class CubeSpawner : MonoBehaviour
{
    public GameObject[] Object_Prefabs;
    public Transform[] spawn_Transform;
    public Transform[] spawn_Transform_Right;
    public Transform[] spawn_Transform_Left;

    public float[] spawn_Rotation;


    [EventID]
    public string eventID;
    public ScoreManager scoreManager;
    private bool spawnRight = false;
    private bool spawnLeft = false;


    


    void Awake()
    {
        Koreographer.Instance.RegisterForEvents(eventID, spawnObjects);
    }



    public void spawnObjects(KoreographyEvent evt)
    {
        
        int objectDetailsNum = evt.GetIntValue();
        Debug.Log("objectDetailsNum");
        //int spawn_Object;
        int spawn_Location;
        int random_Rotation_Select = Random.Range(0, spawn_Rotation.Length);

        //Spawn Cubes Randomly

        if (objectDetailsNum >= 0 && objectDetailsNum < 100)
        {
            /*random_Rotation_Select = Random.Range(0, spawn_Rotation.Length);

            spawn_Object = objectDetailsNum / 10;


            spawn_Location = objectDetailsNum % 10;


            GameObject spawnedCube = Instantiate(Object_Prefabs[spawn_Object], spawn_Transform[spawn_Location].position, Quaternion.identity, this.transform);


            //spawnedCube.transform.Find("Cube").gameObject.transform.rotation = Quaternion.Euler(0f, 0f, spawn_Rotation[random_Rotation_Select]);


            spawnedCube.transform.Find("Cube").transform.Find("SpawnLight").gameObject.transform.rotation = Quaternion.Euler(-90, 0, 0);*/
            Debug.Log("error spawn");
        }

        else if (objectDetailsNum >= 100 && objectDetailsNum < 200)
        {
            random_Rotation_Select = Random.Range(0, spawn_Rotation.Length);

            spawn_Location = objectDetailsNum % 10;

            //int random_Object_Select = 0;
            //0:blue 1:red 2:gray 3:yellow 4:green
            /*if (random_Object_Select == 0)
            {
                switch (receive_eeg.Instance.AttentionLevel)
                {
                    case 1:
                    random_Object_Select = 3;
                    break;
                    case 2:
                    random_Object_Select = 4;
                    break;
                    case 3:
                    random_Object_Select = 0;
                    break;
                    case 4:
                    random_Object_Select = 0;
                    break;
                    default:
                    break;
                } 
            }*/


            GameObject spawnedCube = Instantiate(Object_Prefabs[0], spawn_Transform[spawn_Location].position, Quaternion.identity, this.transform);

            //spawnedCube.transform.Find("Cube").gameObject.transform.rotation = Quaternion.Euler(0f, 0f, spawn_Rotation[random_Rotation_Select]);

            spawnedCube.transform.Find("Cube").transform.Find("SpawnLight").gameObject.transform.rotation = Quaternion.Euler(-90, 0, 0);


            random_Rotation_Select = Random.Range(0, spawn_Rotation.Length);

            spawn_Location = (objectDetailsNum / 10) % 10;



            GameObject spawnedCube_1 = Instantiate(Object_Prefabs[1], spawn_Transform[spawn_Location].position, Quaternion.identity, this.transform);

            //spawnedCube_1.transform.Find("Cube").gameObject.transform.rotation = Quaternion.Euler(0f, 0f, spawn_Rotation[random_Rotation_Select]);

            spawnedCube_1.transform.Find("Cube").transform.Find("SpawnLight").gameObject.transform.rotation = Quaternion.Euler(-90, 0, 0);
        }

        scoreManager.totalSpawns += 2;
        //StartCoroutine(totalAdd());

    }

    /*IEnumerator totalAdd()
    {
        scoreManager.totalSpawns++;
        yield return 1;
    }*/
}







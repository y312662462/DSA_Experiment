using System.Collections;
using UnityEngine;
using SonicBloom.Koreo;

public class InstantiateObjects : MonoBehaviour
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
    //private int pulishRate = 20;





    void Awake()
    {
        Koreographer.Instance.RegisterForEvents(eventID, spawnObjects);

    }



    public void spawnObjects(KoreographyEvent evt)
    {
        //Spawn Cubes Randomly
        //Debug.Log("Spawn");
        int random_Object_Select = Random.Range(0, 2);

        int random_Location_Select = Random.Range(0, spawn_Transform.Length);

        int random_Right_Location_Select = Random.Range(0, spawn_Transform_Right.Length);

        int random_Left_Location_Select = Random.Range(0, spawn_Transform_Left.Length);

        int random_Rotation_Select = Random.Range(0, spawn_Rotation.Length);

        int random_Pulish_Location = 0;

        //int spawnColorProbability = Random.Range(0, 100);

        //int rate = Random.Range(1, 101);

        int isPulish = evt.GetIntValue();

        //if (spawnColorProbability > 5)
        //{
        if (Object_Prefabs[random_Object_Select].transform.CompareTag("BlueCube"))    
        {    
            spawnRight = true;    
            spawnLeft = false;
            random_Pulish_Location = Random.Range(0, 2);
        }    
        else if (Object_Prefabs[random_Object_Select].transform.CompareTag("RedCube"))    
        {    
            spawnLeft = true;    
            spawnRight = false;
            random_Pulish_Location = Random.Range(4, 6);
        }    
        //}
        /*else
        {
            spawnRight = true;
            spawnLeft = true;
        }*/

        //0:blue 1:red 2:gray 3:yellow 4:green
        /*if (random_Object_Select == 0)
        {
            switch (receive_eeg.Instance.AttentionLevel)
            {
                case 1:
                random_Object_Select = 2;
                break;
                case 2:
                random_Object_Select = 2;
                break;
                case 3:
                random_Object_Select = 3;
                break;
                case 4:
                random_Object_Select = 4;
                break;
                default:
                break;
            }
        }*/

        if (spawnRight && !spawnLeft && isPulish != 1)
        {
            GameObject spawnedCube = Instantiate(Object_Prefabs[random_Object_Select], spawn_Transform_Right[random_Right_Location_Select].position, Quaternion.identity, this.transform);


            spawnedCube.transform.Find("Cube").gameObject.transform.rotation = Quaternion.Euler(0f, 0f, spawn_Rotation[random_Rotation_Select]);


            spawnedCube.transform.Find("Cube").transform.Find("SpawnLight").gameObject.transform.rotation = Quaternion.Euler(-90, 0, 0);

            scoreManager.totalSpawns++;

        }

        else if (!spawnRight && spawnLeft && isPulish != 1)
        {
            GameObject spawnedCube = Instantiate(Object_Prefabs[random_Object_Select], spawn_Transform_Left[random_Left_Location_Select].position, Quaternion.identity, this.transform);


            spawnedCube.transform.Find("Cube").gameObject.transform.rotation = Quaternion.Euler(0f, 0f, spawn_Rotation[random_Rotation_Select]);


            spawnedCube.transform.Find("Cube").transform.Find("SpawnLight").gameObject.transform.rotation = Quaternion.Euler(-90, 0, 0);

            scoreManager.totalSpawns++;

        }

        /*else if (spawnLeft && spawnRight && rate > pulishRate)
        {
            GameObject spawnedCube = Instantiate(Object_Prefabs[random_Object_Select], spawn_Transform[random_Location_Select].position, Quaternion.identity, this.transform);


            spawnedCube.transform.Find("Cube").gameObject.transform.rotation = Quaternion.Euler(0f, 0f, spawn_Rotation[random_Rotation_Select]);


            spawnedCube.transform.Find("Cube").transform.Find("SpawnLight").gameObject.transform.rotation = Quaternion.Euler(-90, 0, 0);
        }*/

        else if (isPulish == 1)
        {
            GameObject spawnedCube = Instantiate(Object_Prefabs[2], spawn_Transform[random_Pulish_Location].position, Quaternion.identity, this.transform);


            spawnedCube.transform.Find("Cube").gameObject.transform.rotation = Quaternion.Euler(0f, 0f, spawn_Rotation[random_Rotation_Select]);


            spawnedCube.transform.Find("Cube").transform.Find("SpawnLight").gameObject.transform.rotation = Quaternion.Euler(-90, 0, 0);

            scoreManager.pulishSpawns++;

            //Debug.Log("pulish");
            
        }


        
        //Store total number of spawned cubes
        
        spawnRight = false;
        spawnLeft = false;
    }
    
}

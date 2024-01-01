using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraduallyTurnOnLights : MonoBehaviour
{
    [SerializeField] private GameObject[] lightObjects;
    void Start()
    {
        foreach (GameObject gameObject in lightObjects)
        {
            gameObject.SetActive(false);
        }
        StartCoroutine(TurnOnLightObjects());

    }

    private IEnumerator TurnOnLightObjects()
    {
        int i = 0;
        while (i < lightObjects.Length)
        {
            yield return new WaitForSeconds(Random.Range(1,5));
            lightObjects[i].SetActive(true);
            i++;
        }
    }
}

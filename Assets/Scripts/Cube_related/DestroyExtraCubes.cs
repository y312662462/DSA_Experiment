using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyExtraCubes : MonoBehaviour
{

    public int missedCubes = 0;

    private Cross cross;

    private ScoreManager scoreManager;

    void Start()
    {
        cross = FindObjectOfType<Cross>();
        scoreManager = FindObjectOfType<ScoreManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RedCube") || other.gameObject.CompareTag("BlueCube"))
        {
            scoreManager.MissedCube();

            StartCoroutine(cross.ShowCross());
            missedCubes++;
            //Debug.Log("miss");
        }
        Destroy(other.transform.parent.gameObject, 2.5f);
    }
}

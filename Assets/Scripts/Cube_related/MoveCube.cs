using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCube : MonoBehaviour
{
    public Vector3 moveDirection;

    public float moveSpeed;

    [Range(1, 10)] public float moveUpBeforeTime;
    private Vector3 upOffset;
    private MusicPlayer musicPlayer;


    private int chance_will_go_up;

    private bool canGoUp = false;


    void Start()
    {
        musicPlayer = FindObjectOfType<MusicPlayer>();
        upOffset = new Vector3(0, 2, 0);
        int going_up_Probability = Random.Range(0, 100);
        if (going_up_Probability < 50)
        {
            StartCoroutine(MoveCubeUpwards());
        }

    }
    void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        if (canGoUp)
            transform.position = Vector3.MoveTowards(transform.position, (transform.position + upOffset), 0.01f);

    }

    IEnumerator MoveCubeUpwards()
    {
        yield return new WaitForSeconds(Random.Range(1, moveUpBeforeTime));
        canGoUp = true;
        yield return new WaitForSeconds(0.3f);
        canGoUp = false;

    }
}

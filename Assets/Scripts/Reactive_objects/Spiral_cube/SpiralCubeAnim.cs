using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class SpiralCubeAnim : MonoBehaviour
{
    [EventID]
    public string eventID;

    private Animator animator;

    public int minRotSpeed;
    public int maxRotSpeed;
    private float rotSpeed = 0;

    private bool rotToRight = false;
    void Start()
    {
        animator = GetComponent<Animator>();
        Koreographer.Instance.RegisterForEvents(eventID, SpiralCubeAnimation);
    }


    void Update()
    {
        gameObject.transform.Rotate(Vector3.forward * rotSpeed * Time.deltaTime);

    }
    public void SpiralCubeAnimation(KoreographyEvent evt)
    {
        rotToRight = !rotToRight;
        rotSpeed = Random.Range(minRotSpeed, maxRotSpeed);

        if (!rotToRight)
            rotSpeed = -rotSpeed;

    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashingLights : MonoBehaviour
{
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private float moveSpeed;
    // Update is called once per frame

    void Start()
    {
        Destroy(this.gameObject, 3f);
    }
    void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
}

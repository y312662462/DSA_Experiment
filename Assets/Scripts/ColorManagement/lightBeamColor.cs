using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VLB;

public class lightBeamColor : MonoBehaviour
{
    public lightColor iColor;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<VolumetricLightBeam>().color = iColor.litColor;
    }
}

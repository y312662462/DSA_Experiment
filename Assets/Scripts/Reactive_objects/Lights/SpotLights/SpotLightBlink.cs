using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class SpotLightBlink : MonoBehaviour
{
    [EventID]
    public string eventID;

    private Light spotLight;



    void Start()
    {
        spotLight = GetComponent<Light>();
        Koreographer.Instance.RegisterForEvents(eventID, BlinkingLights);
    }

    public void BlinkingLights(KoreographyEvent evt)
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
        //spotLight.range = Random.Range(10, 51);
    }
}

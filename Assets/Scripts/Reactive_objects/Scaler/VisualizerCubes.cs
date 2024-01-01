using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class VisualizerCubes : MonoBehaviour
{
    [EventID]
    public string eventID;

    [Range(0, 10)] public float scaleFactor;
    void Awake()
    {
        Koreographer.Instance.RegisterForEventsWithTime(eventID, VisualizeBeat);
    }

    private void VisualizeBeat(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (evt.HasCurvePayload())
        {
            float curveValue = evt.GetValueOfCurveAtTime(sampleTime);
            float scaleValue = curveValue * scaleFactor;

            //transform.localScale = Vector3.one * curveValue;
            transform.localScale = new Vector3(1, scaleValue, 1);
        }
    }
}

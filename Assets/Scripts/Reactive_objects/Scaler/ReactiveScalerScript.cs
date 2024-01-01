using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class ReactiveScalerScript : MonoBehaviour
{
    [EventID]
    public string eventID;
    void Awake()
    {
        Koreographer.Instance.RegisterForEventsWithTime(eventID, ScaleWithBeat);
    }

    private void ScaleWithBeat(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (evt.HasCurvePayload())
        {
            float curveValue = evt.GetValueOfCurveAtTime(sampleTime);

            transform.localScale = Vector3.one * curveValue;
        }
    }
}

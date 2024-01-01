using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cross : MonoBehaviour
{
    private Animator animator;
    public Color defaultFogColor;

    public Color missedFOgCOlor;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public IEnumerator ShowCross()
    {
        //StartCoroutine(FogColor());
        animator.SetBool("ScaleIn", true);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("ScaleIn", false);

    }

    public IEnumerator FogColor()
    {
        RenderSettings.fogColor = missedFOgCOlor;
        yield return new WaitForSeconds(0.1f);
        RenderSettings.fogColor = defaultFogColor;

    }

}

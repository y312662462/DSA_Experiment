using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundColor : MonoBehaviour
{
    private Renderer myRenderer;
    /*private bool changed = false;
    private float timer = 0f;
    private Color CurrentColor;
    private Color tempColor;*/

    
    public float smooth;
    public Color aimColor;
    //public receive_eeg eeg;
    
    // Start is called before the first frame update
    void Start()
    {
        myRenderer = GetComponent<Renderer>();
        //StartCoroutine(ChangeLerp());
    }

    // Update is called once per frame
    void Update()
    {

        

        /*switch (receive_eeg.Instance.AttentionLevel)
        {
            case 0:
                //myRenderer.material.SetColor("_EmissionColor", Color.black);
                RenderSettings.fogColor = Color.black;
                break;
            case 1:
                //myRenderer.material.SetColor("_EmissionColor", Color.red);
                RenderSettings.fogColor = Color.red;
                break;
            case 2:
                //myRenderer.material.SetColor("_EmissionColor", Color.green);
                RenderSettings.fogColor = Color.green;
                break;
            case 3:
                //myRenderer.material.SetColor("_EmissionColor", Color.blue);
                RenderSettings.fogColor = Color.blue;
                break;
            case 4:
                //myRenderer.material.SetColor("_EmissionColor", Color.yellow);
                RenderSettings.fogColor = Color.yellow;
                break;
        }*/
    }

    /*IEnumerator ChangeLerp()
    {
        timer = timer + smooth * Time.deltaTime;
        yield return 0;
    }*/
}

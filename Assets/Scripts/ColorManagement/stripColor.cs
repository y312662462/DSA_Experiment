using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist2;
using AudioVisualizer;



public class stripColor : MonoBehaviour
{
    
    public Color sColor;
    public Transform[] stripParent;
    public Renderer[] platRenderer;
    public VolumetricFog fogColor;
    //public LineWaveform[] curvWave;
    public ScoreManager scoreManager;

    private float timer = 0;
    private bool excuted;
    private Renderer[] stripRendererLeft;
    private Renderer[] stripRendererRight;

    

    // Start is called before the first frame update
    void Start()
    {
        //��ȡ�����������Ƶ�Renderer
        stripRendererLeft = stripParent[0].GetComponentsInChildren<Renderer>();
        stripRendererRight = stripParent[1].GetComponentsInChildren<Renderer>(); 
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: ͨ����ǰ�÷����Ƶ���������Լ�רע�ȸı�������ɫsColor
        if (scoreManager.TScore && receive_eeg.Instance.AttentionLevel == 1)
        {
            sColor = new Color(203f / 255f, 51f / 255f, 51f / 255f, 1f);
        }

        else if (scoreManager.TScore && receive_eeg.Instance.AttentionLevel == 2)
        {
            sColor = new Color(51f / 255f, 51f / 255f, 204f / 255f, 1f);
        }

        else if (!scoreManager.TScore && receive_eeg.Instance.AttentionLevel == 1)
        {
            sColor = new Color(203f / 255f, 51f / 255f, 51f / 255f, 1f);
        }



        /*else if (!scoreManager.TScore && receive_eeg.Instance.AttentionLevel == 2)
        {
            
        }*/


        //�ı�������ɫ
        foreach (var child in stripRendererLeft)
        {
            child.material.SetColor("_Color", sColor);
            child.material.SetColor("_EmissionColor", sColor);
            fogColor.profile.specularColor = sColor;
        }
        foreach (var child in stripRendererRight)
        {
            child.material.SetColor("_Color", sColor);
            child.material.SetColor("_EmissionColor", sColor);
            fogColor.profile.specularColor = sColor;
        }
        foreach  (var child in platRenderer)
        {
            child.material.SetColor("_Color", sColor);
            child.material.SetColor("_EmissionColor", sColor);
            fogColor.profile.specularColor = sColor;
        }

       
    }
}

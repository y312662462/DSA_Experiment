using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist2;
using UnityEngine.SceneManagement;


//scale:
//11640.15, 5820.076, 638.6889

public class spiralColor : MonoBehaviour
{
    public Color targetColor;
    public VolumetricFog fogColor;
    [Range(0, 50f)]
    public float rotateSpeed;
    [Range(0, 1f)]
    public float scaleSpeed;
    public ScoreManager scoreManager;
    public Renderer ren;
    public int loopTime;

    [SerializeField]
    private float s;
    private bool isIncrease;
    private Scene currentScene;
    private string[] sceneName;
    private int highScoreLowAttTimes;
    private int highScoreHighAttTimes;
    private int lowScoreLowAttTimes;
    private int lowScoreHighAttTimes;
    private int otherConditionTimes;
    private bool beginCalculate;

    // Start is called before the first frame update
    void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name.Split("_");
        s = 0f;
        StartCoroutine("numberLoop");
        highScoreLowAttTimes = 0;
        highScoreHighAttTimes = 0;
        lowScoreLowAttTimes = 0;
        lowScoreHighAttTimes = 0;
        otherConditionTimes = 0;
        beginCalculate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space ))
        {
            beginCalculate = true;

        }
        //temporary rotate&scale
        if (isIncrease)
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * rotateSpeed);
            transform.localScale -= new Vector3(0.001f * scaleSpeed, 0.001f * scaleSpeed, 0.001f * scaleSpeed);
        }
        else
        {
            transform.Rotate(Vector3.back * Time.deltaTime * rotateSpeed);
            transform.localScale += new Vector3(0.001f * scaleSpeed, 0.001f * scaleSpeed, 0.001f * scaleSpeed);
        }
        //set color
        if (sceneName[1] == "changecolor")
        {
            //成绩上升，低专注度，变成红色
            if (scoreManager.TScore && receive_eeg.Instance.AttentionLevel == 1)
            {
            //   targetColor = new Color(0f / 255f, 191f / 255f, 255f / 255f, 1f);//截图用
                 targetColor = new Color(203f / 255f, 51f / 255f, 51f / 255f, 1f);
                if (beginCalculate)
                     highScoreLowAttTimes += 1;
            }
            //成绩上升，高专注度，变成蓝色
            else if (scoreManager.TScore && receive_eeg.Instance.AttentionLevel == 2)
            {
                targetColor = new Color(51f / 255f, 51f / 255f, 204f / 255f, 1f);
                if (beginCalculate)
                    highScoreHighAttTimes += 1;
            }
            //成绩下降，低专注度，变成红色
            else if (!scoreManager.TScore && receive_eeg.Instance.AttentionLevel == 1)
            {
                //   targetColor = new Color(0f / 255f, 191f / 255f, 255f / 255f, 1f);//截图用
                targetColor = new Color(203f / 255f, 51f / 255f, 51f / 255f, 1f);
                if (beginCalculate)
                    lowScoreLowAttTimes += 1;
            }
            //成绩下降，高专注度，继承前面的颜色
            else if(!scoreManager.TScore && receive_eeg.Instance.AttentionLevel == 2)
            {
                if (beginCalculate)
                {
                    lowScoreHighAttTimes += 1;
                }
                   

            }
            ren.material.SetColor("_Color", targetColor);
            ren.material.color = targetColor;
            ren.material.SetColor("_EmissionColor", targetColor);
            fogColor.profile.specularColor = targetColor;
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            otherConditionTimes = highScoreLowAttTimes + highScoreHighAttTimes + lowScoreLowAttTimes;
            Debug.Log("Low Score but High Attention Times sum = " + lowScoreHighAttTimes);
            Debug.Log("High Score but Low Attention Times sun =" + highScoreLowAttTimes);
            Debug.Log("High Score and High Attention Times sun =" + highScoreHighAttTimes);
            Debug.Log("Low Score and Low Attention Times sun =" + lowScoreLowAttTimes);
            Debug.Log("Other Condition Times sum =" + otherConditionTimes);

            Debug.Log("All Condition Times sum =" + (otherConditionTimes+ lowScoreHighAttTimes));

        }
    }

    IEnumerator numberLoop()
    {
        
        while (true)
        {
            
            if (s <= 0)
                isIncrease = true;
            else if(s >= (loopTime * 4))
                isIncrease = false;

            if (isIncrease)
                s++;
            else
                s--;

            
            yield return new WaitForSeconds(0.25f);
        }
        
    }
}

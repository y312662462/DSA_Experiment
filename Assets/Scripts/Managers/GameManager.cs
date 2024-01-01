using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using FusiSDK;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI countDown;
    public GameObject startMusicAndSpawn;
    public ScoreManager scoreManager;
    public DataManager dataManager;
    public float timer;

    public GameObject levelObjects;

    private bool gameStarted;
    private string[] sceneName;

    [SerializeField]
    private SimpleMusicPlayer smp;


    void Start()
    {
        dataManager.intev = scoreManager.intev;
        Scene currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name.Split("_");      
        StartCoroutine(DetectInput());


    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1))
        {
            if (sceneName[1] == "changecolor")
            {
                receive_eeg.OnCloseBtnDown();
                SceneManager.LoadScene(sceneName[0] + "_nochangecolor");
            }
            else if (sceneName[1] == "nochangecolor")
            {
                receive_eeg.OnCloseBtnDown();
                SceneManager.LoadScene(sceneName[0] + "_changecolor");
            }

        }
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            SceneManager.LoadScene("stay_changecolor");
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            SceneManager.LoadScene("counting_changecolor");
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            SceneManager.LoadScene("party_changecolor");
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            SceneManager.LoadScene("show_changecolor");
        }
    }
    IEnumerator DetectInput()
    {
        while (true)
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                StartCoroutine(GameStarted());
                break;
            }
            

            yield return 0;
        }
    }


    IEnumerator GameStarted()
    {
        smp?.Play();
        yield return new WaitForSeconds(1f);
        countDown.text = "2";

        yield return new WaitForSeconds(1f);
        countDown.text = "1";


        yield return new WaitForSeconds(1f);
        levelObjects.SetActive(true);
        startMusicAndSpawn.SetActive(true);
        countDown.text = "GO";
        scoreManager.beginScore = true;
        dataManager.gameBegin = true;
        

        yield return new WaitForSeconds(1f);
        countDown.transform.parent.gameObject.SetActive(false);

    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*public class Score2Time
{
    private Queue<int> scoreQ;
    
    public int lastGradScore;
    public int qLen;
    public Score2Time(int len)
    {
        scoreQ = new Queue<int>(len);
        qLen = len;
        lastGradScore = 0;
        for(int i = 0; i < 5; i++)
        {
            scoreQ.Enqueue(0);
        }
    }

    public void scoreEnqueue(int score)
    {
        scoreQ.Enqueue(score);
    }

    public void scoreDequeue()
    {
        scoreQ.Dequeue();
    }

    public int headScore()
    {
        return scoreQ.Peek();
    }
}*/

public class ScoreManager : MonoBehaviour
{
    public int totalSpawns;
    public int currentScore;
    public int pulishSpawns;
    public bool TScore;
    public float timeWin;
    public bool beginScore;        
    public float totalScore;
    public float scoreRate;
    public int tScoreSpawn;
    public int tHitScoreSpawn;
    public int tHitPunishSpawn;
    public int intevTotalSpawn;
    public int intevScoreSpawn;
    public int intevPunishSpawn;
    public int lastTotalSpawn;
    public int lastScoreSpawn;


    public float intev;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI rateText;

    [HideInInspector] public Animator animator;

    private Queue<int> scoreQ;
    public int lastGradScore;



    void Start()
    {
        
        int length = (int)(timeWin / intev); 
        beginScore = false;
        animator = scoreText.transform.parent.GetComponent<Animator>();
        scoreQ = new Queue<int>(length);            
        for (int i = 0; i < length; i++)
            scoreQ.Enqueue(0);                      
        //Debug.Log("len" + scoreQ.Count);
        InvokeRepeating("wirteScore", 1f, intev);  
      
    }

    void Update()
    {
        totalScore = totalSpawns * 25 + 0.01f;
        scoreRate = (float)currentScore / (float)totalScore * 100f;
        scoreText.text = currentScore.ToString();
        rateText.text = string.Format("{0:f2}", scoreRate);
        tScoreSpawn = totalSpawns - pulishSpawns;
    }

    public void wirteScore()
    {
        if(beginScore)
        {
            intevTotalSpawn = totalSpawns - lastTotalSpawn;
            intevScoreSpawn = tScoreSpawn - lastScoreSpawn;
            intevPunishSpawn = intevTotalSpawn - intevScoreSpawn;
            lastScoreSpawn = tScoreSpawn;
            lastTotalSpawn = totalSpawns;
            scoreQ.Dequeue();
            if (currentScore - scoreQ.Peek() <= lastGradScore)
                TScore = false;
            else
            {
                TScore = true;
            }
            lastGradScore = currentScore - scoreQ.Peek();
            scoreQ.Enqueue(currentScore);
            Debug.Log("TScore£º" + TScore);
        }

    }


    public void MissedCube()
    {
        if (currentScore - 15 >= 0)
        {
            currentScore -= 15;
        }
        else
        {
            currentScore = 0;
        }
    }

    public void CorrectColorCube()
    {
        currentScore += 25;
        tHitScoreSpawn++;
    }

    public void WrongColorCube()
    {
        currentScore += 25;
        tHitScoreSpawn++;
    }

    public void PulishCube()
    {
        if (currentScore - 15 >= 0)
        {
            currentScore -= 15;
        }
        else
        {
            currentScore = 0;
        }
        tHitPunishSpawn++;
    }

    
}

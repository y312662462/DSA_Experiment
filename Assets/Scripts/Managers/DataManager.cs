using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Data;
using Excel;
using OfficeOpenXml;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    // Start is called before the first frame update
    //attention                                 1   tAttention						
    //瞬时总分数                                2   tTotalScore			
    //瞬时得分比							    3   tScoreRate			
    //瞬时TScore状态							4   TScore			
    //瞬时生成总方块数				            5   tTotalSpawn					
    //瞬时生成得分方块数						6   tScoreSpawn				
    //瞬时生成惩罚方块数						7   tPunishSpawn			
    //瞬时命中得分方块数						8   tHitScoreSpawn			
    //瞬时命中惩罚方块数						9   tHitPunishSpawn			
    //每个lastGradScore记录                     10  lastGradScore			
    //每个intev间隔内，生成的总方块数		    11  intevTotalSpawn			
    //每个intev间隔内，生成的得分块数			12	intevScoreSpawn						
    //每个intev间隔内，生成的惩罚方块数			13	intevPunishSpawn					
    private string tTime;
    private string[] tData;

    public bool gameBegin;
    public ScoreManager scoreManager;
    public float intev;

    private string filePath;
    private DataTable dt;


    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        filePath = (Application.streamingAssetsPath + "\\" + 
                    System.DateTime.Now.Year.ToString() + "." +
                    System.DateTime.Now.Month.ToString() + "." +
                    System.DateTime.Now.Day.ToString() + "." +
                    System.DateTime.Now.Hour.ToString() + "." +
                    System.DateTime.Now.Minute.ToString() + "." +
                    System.DateTime.Now.Second.ToString() + 
                    "_" + currentScene.name + "_" + "Datasave.csv");
        dt = new DataTable("sheet1");
        dt.Columns.Add("时间");
        dt.Columns.Add("attention");
        dt.Columns.Add("瞬时总分数");
        dt.Columns.Add("瞬时得分比");
        dt.Columns.Add("瞬时TScore状态");
        dt.Columns.Add("瞬时生成总方块数");
        dt.Columns.Add("瞬时生成得分方块数");
        dt.Columns.Add("瞬时生成惩罚方块数");
        dt.Columns.Add("瞬时命中得分方块数");
        dt.Columns.Add("瞬时命中惩罚方块数");
        dt.Columns.Add("每个lastGradScore记录");
        dt.Columns.Add("每个intev间隔内，生成的总方块数");
        dt.Columns.Add("每个intev间隔内，生成的得分块数");
        dt.Columns.Add("每个intev间隔内，生成的惩罚方块数");



        StartCoroutine(writeData_intev());
        tData = new string[13];
        gameBegin = false;
    }

    // Update is called once per frame
    void Update()
    {
        tTime = System.DateTime.Now.ToString("yyyy:MM:dd:HH:mm:ss:ffffff");

        tData[0] = receive_eeg.Instance.Attention.ToString();
        tData[1] = scoreManager.currentScore.ToString();
        tData[2] = scoreManager.scoreRate.ToString();
        tData[3] = scoreManager.TScore.ToString();
        tData[4] = scoreManager.totalSpawns.ToString();
        tData[5] = scoreManager.tScoreSpawn.ToString();
        tData[6] = scoreManager.pulishSpawns.ToString();
        tData[7] = scoreManager.tHitScoreSpawn.ToString();
        tData[8] = scoreManager.tHitPunishSpawn.ToString();
        tData[9] = scoreManager.lastGradScore.ToString();
        tData[10] = scoreManager.intevTotalSpawn.ToString();
        tData[11] = scoreManager.intevScoreSpawn.ToString();
        tData[12] = scoreManager.intevPunishSpawn.ToString();


        if (Input.GetKeyUp(KeyCode.Tab))
        {
            Debug.Log("pressed");
            gameBegin = false;
            Invoke("SaveCsvData", 0f);
        }
    }



    IEnumerator writeData_intev()
    {
        while (true)
        {
            //游戏开始进行记录数据并写入；游戏结束则停止
            if (gameBegin)
            {
                DataRow dr = dt.NewRow();
                dr["时间"] = tTime;
                dr["attention"] = tData[0];
                dr["瞬时总分数"] = tData[1];
                dr["瞬时得分比"] = tData[2];
                dr["瞬时TScore状态"] = tData[3];
                dr["瞬时生成总方块数"] = tData[4];
                dr["瞬时生成得分方块数"] = tData[5];
                dr["瞬时生成惩罚方块数"] = tData[6];
                dr["瞬时命中得分方块数"] = tData[7];
                dr["瞬时命中惩罚方块数"] = tData[8];
                dr["每个lastGradScore记录"] = tData[9];
                dr["每个intev间隔内，生成的总方块数"] = tData[10];
                dr["每个intev间隔内，生成的得分块数"] = tData[11];
                dr["每个intev间隔内，生成的惩罚方块数"] = tData[12];
                dt.Rows.Add(dr);
            }
            yield return new WaitForSeconds(intev);
        }
    }

    private void SaveCsvData()
    {
        FileInfo fi = new FileInfo(filePath);
        if (!fi.Directory.Exists)
        {
            fi.Directory.Create();
        }
        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
            {
                string data = "";
                //写入表头
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    data += dt.Columns[i].ColumnName.ToString();
                    if (i < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
                //写入每一行每一列的数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    data = "";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string str = dt.Rows[i][j].ToString();
                        data += str;
                        if (j < dt.Columns.Count - 1)
                        {
                            data += ",";
                        }
                    }
                    sw.WriteLine(data);
                }
                sw.Close();
                fs.Close();
            }
        }
    }
}

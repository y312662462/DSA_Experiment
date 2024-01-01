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
    //˲ʱ�ܷ���                                2   tTotalScore			
    //˲ʱ�÷ֱ�							    3   tScoreRate			
    //˲ʱTScore״̬							4   TScore			
    //˲ʱ�����ܷ�����				            5   tTotalSpawn					
    //˲ʱ���ɵ÷ַ�����						6   tScoreSpawn				
    //˲ʱ���ɳͷ�������						7   tPunishSpawn			
    //˲ʱ���е÷ַ�����						8   tHitScoreSpawn			
    //˲ʱ���гͷ�������						9   tHitPunishSpawn			
    //ÿ��lastGradScore��¼                     10  lastGradScore			
    //ÿ��intev����ڣ����ɵ��ܷ�����		    11  intevTotalSpawn			
    //ÿ��intev����ڣ����ɵĵ÷ֿ���			12	intevScoreSpawn						
    //ÿ��intev����ڣ����ɵĳͷ�������			13	intevPunishSpawn					
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
        dt.Columns.Add("ʱ��");
        dt.Columns.Add("attention");
        dt.Columns.Add("˲ʱ�ܷ���");
        dt.Columns.Add("˲ʱ�÷ֱ�");
        dt.Columns.Add("˲ʱTScore״̬");
        dt.Columns.Add("˲ʱ�����ܷ�����");
        dt.Columns.Add("˲ʱ���ɵ÷ַ�����");
        dt.Columns.Add("˲ʱ���ɳͷ�������");
        dt.Columns.Add("˲ʱ���е÷ַ�����");
        dt.Columns.Add("˲ʱ���гͷ�������");
        dt.Columns.Add("ÿ��lastGradScore��¼");
        dt.Columns.Add("ÿ��intev����ڣ����ɵ��ܷ�����");
        dt.Columns.Add("ÿ��intev����ڣ����ɵĵ÷ֿ���");
        dt.Columns.Add("ÿ��intev����ڣ����ɵĳͷ�������");



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
            //��Ϸ��ʼ���м�¼���ݲ�д�룻��Ϸ������ֹͣ
            if (gameBegin)
            {
                DataRow dr = dt.NewRow();
                dr["ʱ��"] = tTime;
                dr["attention"] = tData[0];
                dr["˲ʱ�ܷ���"] = tData[1];
                dr["˲ʱ�÷ֱ�"] = tData[2];
                dr["˲ʱTScore״̬"] = tData[3];
                dr["˲ʱ�����ܷ�����"] = tData[4];
                dr["˲ʱ���ɵ÷ַ�����"] = tData[5];
                dr["˲ʱ���ɳͷ�������"] = tData[6];
                dr["˲ʱ���е÷ַ�����"] = tData[7];
                dr["˲ʱ���гͷ�������"] = tData[8];
                dr["ÿ��lastGradScore��¼"] = tData[9];
                dr["ÿ��intev����ڣ����ɵ��ܷ�����"] = tData[10];
                dr["ÿ��intev����ڣ����ɵĵ÷ֿ���"] = tData[11];
                dr["ÿ��intev����ڣ����ɵĳͷ�������"] = tData[12];
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
                //д���ͷ
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    data += dt.Columns[i].ColumnName.ToString();
                    if (i < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
                //д��ÿһ��ÿһ�е�����
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

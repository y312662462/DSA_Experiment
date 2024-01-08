using UnityEngine;
using FusiSDK;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections;
using UnityEngine.SceneManagement;

using Excel;
using OfficeOpenXml;
using System.Data;
using System.IO;
using System.Text;

public class receive_eeg : MonoBehaviour
{
    private static receive_eeg instance;
    public static receive_eeg Instance
    {
        get { return instance; }
    }

    myListener mylistener;

    // string TARGET_MAC = "98d863398764";
    string TARGET_MAC = "f0fe6bd409a4";

    public static FusiHeadband headband;


    static List<string> attention_history = new List<string>();
    static List<string> meditation_history = new List<string>();
    static List<string> eeg_data_pga = new List<string>();
    static List<string> eeg_data_samplerate = new List<string>();
    static List<string[]> brain_wave_data = new List<string[]>();
    static List<string> time = new List<string>();

    static DataTable dt;
    static string filePath;

    private int attentionLevel = 0;

    public string participantsName;

    public int AttentionLevel
    {
        get { return attentionLevel; }
    }

    private double attention = 0;
    public double Attention
    {
        get { return attention; }
    }


    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //API.SearchDevices(on_found_devices, on_search_error);

        //创建表 设置表名
        dt = new DataTable(participantsName + "brain_wave_data");
        filePath = Application.streamingAssetsPath + "\\" + participantsName + "brain_wave_data.csv";
        mylistener = new myListener();
        StartCoroutine(SearchDevice());
    }

    // Update is called once per frame
    void Update()
    {
        OnCloseBtnDown();
        //PressD1toLoadLevel();
        if (Input.GetKeyUp(KeyCode.Tab))//可替换为检测到游戏结束
        {
            headband.Disconnect();
            SaveCsv(filePath, dt);
            Debug.Log("Data Write Over!");
        }
    }

    IEnumerator SearchDevice()
    {
        while (!mylistener.isConnected)
        {
            API.SearchDevices(on_found_devices, on_search_error);
            yield return 0;
        }
    }

    public void on_found_devices(FusiHeadband[] devices)
    {
        //Debug.Log(3);
        foreach (FusiHeadband device in devices)
        {
            Debug.Log(device.Mac);
            if (device.Mac == TARGET_MAC)
            {
                headband = device;

                //Debug.Log(4);
            }
        }
        if (headband == null)
        {
            Debug.Log("No device found");
        }
        else
        {
            headband.SetListener(mylistener);
            headband.Connect();
        }

    }

    public void on_search_error(FusiHeadbandError error_msg)
    {
        Debug.Log(5);
        Debug.Log(error_msg);
    }

    public FusiHeadband GetHeadband()
    {
        Debug.Log(headband == null);
        return headband;
    }

    class myListener : FusiHeadbandListener
    {
        public bool isConnected = false;
        private int attention_level = 0;


        public override void OnConnectionChange(HeadbandConnectionState connection_state)
        {
            Debug.Log("Connection state changed");
            if (connection_state == HeadbandConnectionState.Connected)
            {
                Debug.Log(headband == null);
                headband.SetForeheadLEDColor(0, 0, 255);
                Debug.Log("Headband connected");
                isConnected = true;
            }
            else if (connection_state == HeadbandConnectionState.Interrupted)
            {
                Debug.Log("Headband connection interrupted");
            }
            else if (connection_state == HeadbandConnectionState.Disconnected)
            {
                Debug.Log("Headband disconnected");
            }
        }
        public override void OnAttention(double attention)
        {
            Debug.Log(attention);
            /*if (attention < 50)
            {
                attention_level = 1;
            }
            else if (attention < 60)
            {
                attention_level = 2;
            }
            else if (attention < 70)
            {
                attention_level = 3;
            }
            else if (attention < 100)
            {
                attention_level = 4;
            }*/

            if (attention < 60)
                attention_level = 1;
            else if (attention >= 60)
                attention_level = 2;
            receive_eeg.Instance.attentionLevel = attention_level;
            receive_eeg.Instance.attention = attention;
            Debug.Log("attention_level：" + attention_level);
            attention_history.Add(attention.ToString("#0.000"));
            attention_history.Add(attention.ToString("#0.000"));
        }

        public override void OnMeditation(double meditation)
        {
            //Debug.Log("meditation："+meditation);
            meditation_history.Add(meditation.ToString("#0.000"));
            meditation_history.Add(meditation.ToString("#0.000"));
            //dr["Meditation"] = meditation.ToString("#0.000");
        }
        public override void OnEEGData(EEG data)
        {
            eeg_data_pga.Add(data.PGA.ToString("#0.000"));
            //Debug.Log("pga write over");
            eeg_data_samplerate.Add(data.SampleRate.ToString("#0.000"));
            //Debug.Log("sample_rate write over");
            //dr["eeg_data.pga"] = data.PGA.ToString("#0.000");
            //dr["eeg_data.sample_rate"] = data.SampleRate.ToString("#0.000");
        }

        public override void OnBrainWave(BrainWave wave)
        {
            string[] waves = new string[6];
            waves[0] = wave.Delta.ToString("#0.000");
            waves[1] = wave.Theta.ToString("#0.000");
            waves[2] = wave.Alpha.ToString("#0.000");
            waves[3] = wave.LowBeta.ToString("#0.000");
            waves[4] = wave.HighBeta.ToString("#0.000");
            waves[5] = wave.Gamma.ToString("#0.000");
            brain_wave_data.Add(waves);
            time.Add(DateTime.Now.ToString("yyyy:MM:dd:HH:mm:ss:ffffff"));
            //dr["brain_wave.alpha"] = wave.Alpha.ToString("#0.000");
            //dr["brain_wave.delta"] = wave.Delta.ToString("#0.000");
            //dr["brain_wave.gamma"] = wave.Gamma.ToString("#0.000");
            //dr["brain_wave.high_beta"] = wave.HighBeta.ToString("#0.000");
            //dr["brain_wave.low_beta"] = wave.LowBeta.ToString("#0.000");
            //dr["brain_wave.theta"] = wave.Theta.ToString("#0.000");
            //dr["time"] = DateTime.Now.ToString("yyyy:MM:dd:HH:mm:ss:ffffff");
            //Debug.Log("time："+ DateTime.Now.ToString("yyyy:MM:dd:HH:mm:ss:ffffff"));
            //dt.Rows.Add(dr);
            //Debug.Log("dt.Rows.Add(dr)");
            //SaveCsvData(filePath, dt,dr);
        }




    }
    public static void OnCloseBtnDown()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (headband != null)
            {
                headband.Disconnect();
                Debug.Log("Headband disconnected succeed");
            }
            /*
#if UNITY_EDITOR  //在编辑器模式下

            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif */
        }

    }
    /*    public static void PressD1toLoadLevel()
        {
            if (Input.GetKeyUp(KeyCode.F1))
            {
                if (headband != null)
                {
                    headband.Disconnect();
                    Debug.Log("Headband disconnected succeed");
                }

                Scene currentScene = SceneManager.GetActiveScene();
                if (currentScene.name.Equals("changeColor"))
                    Application.LoadLevel("nochangeColor");
                else
                    Application.LoadLevel("changeColor");
            }

        }*/

    // 将数据写入到CSV文件中
    public static void SaveCsv(string filePath, DataTable dt)
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
                sw.WriteLine("Attention,Meditation,brain_wave.alpha,brain_wave.delta,brain_wave.gamma,brain_wave.high_beta," +
                    "brain_wave.low_beta,brain_wave.theta,eeg_data.pga,eeg_data.sample_rate,time");

                //写入每一行每一列的数据
                for (int i = 0; i < attention_history.Count; i++)
                {
                    data = attention_history[i] + "," + meditation_history[i] + "," + brain_wave_data[i][2] + "," +
                        brain_wave_data[i][0] + "," + brain_wave_data[i][5] + "," + brain_wave_data[i][4] + "," +
                        brain_wave_data[i][3] + "," + brain_wave_data[i][1] + "," + eeg_data_pga[i] + "," +
                        eeg_data_samplerate[i] + "," + time[i];
                    sw.WriteLine(data);
                }
                sw.Close();
                fs.Close();
            }
        }
    }


}

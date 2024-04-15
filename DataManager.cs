using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.IO;

public class PlayerData
{
    //플레이어 데이터
    public int Credits; //현재 가지고있는 돈 (게임중의 돈이 아닌 게임이 끝나면 주어지는 크레딧)
    public int Difficulty_1_HighScore; //난이도 쉬움 최고점수
    public int Difficulty_2_HighScore; //난이도 보통 최고점수
    public int Difficulty_3_HighScore; //난이도 어려움 최고점수
    public int Difficulty_4_HighScore; //난이도 지옥 최고점수
    //현 해금된 타워 목록
    //추가예정~
}

public class DataManager : MonoBehaviour
{
    //Options
    public bool ZoomToggleMode;
    public bool BoostToggleMode;
    public bool isDebugMode;
    public bool ShowFPS;
    public VolumeProfile Vol;

    public static DataManager instance;

    string datapath;
    string filename = "saves";

    PlayerData nowPlayer = new PlayerData();

    //KeyBinds
    public KeyCode Run;
    public KeyCode Boost;
    public KeyCode Upgrade;
    public KeyCode Sell;
    public KeyCode Interact;
    public KeyCode Rummage;
    public KeyCode Ride;
    public KeyCode TowerZoom;
    public KeyCode Reload;
    public KeyCode Build;
    public KeyCode BuildGuide_TurnLeft;
    public KeyCode BuildGuide_TurnRight;
    public KeyCode NextTower;
    public KeyCode PreviousTower;
    public KeyCode FlashlightToggle;
    public KeyCode Teleport;
    public KeyCode BackToBase;

    //싱글톤용이빈다.
    private void Awake()
    {
        Vol = GameObject.Find("Global Volume").GetComponent<Volume>().profile;

        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        datapath = Application.persistentDataPath + "/";
    }

    // Start is called before the first frame update
    void Start()
    {
        if(File.ReadAllText(datapath + filename) != null)
        {
            LoadData();
        }
        else
        {
            InitData();
        }
    }

    public void Recomponent()
    {
        Vol = GameObject.Find("Global Volume").GetComponent<Volume>().profile;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            nowPlayer.Credits += 50;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SaveData();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            LoadData();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log(nowPlayer.Credits);
        }
    }

    public void InitData()
    {
        nowPlayer.Credits = 0;
        nowPlayer.Difficulty_1_HighScore = 0;
        nowPlayer.Difficulty_2_HighScore = 0;
        nowPlayer.Difficulty_3_HighScore = 0;
        nowPlayer.Difficulty_4_HighScore = 0;

        SaveData();
    }

    public void SaveData()
    {
        string data = JsonUtility.ToJson(nowPlayer);
        File.WriteAllText(datapath + filename, data);
    }

    public void LoadData()
    {
        string data = File.ReadAllText(datapath + filename);
        PlayerData loaded_data = JsonUtility.FromJson<PlayerData>(data);
        nowPlayer.Credits = loaded_data.Credits;
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagement : MonoBehaviour
{
    public DataManager Gdata;

    public bool isTestMode;
    public bool isLobby;
    public bool isWave;
    public bool isLastWave;
    public float TowerDownAutoRepairTime;
    public float CriticalChance;
    public int NumOfWaves = 10;
    public int NumOfEnemies;
    public int CurrentWave;

    public float WaveFreeTime;
    public float waveFreeTimeTimer;

    public List<WaveSpawnData> WaveSpawnDataList = new List<WaveSpawnData>();
    public bool SpawnDone;
    public Transform SpawnPositionTransform;
    public Transform PlayerBasePosition;

    void Start()
    {
        Gdata = GetComponent<DataManager>();
        if(!isTestMode)
            SpawnPositionTransform = transform.Find("SpawnPositionTransform").GetComponent<Transform>();
        isLobby = true;
        if (isLobby) return;

        waveFreeTimeTimer = WaveFreeTime;
        isWave = false;
        isLastWave = false;
        SpawnDone = false;
    }

    public void GameStart()
    {
        Gdata.Recomponent();
        isLobby = false;
        if (!isTestMode)
            SpawnPositionTransform = transform.Find("SpawnPositionTransform").GetComponent<Transform>();

        waveFreeTimeTimer = WaveFreeTime;
        isWave = false;
        isLastWave = false;
        SpawnDone = false;
    }

    public void FindPlayerBaseTransform()
    {
        if (!isTestMode)
            PlayerBasePosition = GameObject.Find("MainBase").GetComponent<Transform>();
    }

    void Update()
    {
        if (isLobby) return;

        if (NumOfEnemies < 0)
            NumOfEnemies = 0;

        if (!isWave)
        {
            waveFreeTimeTimer -= Time.deltaTime;
            if(waveFreeTimeTimer < 0)
            {
                NextWave();
            }
        }
        else
        {
            if (NumOfEnemies <= 0 && SpawnDone && !isLastWave)
                WaveEnded();
        }
    }

    public void WaveEnded()
    {
        waveFreeTimeTimer = WaveFreeTime;
        isWave = false;
    }

    public void NextWave()
    {
        Debug.Log("NEXT WAVE");

        SpawnDone = false;
        isWave = true;
        CurrentWave++;
        if (CurrentWave >= NumOfWaves)
        {
            isLastWave = true;
        }
        StartCoroutine(SpawnCoroutine());
    }

    public IEnumerator SpawnCoroutine()
    {
        for (int i = 0; i < WaveSpawnDataList[CurrentWave - 1].SpawnEntityInfo.Count; i++)
        {
            yield return new WaitForSeconds(WaveSpawnDataList[CurrentWave - 1].SpawnEntityInfo[i].PreDelayTime);

            SpawnPositionTransform.position = new Vector3(PlayerBasePosition.position.x, PlayerBasePosition.position.y + WaveSpawnDataList[CurrentWave - 1].SpawnEntityInfo[i].SpawnHeight, PlayerBasePosition.position.z);
            SpawnPositionTransform.Rotate(0, Random.Range(0, 360), 0);
            SpawnPositionTransform.Translate(SpawnPositionTransform.forward * Random.Range(WaveSpawnDataList[CurrentWave - 1].SpawnEntityInfo[i].MinDistanceFromPlayerBase, WaveSpawnDataList[CurrentWave - 1].SpawnEntityInfo[i].MaxDistanceFromPlayerBase));
            GameObject spawned = Instantiate(WaveSpawnDataList[CurrentWave - 1].SpawnEntityInfo[i].SpawnEnemyPrefab, SpawnPositionTransform.position, Quaternion.identity);
            NumOfEnemies++;

            yield return new WaitForSeconds(WaveSpawnDataList[CurrentWave - 1].SpawnEntityInfo[i].AfterDelayTime);
        }
        yield return new WaitForSeconds(1);
        SpawnDone = true;
        StopAllCoroutines();
    }

    public void GameEnded()
    {
        SceneManager.LoadScene("TitleSceneTemp", LoadSceneMode.Single);
        isLobby = true;
    }
}

using Demo.Scripts.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct RoundConfigs
{
    public List<float> roundFPS;
    public List<float> spikeMagnitude;
    public List<bool> onAimSpikeEnabled;
    public List<bool> onReloadSpikeEnabled;
}

public class RoundManager : MonoBehaviour
{
    public RoundConfigs roundConfigs;

    public float roundDuration;

    public float roundTimer;

    public int totalRoundNumber;

    List<int> indexArray = new List<int>();

    GameManager gameManager;

    public int currentRoundNumber;

    String sessionStartTime;

    public FPSController playerController;

    long roundFrameCount = 0;
    double frametimeCumulativeRound = 0;

    public string fileNameSuffix = "";
    public String filenamePerTick = "Data\\ClientDataPerTick.csv";
    public String filenamePerRound = "Data\\ClientDataPerRound.csv";

    public float qoeValue;

    // Start is called before the first frame update
    void Start()
    {
        currentRoundNumber = 1;
        fileNameSuffix = GenRandomID(6).ToString();
        sessionStartTime = System.DateTime.Now.ToString("yy:mm:dd:hh:mm:ss");

        gameManager = GetComponent<GameManager>();

        ReadCSV();

        totalRoundNumber = roundConfigs.roundFPS.Count;

        for (int i = 0; i < totalRoundNumber; i++)
        {
            indexArray.Add(i);
        }

        // Shuffle the list
        Shuffle(indexArray);


        /*//Add practice round
        int temp = indexArray[numberOfRounds - 1];
        indexArray[numberOfRounds - 1] = indexArray[0];
        indexArray[0] = 0;
        indexArray.Add(temp);
        indexArray.Add(0);
        numberOfRounds++;*/


        playerController.isQoeDisabled = true;
        SetRounConfig();
    }
    void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(roundTimer > 0 && playerController.isPlayerReady && playerController.isQoeDisabled) { 
            roundTimer-= Time.deltaTime;
            frametimeCumulativeRound += Time.deltaTime;
            roundFrameCount++;
        }
        else if (roundTimer <= 0 && playerController.isQoeDisabled)
        {
            playerController.isQoeDisabled = false;
            playerController.ResetPlayerAndDestroyEnemy();
        }
    }

    public void ReadCSV()
    {
        string line = null;
        StreamReader strReader = new StreamReader("Data\\Configs\\RoundConfig.csv");
        bool EOF = false;
        roundConfigs.roundFPS.Clear();

        while (!EOF)
        {
            line = strReader.ReadLine();

            if (line == null)
            {
                EOF = true;
                break;
            }
            else
            {
                var dataValues = line.Split(',');
                Debug.Log(dataValues[1]);
                roundConfigs.roundFPS.Add(float.Parse(dataValues[0]));
                roundConfigs.spikeMagnitude.Add(float.Parse(dataValues[1]));
                roundConfigs.onAimSpikeEnabled.Add(bool.Parse(dataValues[2]));
                roundConfigs.onReloadSpikeEnabled.Add(bool.Parse(dataValues[3]));
            }
        }
    }

    String GenRandomID(int len)
    {
        String alphanum = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        String tmp_s = "";

        for (int i = 0; i < len; ++i)
        {
            tmp_s += alphanum[UnityEngine.Random.Range(0, 1000) % (alphanum.Length - 1)];
        }

        return tmp_s;
    }

    public void SetRounConfig()
    {
        roundTimer = roundDuration;
        gameManager.isFixedFT = false;

        Application.targetFrameRate = (int)roundConfigs.roundFPS[indexArray[currentRoundNumber - 1]];


        playerController.isAimSpikeEnabled = roundConfigs.onAimSpikeEnabled[indexArray[currentRoundNumber - 1]];
        playerController.isReloadSpikeEnabled = roundConfigs.onReloadSpikeEnabled[indexArray[currentRoundNumber - 1]];

        roundFrameCount = 0;
        frametimeCumulativeRound = 0;

    }

    public void LogRoundData()
    {
            PlayerStats stats = playerController.gameObject.GetComponent<PlayerStats>();

            TextWriter textWriter = null;
            filenamePerRound = "Data\\Logs\\RoundData_"+ fileNameSuffix+".csv";

            while (textWriter == null)
                textWriter = File.AppendText(filenamePerRound);

            float accuracy = 0;
            if (playerController.shotsFiredPerRound > 0)
            {
                accuracy = (float)playerController.shotsHitPerRound / (float)playerController.shotsFiredPerRound;
            }

        double avgFT = frametimeCumulativeRound / roundFrameCount;
        double avgFPS = 1 / avgFT;

            String roundLogLine =
               currentRoundNumber.ToString() + "," +
               sessionStartTime.ToString() + "," +  
               System.DateTime.Now.ToString() + "," +
               roundConfigs.roundFPS[indexArray[currentRoundNumber - 1]].ToString() + "," +
               indexArray[currentRoundNumber - 1].ToString() + "," +
               playerController.score + "," +
               playerController.shotsFiredPerRound + "," +
               playerController.shotsHitPerRound + "," +
               playerController.headshotsHitPerRound + "," + 
               playerController.realoadCountPerRound + "," +
               accuracy.ToString() + "," +
               playerController.roundKills + "," +
               playerController.roundDeaths + "," +
               playerController.distanceTravelledPerRound + "," +
               playerController.delXCumilative.ToString() + "," +
               playerController.delYCumilative.ToString() + "," +
               (playerController.delXCumilative + playerController.delYCumilative).ToString() + "," +
               frametimeCumulativeRound.ToString() + "," +
               roundFrameCount.ToString() + "," +
               avgFT.ToString() + "," +
               avgFPS.ToString() + ","+
               qoeValue.ToString()
                ;
            textWriter.WriteLine(roundLogLine);
            textWriter.Close();
        }
    

}

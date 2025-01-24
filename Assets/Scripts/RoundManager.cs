using Demo.Scripts.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public List<RoundConfig> roundConfigs;

    public float roundDuration;

    public float roundTimer;

    public int totalRoundNumber;

    public List<int> indexArray = new List<int>();

    public GameManager gameManager;

    public int currentRoundNumber;

    public String sessionStartTime;

    public FPSController playerController;

    public long roundFrameCount = 0;
    public double frametimeCumulativeRound = 0;

    public string fileNameSuffix = "";
    public String filenamePerTick = "Data\\ClientDataPerTick.csv";
    public String filenamePerRound = "Data\\ClientDataPerRound.csv";

    public float qoeValue;

    public bool acceptabilityValue;

    public int sessionID = -1;

    public bool isFTStudy;

    public int latinSquareRowNumber = 0;

    public int latinRow;

    public RoundConfig currentRoundConfig => roundConfigs[indexArray[currentRoundNumber - 1]];

    // Start is called before the first frame update
    void Start()
    {
        latinSquareRowNumber = 0;
        currentRoundNumber = 1;
        fileNameSuffix = GenRandomID(6).ToString();
        sessionStartTime = System.DateTime.Now.ToString("yy:mm:dd:hh:mm:ss");

        gameManager = GetComponent<GameManager>();

        ReadGlobalConfig();

        ReadLatinSquareSize();

        ReadFromLatinSquare();

        totalRoundNumber = roundConfigs.Count;

        for (int i = 0; i < totalRoundNumber; i++)
        {
            indexArray.Add(i);
        }

        // Shuffle the list
        //Shuffle(indexArray);

        //LEGACY CODE
        //Add practice round
        /*int temp = indexArray[totalRoundNumber - 1];
        indexArray[totalRoundNumber - 1] = indexArray[0];
        indexArray[0] = 0;
        indexArray.Add(temp);
        indexArray.Add(0);
        totalRoundNumber++;*/


        playerController.isQoeDisabled = true;
        playerController.isAcceptabilityDisabled = true;
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
        if (roundTimer > 0 && playerController.isPlayerReady && playerController.isQoeDisabled && playerController.isAcceptabilityDisabled)
        {
            roundTimer -= Time.deltaTime;
            frametimeCumulativeRound += Time.deltaTime;
            roundFrameCount++;
        }
        else if (roundTimer <= 0 && playerController.isQoeDisabled && playerController.isAcceptabilityDisabled)
        {
            playerController.isQoeDisabled = false;
            playerController.ResetPlayerAndDestroyEnemy();
        }
    }

    void ReadLatinSquareSize()
    {
        if (isFTStudy)
        {
            bool EOF = false;
            string line = null;
            StreamReader strReader = new StreamReader("Data\\Configs\\RoundConfig.csv");
            EOF = false;

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
                    latinSquareRowNumber++;
                    Debug.Log("LATIN SQUARE ROW COUNT = " + latinSquareRowNumber);
                }
            }

            Debug.Log("LATIN SQUARE final ROW COUNT = " +latinSquareRowNumber);
        }
    }
    void ReadGlobalConfig()
    {
        string line = null;
        StreamReader strReader = new StreamReader("Data\\Configs\\GlobalConfig.csv");
        bool EOF = false;
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
                roundDuration = float.Parse(dataValues[0]);
                isFTStudy = bool.Parse(dataValues[1]);
                playerController.aimSpikeDelay = float.Parse(dataValues[2]);
                playerController.mouseSpikeDelay = float.Parse(dataValues[3]);
                playerController.mouseSpikeDegreeOfMovement = float.Parse(dataValues[4]);
                playerController.enemySpeedGlobal = float.Parse(dataValues[5]);
                playerController.enemyHealthGlobal = float.Parse(dataValues[6]);
                playerController.reticleSizeMultiplier = float.Parse(dataValues[7]);

                playerController.onHitScore = int.Parse(dataValues[8]);
                playerController.onMissScore = int.Parse(dataValues[9]);
                playerController.onKillScore = int.Parse(dataValues[10]);
                playerController.onDeathScore = int.Parse(dataValues[11]);
            }
        }
    }

    public void ReadFromLatinSquare()
    {
        string line = null;
        StreamReader strReader = new StreamReader("Data\\Configs\\SessionID.csv");
        bool EOF = false;
        roundConfigs.Clear();

        sessionID = -1;

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
                sessionID = int.Parse(dataValues[0]);
            }
        }



        line = null;
        strReader = new StreamReader("Data\\Configs\\LatinSquare.csv");
        EOF = false;
        int index = 1;

        if (!isFTStudy)
        {
            //Practice
            roundConfigs.Add(new RoundConfig {
                roundFPS = 500,
                spikeMagnitude = 100,
                onAimSpikeEnabled = false,
                onReloadSpikeEnabled = false,
                onMouseSpikeEnabled = false,
                onEnemySpawnSpikeEnabled = false,
            });

            roundConfigs.Add(new RoundConfig {
                roundFPS = 7,
                spikeMagnitude = 100,
                onAimSpikeEnabled = false,
                onReloadSpikeEnabled = false,
                onMouseSpikeEnabled = false,
                onEnemySpawnSpikeEnabled = false,
            });

            while ((line = strReader.ReadLine()) != null)
            {

                var dataValues = line.Split(',');
                if (index == sessionID)
                {
                    for (int i = 0; i < dataValues.Length; i++)
                        roundConfigs.Add(new RoundConfig {
                            roundFPS = float.Parse(dataValues[i]),
                            spikeMagnitude = 100,
                            onAimSpikeEnabled = false,
                            onReloadSpikeEnabled = false,
                            onMouseSpikeEnabled = false,
                            onEnemySpawnSpikeEnabled = false,
                        });

                    /*// Round x2
                    for (int i = 0; i < dataValues.Length; i++)
                        roundConfigs.roundFPS.Add(float.Parse(dataValues[i]));*/
                    break;
                }
                index++;
            }
        }
        else
        {
            //Practice round single for FT study
            /*roundConfigs.roundFPS.Add(500);
            roundConfigs.spikeMagnitude.Add(0);
            roundConfigs.onAimSpikeEnabled.Add(false);
            roundConfigs.onReloadSpikeEnabled.Add(false);
            roundConfigs.onMouseSpikeEnabled.Add(false);
            roundConfigs.onEnemySpawnSpikeEnabled.Add(false);*/

            ReadFTStudyCSV();
        }
    }

    // Primary Config
    public void ReadFTStudyCSV()
    {
        string line = null;
        StreamReader strReader = new StreamReader("Data\\Configs\\LatinMap.csv");
        bool EOF = false;
        List<string> latinMap = new List<string>();

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
                latinMap.Add(line);
                //Debug.Log(strReader.ReadLine());
            }
        }

        /* for (int i = 0; i < latinMap.Count; i++)
             Debug.Log("latmap::: " + i +"::: "+latinMap[i]);*/

        line = null;
        strReader = new StreamReader("Data\\Configs\\SessionID.csv");
        EOF = false;

        sessionID = -1;

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
                sessionID = int.Parse(dataValues[0]);
            }
        }

        line = null;
        strReader = new StreamReader("Data\\Configs\\RoundConfig.csv");
        EOF = false;
        roundConfigs.Clear();
        latinRow = ((sessionID - 1) % latinSquareRowNumber) +1;

        Debug.Log("LATIN ROW NUMBER: " + latinRow);

        int index = 1;

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
                var configVals = line.Split(',');
                Debug.Log("CONF:  " + line);
                if (index == latinRow)

                {
                    for (int i = 0; i < configVals.Length; i++)
                    {
                        string config = latinMap[int.Parse(configVals[i]) - 1];
                        Debug.Log("AC CONFIG:: " + config + "index ::: " + int.Parse(configVals[i]));
                        var dataValues = config.Split(',');



                        Debug.Log(dataValues[1]);

                        roundConfigs.Add(new RoundConfig {
                            roundFPS = float.Parse(dataValues[0]),
                            spikeMagnitude = float.Parse(dataValues[1]),
                            onAimSpikeEnabled = bool.Parse(dataValues[2]),
                            onReloadSpikeEnabled = bool.Parse(dataValues[3]),
                            onMouseSpikeEnabled = bool.Parse(dataValues[4]),
                            onEnemySpawnSpikeEnabled = bool.Parse(dataValues[5]),
                        });
                    }
                }
            }
            index++;
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
        
        Application.targetFrameRate = Mathf.RoundToInt(currentRoundConfig.roundFPS);

        playerController.isAimSpikeEnabled = currentRoundConfig.onAimSpikeEnabled;
        playerController.isReloadSpikeEnabled = currentRoundConfig.onReloadSpikeEnabled;
        playerController.isMouseMovementSpikeEnabled = currentRoundConfig.onMouseSpikeEnabled;
        playerController.isEnemySpawnSpikeEnabled = currentRoundConfig.onEnemySpawnSpikeEnabled;

        gameManager.delayDuration = currentRoundConfig.spikeMagnitude;

        roundFrameCount = 0;
        frametimeCumulativeRound = 0;

    }

    public void LogRoundData()
    {
        PlayerStats stats = playerController.gameObject.GetComponent<PlayerStats>();

        TextWriter textWriter = null;
        filenamePerRound = "Data\\Logs\\RoundData_" + fileNameSuffix + "_" + sessionID + "_" + ".csv";

        while (textWriter == null)
            textWriter = System.IO.File.AppendText(filenamePerRound);

        float accuracy = 0;
        if (playerController.shotsFiredPerRound > 0)
        {
            accuracy = (float)playerController.shotsHitPerRound / (float)playerController.shotsFiredPerRound;
        }

        float degXTargetAvg = (float)playerController.degreeToTargetXCumulative / (float)playerController.roundKills;
        float degXShootAvg = (float)playerController.degreeToShootXCumulative / (float)playerController.roundKills;
        float enemySizeOnSpawnAvg = (float)playerController.enemySizeCumulative / (float)playerController.roundKills;

        float timeToTargetAvg = (float)playerController.timeToTargetEnemyCumulative / (float)playerController.roundKills;
        float timeToHitAvg = (float)playerController.timeToHitEnemyCumulative / (float)playerController.roundKills;
        float timeToKillAvg = (float)playerController.timeToKillEnemyCumulative / (float)playerController.roundKills;
        double avgspikeDurationCumulative = 0;
        if (playerController.perRoundAimSpikeCount + playerController.perRoundReloadSpikeCount + playerController.perRoundMouseMovementSpikeCount > 0)
            avgspikeDurationCumulative = (float)playerController.spikeDurationCumulative / (float)(playerController.perRoundAimSpikeCount + playerController.perRoundReloadSpikeCount + playerController.perRoundMouseMovementSpikeCount);
        double avgFT = frametimeCumulativeRound / roundFrameCount;
        double avgFPS = 1 / avgFT;

        string roundLogLine =
           $"{sessionID}," +
           $"{latinRow}," +
           $"{currentRoundNumber}," +
           $"{sessionStartTime}," +
           $"{DateTime.Now}," +
           $"{currentRoundConfig.roundFPS}," +
           $"{currentRoundConfig.spikeMagnitude}," +
           $"{currentRoundConfig.onAimSpikeEnabled}," +
           $"{currentRoundConfig.onEnemySpawnSpikeEnabled}," +
           $"{currentRoundConfig.onMouseSpikeEnabled}," +
           $"{currentRoundConfig.onReloadSpikeEnabled}," +
           $"{indexArray[currentRoundNumber - 1]}," +
           $"{playerController.score}," +
           $"{playerController.shotsFiredPerRound}," +
           $"{playerController.shotsHitPerRound}," +
           $"{playerController.headshotsHitPerRound}," +
           $"{playerController.realoadCountPerRound}," +
           $"{playerController.tacticalReloadCountPerRound}," +
           $"{accuracy}," +
           $"{playerController.roundKills}," +
           $"{playerController.roundDeaths}," +
           $"{playerController.distanceTravelledPerRound}," +
           $"{playerController.delXCumilative}," +
           $"{playerController.delYCumilative}," +
           $"{(playerController.delXCumilative + playerController.delYCumilative)}," +
           $"{frametimeCumulativeRound}," +
           $"{roundFrameCount}," +
           $"{avgFT}," +
           $"{avgFPS}," +
           $"{playerController.perRoundAimSpikeCount}," +
           $"{playerController.perRoundReloadSpikeCount}," +
           $"{playerController.perRoundMouseMovementSpikeCount}," +
           $"{playerController.spikeDurationCumulative}," +
           $"{avgspikeDurationCumulative}," +
           $"{playerController.perRoundEnemySpawnSpikeCount}," +
           $"{playerController.degreeToShootXCumulative}," +
           $"{playerController.degreeToTargetXCumulative}," +
           $"{playerController.minAnlgeToEnemyCumulative}," +
           $"{playerController.enemySizeCumulative}," +
           $"{playerController.timeToTargetEnemyCumulative}," +
           $"{playerController.timeToHitEnemyCumulative}," +
           $"{playerController.timeToKillEnemyCumulative}," +
           $"{degXShootAvg}," +
           $"{degXTargetAvg}," +
           $"{enemySizeOnSpawnAvg}," +
           $"{playerController.aimDurationPerRound}," +
           $"{playerController.isFiringDurationPerRound}," +
           $"{qoeValue}," +
           $"{acceptabilityValue.ToString()}";
        textWriter.WriteLine(roundLogLine);
        textWriter.Close();
    }

    public void LogPlayerData()
    {
        PlayerStats stats = playerController.gameObject.GetComponent<PlayerStats>();

        TextWriter textWriter = null;
        filenamePerRound = "Data\\Logs\\PlayerData_" + fileNameSuffix + "_" + sessionID + "_" + ".csv";
        while (textWriter == null)
            textWriter = System.IO.File.AppendText(filenamePerRound);

        for (int i = 0; i < playerController.playerTickLog.Count; i++)
        {
            var tickLogEntry = playerController.playerTickLog[i];

            string tickLogLine =
               $"{sessionID}," +
               $"{latinRow}," +
               $"{currentRoundNumber}," +
               $"{currentRoundConfig.roundFPS}," +
               $"{currentRoundConfig.spikeMagnitude}," +
               $"{currentRoundConfig.onAimSpikeEnabled}," +
               $"{currentRoundConfig.onEnemySpawnSpikeEnabled}," +
               $"{currentRoundConfig.onMouseSpikeEnabled}," +
               $"{currentRoundConfig.onReloadSpikeEnabled}," +
               $"{indexArray[currentRoundNumber - 1]}," +
               $"{tickLogEntry.roundTimer}," +
               $"{tickLogEntry.time}," +
               $"{tickLogEntry.mouseX}," +
               $"{tickLogEntry.mouseY}," +
               $"{tickLogEntry.playerX}," +
               $"{tickLogEntry.playerY}," +
               $"{tickLogEntry.playerZ}," +
               $"{tickLogEntry.scorePerSec}," +
               $"{tickLogEntry.playerRot}," +
               $"{tickLogEntry.enemyPos}," +
               $"{tickLogEntry.isADS}," +
               $"{tickLogEntry.frameTimeMS}";

            textWriter.WriteLine(tickLogLine);
        }
        textWriter.Close();

    }
}

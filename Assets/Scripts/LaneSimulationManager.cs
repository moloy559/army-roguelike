using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class LaneSimulationManager : MonoBehaviour
{

    public TextAsset matchupsTextFile;

    public List<SimulationMatch> simulations;

    public List<SimulatorLane> lanes;

    public float gameSpeedSimulations = 20f;

    public string outputPath = "Data/results.csv";

    private string finalPath;

    [ReadOnly]
    public int largestTeamSize = 0;

    TextWriter tw;
    private int matchIndex = 0;
    
    [ReadOnly]
    public int matchesDone = 0;

    bool runningMatches = false;

    float startTime;

    [Button]
    public void GenerateSimulations()
    {
        simulations = new();
        simulations = GetSimulationsFromTable();
    }

    [Button]
    public void StartSimulations()
    {
        if (runningMatches) return;
        if (simulations.Count == 0) return; 

        matchIndex = 0;
        matchesDone = 0;

        finalPath = Application.dataPath + "/" + outputPath;

        tw = new StreamWriter(finalPath, false);
        tw.WriteLine(WriteHeaders());
        tw.Close();


        foreach (SimulatorLane lane in lanes) TakeNextFight(lane);

        Time.timeScale = gameSpeedSimulations;
        runningMatches = true;
        startTime = Time.realtimeSinceStartup;
    }

    private string WriteHeaders()
    {
        string header = "";
        header += "Match Up ID,";
        header += "Match Result,";
        header += "Total Army Health Remaining,";

        for(int i = 1; i <= largestTeamSize; i++)
        {
            header += "Surviving Unit " + i + " Name,";
            header += "Surviving Unit " + i + " Count";
        }




        return header;
    }

    public void TakeSimulationResult(SimulatorLane lane, SimulationMatch simMatch)
    {
        tw = new StreamWriter(finalPath, true);
        tw.WriteLine(WriteResult(simMatch));
        tw.Close();

        TakeNextFight(lane);
        matchesDone++;

        if(matchesDone == simulations.Count)
        {
            float elapsed = Time.realtimeSinceStartup - startTime;

            Debug.Log($"Simulation took {elapsed} seconds");
            runningMatches = false;
        }
    }

    private string WriteResult(SimulationMatch match)
    {
        string result = "";
        result += match.matchUpId + ",";

        string winnerText = match.playerWon ? "Player" : "Enemy";
        result += winnerText + ",";

        result += match.totalHealth.ToString("F2") + ",";

        for (int i = 1; i <= largestTeamSize; i++)
        {
            if(i <= match.survivingUnits.Count)
            {
                result += match.survivingUnits.ElementAt(i-1).Key + ",";
                result += match.survivingUnits.ElementAt(i-1).Value + ",";
            }
            else
            {
                // Leave blank if no surviving units
                result += ",,";
            }
                
        }


        return result;
    }


    public void TakeNextFight(SimulatorLane simulatorLane)
    {
        if(matchIndex < simulations.Count)
        {
            simulatorLane.StartSimulation(simulations[matchIndex], TakeSimulationResult);
            matchIndex++;
        }
    }


    private List<SimulationMatch> GetSimulationsFromTable()
    {
        List<SimulationMatch> simData = new();

        List<string[]> data = GameData.GetTableData(matchupsTextFile);
        Dictionary<string, int> headers = GameData.GetHeaders(matchupsTextFile);

        int playerUnitTypeMax = 0;
        int enemyUnitTypeMax = 0;

        while (true)
        {
            if (headers.ContainsKey("Player Unit " + (playerUnitTypeMax + 1))){
                playerUnitTypeMax++;
            } else break;
        }

        while (true)
        {
            if (headers.ContainsKey("Enemy Unit " + (enemyUnitTypeMax + 1)))
            {
                enemyUnitTypeMax++;
            }
            else break;
        }

        largestTeamSize = playerUnitTypeMax > enemyUnitTypeMax ? playerUnitTypeMax : enemyUnitTypeMax;


        int tableSize = GameData.GetTableSize(data, headers.Count);

        for (int i = 0; i < tableSize; i++)
        {
            SimulationMatch simMatch = new()
            {
                matchUpId = int.Parse(GameData.GetValue(data, headers, i, "Match Up ID")),
                playerWon = false,
                survivingUnits = new()

            };

            simMatch.playerArmy = new();

            for (int iPlayerUnits = 1; iPlayerUnits <= playerUnitTypeMax; iPlayerUnits++)
            {
                string unitName = GameData.GetValue(data, headers, i, "Player Unit " + iPlayerUnits);
                if (GameManager.Instance.unitData.ContainsKey(unitName))
                {
                    simMatch.playerArmy.Add(GameManager.Instance.unitData[unitName], 
                        int.Parse(GameData.GetValue(data, headers, i, "Player Unit " + iPlayerUnits + " Count")));
                }
            }

            simMatch.enemyArmy = new();

            for (int iEnemyUnits = 1; iEnemyUnits <= enemyUnitTypeMax; iEnemyUnits++)
            {
                string unitName = GameData.GetValue(data, headers, i, "Enemy Unit " + iEnemyUnits);
                if (GameManager.Instance.unitData.ContainsKey(unitName))
                {
                    simMatch.enemyArmy.Add(GameManager.Instance.unitData[unitName],
                        int.Parse(GameData.GetValue(data, headers, i, "Enemy Unit " + iEnemyUnits + " Count")));
                }
            }


            simData.Add(simMatch);
        }

        

        return simData;
    }
}


[System.Serializable]
public class SimulationMatch
{
    public int matchUpId;
    public Dictionary<UnitData, int> playerArmy;
    public Dictionary<UnitData, int> enemyArmy;


    public SerializedDictionary<string, int> survivingUnits;
    public float totalHealth;
    public bool playerWon;

}
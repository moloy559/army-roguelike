using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimulatorLane : Lane
{
    Action<SimulatorLane, SimulationMatch> callbackSimulationComplete;
    public float unitSpawnRadius;

    private SimulationMatch simulationMatch;
    private bool doingSimulation = false;

    protected override void Start()
    {
        
    }

    public void StartSimulation(SimulationMatch match, Action<SimulatorLane, SimulationMatch> callback)
    {
        callbackSimulationComplete = callback;
        simulationMatch = match;

        ClearArmy(playerArmy);

        ClearArmy(enemyArmy);

        SpawnArmy(match.playerArmy, unitSpawnPoint, true, playerArmy);

        SpawnArmy(match.enemyArmy, enemyUnitSpawnPoint, false, enemyArmy);

        doingSimulation = true;
    }


   protected override void Update()
    {
        if (!doingSimulation) return;
        
        base.Update();

        if (playerArmy.Count == 0) 
        {
            // Enemy Wins
            EndSimulation(false);
        }

        else if (enemyArmy.Count == 0)
        {
            // Player Wins
            EndSimulation(true);
        }
    }

    protected virtual void EndSimulation(bool playerWon)
    {
        simulationMatch.playerWon = playerWon;
        SerializedDictionary<string, int> leftOverUnits = new();

        if (playerWon) 
        {
            GetSurviving(playerArmy);
        }else
        {
            GetSurviving(enemyArmy);
        }
        
        simulationMatch.survivingUnits = leftOverUnits;

        doingSimulation = false;

        ClearArmy(playerArmy);

        ClearArmy(enemyArmy);

        callbackSimulationComplete.Invoke(this, simulationMatch);


        void GetSurviving(List<ArmyUnit> units)
        {
            foreach (ArmyUnit unit in units)
            {
                simulationMatch.totalHealth += unit.CurrentHealth;

                if (!leftOverUnits.ContainsKey(unit.GetUnitData.name))
                {
                    leftOverUnits.Add(unit.GetUnitData.name, 0);
                }

                leftOverUnits[unit.GetUnitData.name]++;
            }
        }
    }

    protected override void UpdateArmy(List<ArmyUnit> army, float deltaTime)
    {
        for (int i = army.Count - 1; i >= 0; i--)
        {
            ArmyUnit unit = army[i];

            if (unit == null || !unit.gameObject.activeSelf)
                continue;

            unit.HandleCombat(deltaTime);
        }
    }

    protected void SpawnArmy(Dictionary<UnitData, int> army, Transform spawnPoint, bool playerControlled, List<ArmyUnit> units)
    {


        foreach (UnitData key in army.Keys)
        {
            for (int i = 0; i < army[key]; i++)
            {
                Vector2 randomOffset = Random.insideUnitCircle * unitSpawnRadius;
                ArmyUnit aUnit = UnitPool.Instance.Get();
                aUnit.Fill(key, this, playerControlled, (Vector2)spawnPoint.position + randomOffset);

                units.Add(aUnit);
            }
        }
    }
}



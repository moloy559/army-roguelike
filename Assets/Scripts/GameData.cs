using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName ="GameData",menuName = "GameData/GameData", order = 1)] [ExecuteInEditMode]
public class GameData : ScriptableObject
{
    [Header("Generated Data")]
    public List<UnitData> units;
    public List<StructureData> structures;
    public List<ResourceData> resources;
    public List<ArmySetData> armySets;

    [Header("Files For Generation")]
    public int unitTableColumnCount;
    public TextAsset unitTable;

    public int structureTableColumnCount;
    public TextAsset structureTable;

    public int resourceTableColumnCount;
    public TextAsset resourceTable;

    public int armySetTableColumnCount;
    public TextAsset armySetTable;

    private string[] GetTableData(TextAsset table)
    {
        return table.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
    }

    private int GetEntryIndex(int row, int column, int columnCount)
    {
        return (columnCount * (row + 1)) + column;
    }

    private int GetTableSize(string[] data, int columnCount)
    {
        return data.Length / columnCount - 1;
    }

    private List<UnitData> GetUnitsFromTable()
    {
        List<UnitData> unitsTable = new();
        string[] data = GetTableData(unitTable);

        for (int i = 0; i < GetTableSize(data, unitTableColumnCount); i++)
        {
            unitsTable.Add(new UnitData()
            {
                name = data[GetEntryIndex(i, 1, unitTableColumnCount)],
                sprite = Resources.Load<Sprite>("sprites/" + data[GetEntryIndex(i, 4, unitTableColumnCount)]),
                maxHealth = float.Parse(data[GetEntryIndex(i, 8, unitTableColumnCount)]),
                attackDamage = float.Parse(data[GetEntryIndex(i, 9, unitTableColumnCount)]),
                attackSpeed = float.Parse(data[GetEntryIndex(i, 10, unitTableColumnCount)]),
                attackRange = float.Parse(data[GetEntryIndex(i, 11, unitTableColumnCount)]),
                moveSpeed = float.Parse(data[GetEntryIndex(i, 12, unitTableColumnCount)])
            });
        }

        return unitsTable;
    }

    private List<StructureData> GetStructuresFromTable()
    {
        List<StructureData> structuresData = new();
        string[] data = GetTableData(structureTable);

        for (int i = 0; i < GetTableSize(data, structureTableColumnCount); i++)
        {
            structuresData.Add(new StructureData()
            {
                name = data[GetEntryIndex(i, 1, structureTableColumnCount)],
                sprite = Resources.Load<Sprite>("sprites/" + data[GetEntryIndex(i, 4, structureTableColumnCount)]),
                goldCost = int.Parse(data[GetEntryIndex(i, 7, structureTableColumnCount)]),

                inputResource = new ResourceSet()
                {
                    resourceName = data[GetEntryIndex(i, 8, structureTableColumnCount)],
                    amount = int.Parse(data[GetEntryIndex(i, 9, structureTableColumnCount)])
                },

                outputResource = new ResourceSet()
                {
                    resourceName = data[GetEntryIndex(i, 12, structureTableColumnCount)],
                    amount = int.Parse(data[GetEntryIndex(i, 13, structureTableColumnCount)])
                }
            });
        }

        return structuresData;
    }

    private List<ResourceData> GetResourcesFromTable()
    {
        List<ResourceData> resourcesData = new();
        string[] data = GetTableData(resourceTable);

        for (int i = 0; i < GetTableSize(data, resourceTableColumnCount); i++)
        {
            resourcesData.Add(new ResourceData()
            {
                name = data[GetEntryIndex(i, 0, resourceTableColumnCount)],
                sprite = Resources.Load<Sprite>(
                    "sprites/" + data[GetEntryIndex(i, 1, resourceTableColumnCount)].Trim('\r', '\n'))
            });
        }

        return resourcesData;
    }

    private List<ArmySetData> GetArmySetsFromTable()
    {
        List<ArmySetData> armySetsData = new();
        string[] data = GetTableData(armySetTable);

        for (int i = 0; i < GetTableSize(data, armySetTableColumnCount); i++)
        {
            armySetsData.Add(new ArmySetData()
            {
                round = int.Parse(data[GetEntryIndex(i, 1, armySetTableColumnCount)]),
                army = MakeArmy(i)
            });
        }

        SerializedDictionary<string, int> MakeArmy(int index)
        {
            SerializedDictionary<string, int> armySet = new SerializedDictionary<string, int>();
            armySet.Add(
                data[GetEntryIndex(index, 2, armySetTableColumnCount)],
                int.Parse(data[GetEntryIndex(index, 3, armySetTableColumnCount)])
                );

            int army2Count = int.Parse(data[GetEntryIndex(index, 5, armySetTableColumnCount)]);

            if (army2Count > 0) 
            {
                armySet.Add(
                    data[GetEntryIndex(index, 4, armySetTableColumnCount)],
                    army2Count
                );
            }
            return armySet;
        }
        return armySetsData;
    }

    [Button]
    public void GenerateData()
    {
        units = new List<UnitData>();
        foreach (UnitData unitData in GetUnitsFromTable()) 
        { 
            units.Add(unitData);
        }
        
        structures = new List<StructureData>();
        foreach (StructureData structureData in GetStructuresFromTable())
        {
            structures.Add(structureData);
        }

        resources = new List<ResourceData>();
        foreach (ResourceData resourceData in GetResourcesFromTable())
        {
            resources.Add(resourceData);
        }

        armySets = new List<ArmySetData>();
        foreach (ArmySetData armySetData in GetArmySetsFromTable())
        {
            armySets.Add(armySetData);
        }

    }
}

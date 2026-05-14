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
    public List<ShopGenerationData> shopGenerations;

    [Header("Files For Generation")]
    //public int unitTableColumnCount;
    public TextAsset unitTable;

    public TextAsset structureTable;

    public TextAsset resourceTable;

    public TextAsset armySetTable;

    public TextAsset shopGenerationTable;

    private Dictionary<string, int> GetHeaders(TextAsset table)
    {
        string[] headers = table.text.Split('\n')[0].Split(',');

        Dictionary<string, int> map = new();

        for (int i = 0; i < headers.Length; i++)
            map.Add(headers[i].Trim(), i);

        return map;
    }

    private string[] GetTableData(TextAsset table)
    {
        return table.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
    }

    private string GetValue(string[] data, Dictionary<string, int> headers, int row, string columnName)
    {
        int columnCount = headers.Count;
        int column = headers[columnName];

        return data[(columnCount * (row + 1)) + column].Trim('\r', '\n');
    }

    private int GetTableSize(string[] data, int columnCount)
    {
        return data.Length / columnCount - 1;
    }

    private List<UnitData> GetUnitsFromTable()
    {
        List<UnitData> units = new();

        string[] data = GetTableData(unitTable);
        Dictionary<string, int> headers = GetHeaders(unitTable);

        int tableSize = data.Length / headers.Count - 1;

        for (int i = 0; i < tableSize; i++)
        {
            units.Add(new UnitData()
            {
                name = GetValue(data, headers, i, "Name"),
                sprite = Resources.Load<Sprite>("sprites/" + GetValue(data, headers, i, "Sprite")),
                maxHealth = float.Parse(GetValue(data, headers, i, "Health")),
                attackDamage = float.Parse(GetValue(data, headers, i, "Attack")),
                attackSpeed = float.Parse(GetValue(data, headers, i, "Attack Speed")),
                attackRange = float.Parse(GetValue(data, headers, i, "Attack Range")),
                moveSpeed = float.Parse(GetValue(data, headers, i, "Move Speed"))
            });
        }

        return units;
    }

    private List<StructureData> GetStructuresFromTable()
    {
        List<StructureData> structuresData = new();

        string[] data = GetTableData(structureTable);
        Dictionary<string, int> headers = GetHeaders(structureTable);

        int tableSize = GetTableSize(data, headers.Count);

        for (int i = 0; i < tableSize; i++)
        {
            structuresData.Add(new StructureData()
            {
                name = GetValue(data, headers, i, "Name"),

                sprite = Resources.Load<Sprite>(
                    "sprites/" + GetValue(data, headers, i, "Sprite")),

                rarity = Enum.Parse<Rarity>(GetValue(data, headers, i, "Rarity")),

                goldCost = int.Parse(
                    GetValue(data, headers, i, "Gold Cost")),

                inputResource = new ResourceSet()
                {
                    resourceName = GetValue(data, headers, i, "Resource 1 In"),
                    amount = int.Parse(GetValue(data, headers, i, "Input 1 Value"))
                },

                outputResource = new ResourceSet()
                {
                    resourceName = GetValue(data, headers, i, "Resource Out"),
                    amount = int.Parse(GetValue(data, headers, i, "Output Value"))
                }
            });
        }

        return structuresData;
    }

    private List<ResourceData> GetResourcesFromTable()
    {
        List<ResourceData> resourcesData = new();

        string[] data = GetTableData(resourceTable);
        Dictionary<string, int> headers = GetHeaders(resourceTable);

        int tableSize = GetTableSize(data, headers.Count);

        for (int i = 0; i < tableSize; i++)
        {
            resourcesData.Add(new ResourceData()
            {
                name = GetValue(data, headers, i, "Name"),

                sprite = Resources.Load<Sprite>(
                    "sprites/" + GetValue(data, headers, i, "Sprite"))
            });
        }

        return resourcesData;
    }

    private List<ArmySetData> GetArmySetsFromTable()
    {
        List<ArmySetData> armySetsData = new();

        string[] data = GetTableData(armySetTable);
        Dictionary<string, int> headers = GetHeaders(armySetTable);

        int tableSize = GetTableSize(data, headers.Count);

        for (int i = 0; i < tableSize; i++)
        {
            armySetsData.Add(new ArmySetData()
            {
                round = int.Parse(GetValue(data, headers, i, "Round")),
                army = MakeArmy(i)
            });
        }

        return armySetsData;

        SerializedDictionary<string, int> MakeArmy(int index)
        {
            SerializedDictionary<string, int> armySet = new();

            armySet.Add(
                GetValue(data, headers, index, "Unit 1"),
                int.Parse(GetValue(data, headers, index, "Unit 1 Count"))
            );

            int army2Count = int.Parse(
                GetValue(data, headers, index, "Unit 2 Count"));

            if (army2Count > 0)
            {
                armySet.Add(
                    GetValue(data, headers, index, "Unit 2"),
                    army2Count
                );
            }

            return armySet;
        }
    }

    private List<ShopGenerationData> GetShopGenerationFromTable()
    {
        List<ShopGenerationData> resourcesData = new();

        string[] data = GetTableData(shopGenerationTable);
        Dictionary<string, int> headers = GetHeaders(shopGenerationTable);

        int tableSize = GetTableSize(data, headers.Count);

        for (int i = 0; i < tableSize; i++)
        {
            resourcesData.Add(new ShopGenerationData()
            {
                round = int.Parse(GetValue(data, headers, i, "Round")),
                commonWeight = float.Parse(GetValue(data, headers, i, "Common Weight")),
                uncommonWeight = float.Parse(GetValue(data, headers, i, "Uncommon Weight")),
                rareWeight = float.Parse(GetValue(data, headers, i, "Rare Weight")),
                forcedUncommon = (int.Parse(GetValue(data, headers, i, "Forced Uncommon")) == 1),
                forcedRare = (int.Parse(GetValue(data, headers, i, "Forced Rare")) == 1),
            });
        }

        return resourcesData;
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

        shopGenerations = new List<ShopGenerationData>();
        foreach (ShopGenerationData shopGenerationData in GetShopGenerationFromTable())
        {
            shopGenerations.Add(shopGenerationData);
        }
    }
}

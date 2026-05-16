using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
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

    public static Dictionary<string, int> GetHeaders(TextAsset table)
    {
        List<string[]> rows = ParseCSV(table.text);

        Dictionary<string, int> map = new();

        for (int i = 0; i < rows[0].Length; i++)
            map.Add(rows[0][i].Trim(), i);

        return map;
    }

    public static List<string[]> GetTableData(TextAsset table)
    {
        return ParseCSV(table.text);
    }

    public static List<string[]> ParseCSV(string csv)
    {
        List<string[]> rows = new();

        List<string> currentRow = new();
        System.Text.StringBuilder currentCell = new();

        bool inQuotes = false;

        foreach (char c in csv)
        {
            switch (c)
            {
                case '"':
                    inQuotes = !inQuotes;
                    break;

                case ',' when !inQuotes:
                    currentRow.Add(currentCell.ToString());
                    currentCell.Clear();
                    break;

                case '\n' when !inQuotes:
                    currentRow.Add(currentCell.ToString().Trim('\r'));

                    rows.Add(currentRow.ToArray());

                    currentRow = new();
                    currentCell.Clear();
                    break;

                default:
                    currentCell.Append(c);
                    break;
            }
        }

        // Final cell
        currentRow.Add(currentCell.ToString().Trim('\r'));

        // Only add if not empty
        if (currentRow.Count > 0 && !(currentRow.Count == 1 && string.IsNullOrWhiteSpace(currentRow[0])))
            rows.Add(currentRow.ToArray());

        return rows;
    }

    public static string GetValue(List<string[]> data, Dictionary<string, int> headers, int row, string columnName)
    {
        return data[row + 1][headers[columnName]]
        .Trim('\r', '\n');
    }

    public static int GetTableSize(List<string[]> data, int columnCount)
    {
        return data.Count - 1;
    }

    public static Dictionary<string, string> ParseExtraData(string data)
    {
        Dictionary<string, string> result = new();

        foreach (string pair in data.Replace("\r", "").Replace("\n", "").Split(';'))
        {
            if (string.IsNullOrWhiteSpace(pair))
                continue;

            string[] split = pair.Split('=');

            if(split.Length > 0) result.Add(split[0], split[1]);
        }

        return result;
    }
    public static ResourceTransaction ParseTransaction(string data)
    {
        ResourceTransaction transaction = new()
        {
            inputResources = new(),
            outputResources = new()
        };

        foreach (string section in data.Replace("\r", "").Replace("\n", "").Split(';'))
        {
            string[] split = section.Split('=');

            string key = split[0];

            string value = split[1];

            if (key == "input")
                transaction.inputResources = ParseResourceList(value);

            else if (key == "output")
                transaction.outputResources = ParseResourceList(value);

            else if (key == "repeats")
                transaction.repeats = int.Parse(value);

            else if (key == "cooldown")
                transaction.cooldown = int.Parse(value);
        }

        return transaction;
    }

    public static List<ResourceSet> ParseResourceList(string data)
    {
        List<ResourceSet> list = new();

        foreach (string resource in data.Split('|'))
        {
            string[] split = resource.Split(':');

            list.Add(new ResourceSet()
            {
                resourceName = split[0],
                amount = int.Parse(split[1])
            });
        }

        return list;
    }

    private List<UnitData> GetUnitsFromTable()
    {
        List<UnitData> units = new();

        List<string[]> data = GetTableData(unitTable);
        Dictionary<string, int> headers = GetHeaders(unitTable);

        int tableSize = GetTableSize(data, headers.Count);

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
                moveSpeed = float.Parse(GetValue(data, headers, i, "Move Speed")),
                attackType = Enum.Parse<AttackType>(GetValue(data, headers, i, "Attack Type")),
                projectileData = ParseProjectile(GetValue(data, headers, i, "Attack Data"))
            });
        }

        return units;
    }


    private ProjectileData ParseProjectile(string data)
    {
        Dictionary<string, string> extraData = ParseExtraData(data);

        ProjectileData projectile = new ProjectileData();

        if (extraData.ContainsKey("projectile-speed"))
        {
            projectile.speed = float.Parse(extraData["projectile-speed"]);
        }

        if (extraData.ContainsKey("projectile-sprite"))
        {
            projectile.sprite = Resources.Load<Sprite>("sprites/" + extraData["projectile-sprite"]);
        }

        return projectile;
    }

    private List<StructureData> GetStructuresFromTable()
    {
        List<StructureData> structuresData = new();

        List<string[]> data = GetTableData(structureTable);
        Dictionary<string, int> headers = GetHeaders(structureTable);

        int tableSize = GetTableSize(data, headers.Count);

        for (int i = 0; i < tableSize; i++)
        {
            StructureData structureData = new()
            {
                name = GetValue(data, headers, i, "Name"),

                sprite = Resources.Load<Sprite>(
                    "sprites/" + GetValue(data, headers, i, "Sprite")),

                rarity = Enum.Parse<Rarity>(GetValue(data, headers, i, "Rarity")),

                goldCost = int.Parse(
                    GetValue(data, headers, i, "Gold Cost"))
            };

            structureData.transactions = new();

            TryAddTransaction(structureData, "Transaction");

            TryAddTransaction(structureData, "Transaction 2");

            TryAddTransaction(structureData, "Transaction 3");


            void TryAddTransaction(StructureData sd, string columnName)
            {
                string transactionString = GetValue(data, headers, i, columnName);
                if(transactionString.Length > 4)
                {
                    sd.transactions.Add(ParseTransaction(transactionString));
                }
            }

            structuresData.Add(structureData);
        }

        return structuresData;
    }

    private List<ResourceData> GetResourcesFromTable()
    {
        List<ResourceData> resourcesData = new();

        List<string[]> data = GetTableData(resourceTable);
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

        List<string[]> data = GetTableData(armySetTable);
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

        List<string[]> data = GetTableData(shopGenerationTable);
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

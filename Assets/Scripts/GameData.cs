using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName ="GameData",menuName = "GameData/GameData", order = 1)] [ExecuteInEditMode]
public class GameData : ScriptableObject
{
    public List<UnitData> units;
    public List<StructureData> structures;
    public List<ResourceData> resources;

    public int unitTableColumnCount;
    public TextAsset unitTable;

    public int structureTableColumnCount;
    public TextAsset structureTable;

    public TextAsset resourceTable;


    private List<UnitData> GetUnitsFromTable()
    {
        List<UnitData> unitsTable = new List<UnitData>();

        //Split up data from csv
        string[] data = unitTable.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        //Divide up data into sets
        int tableSize = data.Length / unitTableColumnCount - 1;

        for (int i = 0; i < tableSize; i++)
        {
            UnitData unitData = new UnitData()
            {
                name = data[entryAt(i, 1)],
                sprite = Resources.Load<Sprite>("sprites/" + data[entryAt(i, 4)]) as Sprite,
                maxHealth = float.Parse(data[entryAt(i, 8)]),
                attackDamage = float.Parse(data[entryAt(i, 9)]),
                attackSpeed = float.Parse(data[entryAt(i, 10)]),
                attackRange = float.Parse(data[entryAt(i, 11)]),
                moveSpeed = float.Parse(data[entryAt(i, 12)])
            };
            unitsTable.Add(unitData);
        }

        return unitsTable;
        int entryAt(int row, int column)
        {
            return (unitTableColumnCount * (row + 1)) + column;
        }
    }

    private List<StructureData> GetStructuresFromTable()
    {
        List<StructureData> structuresData = new List<StructureData>();

        //Split up data from csv
        string[] data = structureTable.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        //Divide up data into sets
        int tableSize = data.Length / structureTableColumnCount - 1;

        for (int i = 0; i < tableSize; i++)
        {
            StructureData structureData = new StructureData()
            {
                name = data[entryAt(i, 1)],
                sprite = Resources.Load<Sprite>("sprites/" + data[entryAt(i, 4)]) as Sprite,
                goldCost = int.Parse(data[entryAt(i, 7)]),
                inputResource = new ResourceSet(){ 
                    resourceName = data[entryAt(i,8)],
                    amount = int.Parse(data[entryAt(i,9)]),
                    },
                outputResource = new ResourceSet()
                {
                    resourceName = data[entryAt(i,12)],
                    amount = int.Parse(data[entryAt(i,13)]),
                }
            };
            structuresData.Add(structureData);
        }

        return structuresData;
        int entryAt(int row, int column)
        {
            return (structureTableColumnCount * (row + 1)) + column;
        }
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
    }
}

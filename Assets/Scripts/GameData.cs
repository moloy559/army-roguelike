using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="GameData",menuName = "GameData/GameData", order = 1)]
public class GameData : ScriptableObject
{
    public List<UnitData> units;
    public List<StructureData> structures;
    public List<ResourceData> resources;

    public TextAsset unitTable;
    public TextAsset structureTable;
    public TextAsset resourceTable;


    public List<UnitData> GetUnitsFromTable()
    {
        List<UnitData> unitsTable = new List<UnitData>();

        //Split up data from csv
        string[] data = unitTable.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        //Divide up data into sets
        int tableSize = data.Length / NumberOfColumns - 1;
        myUnitDataList.unitData = new UnitData[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            myUnitDataList.unitData[i] = new UnitData();
            myUnitDataList.unitData[i].name = data[NumberOfColumns * (i + 1)];
            myUnitDataList.unitData[i].maxHealth = float.Parse(data[NumberOfColumns * (i + 1) + 1]);
            myUnitDataList.unitData[i].attackDamage = float.Parse(data[NumberOfColumns * (i + 1) + 2]);
            myUnitDataList.unitData[i].attackRange = float.Parse(data[NumberOfColumns * (i + 1) + 3]);
            myUnitDataList.unitData[i].attackSpeed = float.Parse(data[NumberOfColumns * (i + 1) + 4]);
            myUnitDataList.unitData[i].moveSpeed = float.Parse(data[NumberOfColumns * (i + 1) + 5]);
        }
    }
}

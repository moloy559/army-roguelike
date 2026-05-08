using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "TempData", menuName = "GameData/ArmySet", order = 2)]
public class TempArmySet : ScriptableObject
{
    [SerializedDictionary("Unit Name", "Amount")]
    public SerializedDictionary<string, int> army;

}

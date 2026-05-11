using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="GameData",menuName = "GameData/GameData", order = 1)]
public class GameData : ScriptableObject
{
    public List<UnitData> units;
    public List<StructureData> structures;
    public List<ResourceData> resources;

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="TempData",menuName = "GameData/GameData", order = 1)]
public class TempData : ScriptableObject
{
    public List<UnitData> units;
    public List<StructureData> structures;
    public List<ResourceData> resources;

}

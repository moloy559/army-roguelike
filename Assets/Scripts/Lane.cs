using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public List<Structure> structures;

    private List<UnitData> unitData;


    public List<ArmyUnit> playerArmy;
    public List<ArmyUnit> enemyArmy;
}

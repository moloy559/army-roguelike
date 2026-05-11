using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSim : MonoBehaviour
{
    public GameObject unitPrefab;
    public UnitData unitData1;
    public UnitData unitData2;

    public float unitDist;

    public ArmyUnit unit1;
    public ArmyUnit unit2;

    public bool simOver = false;
    public bool turnedOff = false;

    public void Start()
    {
        Spawn();
    }


    public void Spawn()
    {
        GameObject obj1 = Instantiate(unitPrefab, transform.position, Quaternion.identity);
        unit1 = obj1.GetComponent<ArmyUnit>();
        unit1.playerControlled = true;
        //unit1.GetComponent<ArmyUnit>().Fill(unitData1);

        GameObject obj2 = Instantiate(unitPrefab, transform.position + new Vector3(0,unitDist,0), Quaternion.identity);
        unit2 = obj2.GetComponent<ArmyUnit>();
        unit2.playerControlled = false;
        //unit2.Fill(unitData2);
    }

    private void Update()
    {
        if (simOver)
        {
            if (turnedOff) return;
            if (unit1 != null)
            {
                unit1.pause = true;
            }
            if (unit2 != null)
            {
                unit2.pause = true;
            }
            return;
        }


        if(unit1 == null)
        {
            Debug.Log(unitData2.name + "Won with " + unit2.CurrentHealth);
            simOver = true;
        }

        if (unit2 == null)
        {
            Debug.Log(unitData1.name + "Won with " + unit1.CurrentHealth);
            simOver = true;
        }
    }

}

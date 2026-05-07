using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public bool playerControlled;
    public string nameId;

    private StructureData data;

    [SerializeField]
    private GameObject unitPrefab;

    [SerializeField]
    private Transform unitSpawnPoint;

    void OnEnable()
    {
        GameManager.Instance.structures.Add(this);
    }

    void OnDisable()
    {
        GameManager.Instance.structures.Remove(this);
    }

    public void SetData()
    {
        if (GameManager.Instance.structureData.ContainsKey(nameId))
        {
            data = GameManager.Instance.structureData[nameId];
        }
    }

    public void OnTurnStart()
    {
        if (data == null)
        {
            SetData();
        }
        if (data != null) 
        {
            if (GameManager.Instance.unitData.ContainsKey(data.outputResource.resourceName))
            {
                for (int i = 0; i < data.outputResource.amount; i++) 
                {
                    GameObject obj = Instantiate(unitPrefab, unitSpawnPoint.position, Quaternion.identity);
                    obj.GetComponent<ArmyUnit>().playerControlled = playerControlled;
                    obj.GetComponent<ArmyUnit>().Fill(GameManager.Instance.unitData[data.outputResource.resourceName]);
                }
            }

        }




    }
}
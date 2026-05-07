using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public bool playerControlled;

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

    public void OnTurnStart()
    {
        GameObject obj = Instantiate(unitPrefab, unitSpawnPoint.position, Quaternion.identity);
        obj.GetComponent<ArmyUnit>().playerControlled = playerControlled;
    }
}
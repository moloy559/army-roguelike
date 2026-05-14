using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class UnitPool : MonoBehaviour
{
    public static UnitPool Instance;

    ObjectPool<ArmyUnit> unitPool;
    [SerializeField] private GameObject unitPrefab;

    private void Awake()
    {
        Instance = this;
        unitPool = new ObjectPool<ArmyUnit>(CreateUnit, OnTakeUnit, OnReturnUnit);

    }

    private void Start()
    {
        List<ArmyUnit> prewarm = new List<ArmyUnit>();
        for (int i = 0; i < 500; i++)
        {
            var obj = unitPool.Get();
            prewarm.Add(obj);
        }

        foreach (ArmyUnit item in prewarm)
        {
            unitPool.Release(item);
        }
    }

    ArmyUnit CreateUnit()
    {
        return Instantiate(unitPrefab).GetComponent<ArmyUnit>();
    }

    void OnTakeUnit(ArmyUnit obj)
    {
        obj.gameObject.SetActive(true);
    }

    void OnReturnUnit(ArmyUnit obj)
    {
        obj.gameObject.SetActive(false);
    }

    public void ReleaseFrom(ArmyUnit obj) 
    { 
        unitPool.Release(obj);
    }

    public ArmyUnit Get()
    {
        return unitPool.Get();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

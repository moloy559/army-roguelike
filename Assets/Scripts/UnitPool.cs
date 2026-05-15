using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class UnitPool : MonoBehaviour
{
    public static UnitPool Instance;

    ObjectPool<ArmyUnit> unitPool;
    ObjectPool<Projectile> projectilePool;

    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private GameObject projectilePrefab;

    private void Awake()
    {
        Instance = this;
        unitPool = new ObjectPool<ArmyUnit>(CreateUnit, OnTakeUnit, OnReturnUnit);

        projectilePool = new ObjectPool<Projectile>(CreateProjectile, OnTakeProjectile, OnReturnProjectile);

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

        List<Projectile> prewarm2 = new List<Projectile>();
        for (int i = 0; i < 25; i++)
        {
            var obj = projectilePool.Get();
            prewarm2.Add(obj);
        }

        foreach (Projectile item in prewarm2)
        {
            projectilePool.Release(item);
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


    Projectile CreateProjectile()
    {
        return Instantiate(projectilePrefab).GetComponent<Projectile>();
    }

    void OnTakeProjectile(Projectile obj)
    {
        obj.gameObject.SetActive(true);
    }

    void OnReturnProjectile(Projectile obj)
    {
        obj.gameObject.SetActive(false);
    }

    public void ReleaseFromProjectile(Projectile obj)
    {
        projectilePool.Release(obj);
    }

    public Projectile GetProjectile()
    {
        return projectilePool.Get();
    }

}

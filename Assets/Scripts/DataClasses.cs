using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[System.Serializable]
public class UnitData
{
    public string name;
    public Sprite sprite;
    public float maxHealth;
    public float attackDamage;
    public float attackSpeed;
    public float attackRange;
    public float moveSpeed;
}

[System.Serializable]
public class StructureData
{
    public string name;
    public Sprite sprite;
    public int goldCost;
    public ResourceSet inputResource;
    public ResourceSet outputResource;
}

[System.Serializable]
public struct ResourceSet
{
    public string resourceName;
    public int amount; 
}

[System.Serializable]
public class ResourceData
{
    public string name;
    public Sprite sprite;
}

[System.Serializable]
public class ArmySetData
{
    public int round;

    [SerializedDictionary("Unit Name", "Amount")]
    public SerializedDictionary<string, int> army;
}
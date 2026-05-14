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

public enum Rarity
{
    Common,
    Uncommon,
    Rare
}

[System.Serializable]
public class StructureData
{
    public string name;
    public Rarity rarity;
    public Sprite sprite;
    public int goldCost;
    public ResourceTransaction transaction;
}

[System.Serializable]
public struct ResourceSet
{
    public string resourceName;
    public int amount; 
}

[System.Serializable]
public class ResourceTransaction
{
    public List<ResourceSet> inputResources;
    public List<ResourceSet> outputResources;
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

[System.Serializable]
public class ShopGenerationData
{
    public int round;

    public float commonWeight;
    public float uncommonWeight;
    public float rareWeight;
    public bool forcedUncommon;
    public bool forcedRare;
}

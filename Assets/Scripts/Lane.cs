using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

public class Lane : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyUnitPrefab;
    public TempArmySet enemyArmySet;
    public Transform enemyUnitSpawnPoint;

    [Header("Player Settings")]
    public GameObject unitPrefab;
    public Transform unitSpawnPoint;

    public GameObject structurePrefab;
    public Transform structureSpawnPoint;
    public float structureSpawnRadius = 1f;
    public List<Structure> structures;

    [Header("UI")]
    public RectTransform resourceDisplayHolder;
    public GameObject resourceDisplayPrefab;

    [SerializedDictionary("Resource Name", "Amount")]
    public SerializedDictionary<string, int> resources; 

    private List<Structure> structuresNeedingInput = new List<Structure>();

    private List<ArmyUnit> playerArmy = new List<ArmyUnit>();
    private List<ArmyUnit> enemyArmy = new List<ArmyUnit>();

    private List<LaneResourceDisplay> laneResourceDisplays = new List<LaneResourceDisplay>();

    private void Start()
    {
        
    }

    public void ResourceGeneration()
    {
        GameManager.Instance.gold += resources["gold"];
        int popGrowth = resources["pop-growth"];
        resources["population"] += popGrowth;

        ClearArmy(playerArmy);

        ClearArmy(enemyArmy);

        SpawnArmy(resources, unitPrefab, unitSpawnPoint, true, playerArmy);

        SpawnArmy(enemyArmySet.army, enemyUnitPrefab, enemyUnitSpawnPoint, false, enemyArmy);

        UpdateDisplay();
    }

    public List<ArmyUnit> GetArmy(bool playeControlled)
    {
        return playeControlled ? playerArmy : enemyArmy;
    }

    public void AddStructure(StructureData structureData)
    {
        Vector2 randomOffset = Random.insideUnitCircle * structureSpawnRadius;
        randomOffset.y = randomOffset.y / 5f;

        GameObject obj = Instantiate(structurePrefab, structureSpawnPoint.position + (Vector3)randomOffset, Quaternion.identity);
        Structure structure = obj.GetComponent<Structure>();
        structure.Fill(structureData);
        structures.Add(structure);

        if (structure.CanGiveOutput(resources))
        {
            structure.ModifyResources(resources);
        }
        else
        {
            structuresNeedingInput.Add(structure);
        }

        UpdateDisplay();
    }

    private void ClearArmy(List<ArmyUnit> units)
    {
        for (int i = units.Count - 1; i >= 0; i--)
        {
            if (units[i] != null) Destroy(units[i].gameObject);
        }
        units.Clear();
    }

    private void SpawnArmy(SerializedDictionary<string, int> army, GameObject unitPrefab, Transform spawnPoint, bool playerControlled, List<ArmyUnit> units) 
    {
        foreach (string key in army.Keys)
        {
            if (GameManager.Instance.unitData.ContainsKey(key))
            {
                for (int i = 0; i < army[key]; i++)
                {
                    GameObject obj = Instantiate(unitPrefab, spawnPoint.position, Quaternion.identity);
                    ArmyUnit aUnit = obj.GetComponent<ArmyUnit>();
                    aUnit.playerControlled = playerControlled;
                    aUnit.Fill(GameManager.Instance.unitData[key], this);

                    units.Add(aUnit);
                }
            }
        }
    }

    private void UpdateDisplay()
    {
        for (int i = laneResourceDisplays.Count - 1; i >= 0; i--)
        {
            if (laneResourceDisplays[i] != null) Destroy(laneResourceDisplays[i].gameObject);
        }
        laneResourceDisplays.Clear();

        foreach(KeyValuePair<string, int> resouceToDisplay in resources)
        {
            GameObject obj = Instantiate(resourceDisplayPrefab, resourceDisplayHolder);
            LaneResourceDisplay laneResourceDisplay = obj.GetComponent<LaneResourceDisplay>();
            laneResourceDisplay.Fill(GameManager.Instance.resourceData[resouceToDisplay.Key].sprite, resouceToDisplay.Value);
            laneResourceDisplays.Add(laneResourceDisplay);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(resourceDisplayHolder);
    }
}

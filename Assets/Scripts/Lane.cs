using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

public class Lane : MonoBehaviour
{
    [Header("Enemy Settings")]
    public Transform enemyUnitSpawnPoint;

    [Header("Player Settings")]
    public Transform unitSpawnPoint;
    
    public Spire spire;
    public float spireRegen = 15f;

    public GameObject structurePrefab;
    public Transform structureSpawnPoint;
    public float structureSpawnRadius = 1f;
    public List<Structure> structures;

    [Header("UI")]
    public RectTransform resourceDisplayHolder;
    public GameObject resourceDisplayPrefab;
    public Button purchaseButton;

    public GameObject levelButtonsHolder;

    [SerializedDictionary("Resource Name", "Amount")]
    public SerializedDictionary<string, int> resources; 


    protected List<ArmyUnit> playerArmy = new List<ArmyUnit>();
    protected List<ArmyUnit> enemyArmy = new List<ArmyUnit>();


    private List<ResourceDisplay> laneResourceDisplays = new List<ResourceDisplay>();

    private int repairTimer = 0;
    public int turnsToRepair;

    List<ResourceTransaction> unfulfilled = new();

    protected virtual void Start()
    {
        UpdateDisplay();
        purchaseButton.gameObject.SetActive(false);
        levelButtonsHolder.SetActive(false);
        ShopManager.Instance.onSelectStructure += OnStructureSelected;
        ShopManager.Instance.onShopReset += OnShopReset;

        spire.SetLane(this);
    }

    protected virtual void Update()
    {
        float deltaTime = Time.deltaTime;

        UpdateArmy(playerArmy, deltaTime);
        UpdateArmy(enemyArmy, deltaTime);
    }

    protected virtual void UpdateArmy(List<ArmyUnit> army, float deltaTime)
    {
        for (int i = army.Count - 1; i >= 0; i--)
        {
            ArmyUnit unit = army[i];

            if (unit == null || !unit.gameObject.activeSelf)
                continue;

            if (GameManager.Instance.inCombat)
                unit.HandleCombat(deltaTime);
            else
                unit.HandleWander(deltaTime);
        }
    }


    private void OnDestroy()
    {
        ShopManager.Instance.onSelectStructure -= OnStructureSelected;
        ShopManager.Instance.onShopReset -= OnShopReset;

    }

    public void ResourceGeneration()
    {
        ClearArmy(playerArmy);

        ClearArmy(enemyArmy); 
        if (spire.CurrentHealth <= 0)
        {
            repairTimer++;
            if (repairTimer < turnsToRepair)
            {
                UpdateDisplay();
                return;
            }
            repairTimer = 0;
            spire.TakeDamage(-spireRegen);
            spire.gameObject.SetActive(true);
        }

        levelButtonsHolder.SetActive(false);
        GameManager.Instance.Gold += resources["gold"];
        int popGrowth = resources["pop-growth"];
        resources["population"] += popGrowth;
        if (resources.ContainsKey("gem-income"))
        {
            if (!resources.ContainsKey("gems"))
            {
                resources.Add("gems", 0);
            }
            int gemIncome = resources["gem-income"];
            resources["gems"] += gemIncome;
        }

        foreach (Structure structure in structures) structure.RoundUpdate();
        ResolveTransactions(resources, structures);
        UpdateDisplay();

        ClearArmy(playerArmy);
        playerArmy = new();

        ClearArmy(enemyArmy);
        enemyArmy = new();


        SpawnArmy(resources, unitSpawnPoint, true, playerArmy);
        playerArmy.Add(spire);

        SpawnArmy(GameManager.Instance.GetArmy().army, enemyUnitSpawnPoint, false, enemyArmy);

        float missingHp =  spire.maxHealth - spire.CurrentHealth;
        if (missingHp > spireRegen)
        {
            spire.TakeDamage(-spireRegen);
        }
        else
        {
            spire.TakeDamage(-missingHp);
        }

    }

    public void ResolveTransactions(SerializedDictionary<string, int> resources, List<Structure> structures)
    {
        // Flatten all transactions into one ordered queue.
        // Earlier transactions always have priority.
        List<(Structure structure, ResourceTransaction transaction)> queue = new();

        foreach (var structure in structures)
        {
            foreach (var transaction in structure.OutstandingTransactions)
            {
                queue.Add((structure, transaction));
            }
        }

        bool resolvedSomething;

        do
        {
            resolvedSomething = false;

            // Iterate in strict priority order every pass
            foreach (var entry in queue)
            {
                var transaction = entry.transaction;

                // Skip if already resolved this loop
                if (!entry.structure.OutstandingTransactions.Contains(transaction))
                    continue;

                // Check affordability
                bool canResolve = true;

                foreach (var input in transaction.inputResources)
                {
                    int current = resources.GetValueOrDefault(input.resourceName, 0);

                    if (current < input.amount)
                    {
                        canResolve = false;
                        break;
                    }
                }

                if (!canResolve)
                    continue;

                // Consume inputs
                foreach (var input in transaction.inputResources)
                {
                    resources[input.resourceName] -= input.amount;
                }

                // Produce outputs
                foreach (var output in transaction.outputResources)
                {
                    if (!resources.ContainsKey(output.resourceName))
                        resources[output.resourceName] = 0;

                    resources[output.resourceName] += output.amount;
                }

                // Mark complete
                entry.structure.OutstandingTransactions.Remove(transaction);

                resolvedSomething = true;
            }

        } while (resolvedSomething);

        unfulfilled = new();

        foreach (var structure in structures)
        {
            unfulfilled.AddRange(structure.OutstandingTransactions);
        }
    }

    protected virtual void SpawnArmy(SerializedDictionary<string, int> army, Transform spawnPoint, bool playerControlled, List<ArmyUnit> units)
    {
        foreach (string key in army.Keys)
        {
            if (GameManager.Instance.unitData.ContainsKey(key))
            {
                for (int i = 0; i < army[key]; i++)
                {
                    ArmyUnit aUnit = UnitPool.Instance.Get();
                    aUnit.Fill(GameManager.Instance.unitData[key], this, playerControlled, spawnPoint.position);

                    units.Add(aUnit);
                }
            }
        }
    }

    public void CityLevelUp()
    {
        levelButtonsHolder.SetActive(true);
    }

    public void ButtonAddResource(string resourceName)
    {
        if (resources.ContainsKey(resourceName))
        {
            resources[resourceName] += 1;
        }
        else
        {
            resources.Add(resourceName, 1);
        }
        UpdateDisplay();
        levelButtonsHolder.SetActive(false);
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


        ResolveTransactions(resources, structures);
        UpdateDisplay();
    }

    protected void ClearArmy(List<ArmyUnit> units)
    {
        if(units.Contains(spire)) units.Remove(spire);
        for (int i = units.Count - 1; i >= 0; i--)
        {
            if (units[i] != null) units[i].Die();
        }
        units.Clear();
    }

    public void RemoveFromArmy(ArmyUnit unit)
    {
        if(unit.playerControlled) playerArmy.Remove(unit);
        else enemyArmy.Remove(unit);
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
            ResourceDisplay laneResourceDisplay = obj.GetComponent<ResourceDisplay>();
            laneResourceDisplay.Fill(GameManager.Instance.resourceData[resouceToDisplay.Key].sprite, resouceToDisplay.Value);
            laneResourceDisplays.Add(laneResourceDisplay);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(resourceDisplayHolder);
    }

    private void OnStructureSelected(StructureData structureData)
    {
        purchaseButton.gameObject.SetActive(true);
    }

    private void OnShopReset()
    {
        purchaseButton.gameObject.SetActive(false);
    }

    public void PurchaseFromShop()
    {
        AddStructure(ShopManager.Instance.selectedStructure);
        ShopManager.Instance.PurchaseSelectedItem();
    }

}

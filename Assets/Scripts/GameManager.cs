using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game")]
    public bool inCombat;
    public List<Lane> lanes;

    [Header("UI")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI roundText;
    public GameObject endScreen;

    [Header("Data")]
    public GameData data;
    [SerializeField] private int gold;
    public int Gold { get { return gold; } set { gold = value; UpdateText(); } }
    [SerializeField] private int round;
    public int Round { get { return round; } set { round = value;} }


    public Dictionary<string, UnitData> unitData;
    public Dictionary<string, StructureData> structureData;
    public Dictionary<string, ResourceData> resourceData;



    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        endScreen.SetActive(false);
        //Setting up dictionaries
        #region Set up dictionaries

        unitData = new Dictionary<string, UnitData>();
        foreach(UnitData unitD in data.units)
        {
            unitData.Add(unitD.name, unitD);
        }

        structureData = new Dictionary<string, StructureData>();
        foreach(StructureData structureD in data.structures)
        {
            structureData.Add(structureD.name, structureD);
        }

        resourceData = new Dictionary<string, ResourceData>();
        foreach(ResourceData resourceD in data.resources)
        {
            resourceData.Add(resourceD.name, resourceD);
        }
        foreach (UnitData unitD in data.units)
        {
            if(!resourceData.ContainsKey(unitD.name)) resourceData.Add(unitD.name, new ResourceData() { name = unitD.name, sprite = unitD.sprite });
        }
        #endregion

        UpdateText();
    }

    public ArmySetData GetArmy()
    {
        List<ArmySetData> possibleArmies = new();

        foreach(ArmySetData armySet in data.armySets)
        {
            if (armySet.round == round) possibleArmies.Add(armySet);
        }

        if (possibleArmies.Count == 0) return data.armySets.Last();

        return possibleArmies[Random.Range(0, possibleArmies.Count)];
    }

    public ShopGenerationData GetShopGeneration()
    {
        foreach(ShopGenerationData shopGenerationData in data.shopGenerations)
        {
            if (shopGenerationData.round == round) return shopGenerationData;
        }
        return data.shopGenerations.Last();
    }

    int levelingLane = 0;
    public void TurnStart()
    {
        foreach (Lane lane in lanes) lane.ResourceGeneration();
        round++;

        UpdateText();
        
        lanes[levelingLane].CityLevelUp();

        levelingLane++;
        if(levelingLane >= lanes.Count) levelingLane = 0;
    }

    public void ToggleCombat()
    {
        inCombat = !inCombat;
    }

    public void ToggleShop()
    {
        if (ShopManager.Instance.Open)
        {
            ShopManager.Instance.CloseShop();
        }
        else
        {
            ShopManager.Instance.OpenShop();
        }
 
    }

    public void UpdateText()
    {
        goldText.text = gold + " gold";
        roundText.text = "Round: " + round;
    }

    public void GameOver()
    {
        endScreen.SetActive(true);
    }

    public void Restart()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
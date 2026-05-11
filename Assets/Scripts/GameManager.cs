using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    [Header("Game")]
    public bool inCombat;
    public List<Lane> lanes;

    [Header("UI")]
    public TextMeshProUGUI goldText;
    public TMP_Dropdown laneDropdown;
    public TMP_Dropdown structureDropDown;


    [Header("Data")]
    public GameData data;
    public int gold;

    public Dictionary<string, UnitData> unitData;
    public Dictionary<string, StructureData> structureData;
    public Dictionary<string, ResourceData> resourceData;

    private int selectedLane = 0;
    private int selectedStructure = 0;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
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
            resourceData.Add(unitD.name, new ResourceData() { name = unitD.name, sprite = unitD.sprite});
        }

        structureDropDown.ClearOptions();
        List<string> structureOptions = new List<string>();
        for (int i = 0; i < structureData.Count; i++) 
        {
            structureOptions.Add(structureData.ElementAt(i).Key);
            
        }
        structureDropDown.AddOptions(structureOptions);
        structureDropDown.onValueChanged.AddListener((int val) => { selectedStructure = val; });

        laneDropdown.ClearOptions();
        List<string> laneOptions = new List<string>();

        laneOptions.Add("Left");
        laneOptions.Add("Middle");
        laneOptions.Add("Right");
        laneDropdown.AddOptions(laneOptions);
        laneDropdown.onValueChanged.AddListener((int val) => { selectedLane = val; });

        UpdateText();
    }

    public void AddStructureToLane()
    {
        StructureData structureD = structureData.ElementAt(selectedStructure).Value;
        gold -= structureD.goldCost;
        lanes[selectedLane].AddStructure(structureD);
        UpdateText();
    }

    public void TurnStart()
    {
        foreach (Lane lane in lanes) lane.ResourceGeneration();

        UpdateText();
    }

    public void ToggleCombat()
    {
        inCombat = !inCombat;
    }

    private void UpdateText()
    {
        goldText.text = gold + " gold";
    }

}
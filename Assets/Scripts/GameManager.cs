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
    [SerializeField] private int gold;
    public int Gold { get { return gold; } set { gold = value; UpdateText(); } }

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
            resourceData.Add(unitD.name, new ResourceData() { name = unitD.name, sprite = unitD.sprite});
        }
        #endregion

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
    }

}
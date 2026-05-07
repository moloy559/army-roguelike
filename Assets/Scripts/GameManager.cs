using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int gold;
    public int goldGain;

    public int mana;
    public int manaGain;

    public bool inCombat;

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI manaText;

    public List<Structure> structures;
    public List<ArmyUnit> allUnits;

    public TempData data;

    public Dictionary<string, UnitData> unitData;
    public Dictionary<string, StructureData> structureData;

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

        UpdateText();
    }

    public void TurnStart()
    {
        gold += goldGain;
        mana = manaGain;

        UpdateText();
        UpdateStructures();
    }

    public void ToggleCombat()
    {
        inCombat = !inCombat;
    }

    private void UpdateText()
    {
        goldText.text = gold + " gold";
        manaText.text = mana + "/" + manaGain + " mana";
    }

    private void UpdateStructures()
    {
        foreach (Structure structure in structures) 
        { 
            structure.OnTurnStart();
        }
    }
}
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

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI manaText;

    public List<Structure> structures;

    public List<ArmyUnit> allUnits;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnStart()
    {
        gold += goldGain;
        mana = manaGain;

        UpdateText();
        UpdateStructures();
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

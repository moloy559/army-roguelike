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

    public GameInfo infoForGame;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateText();


        if(infoForGame != null) Debug.Log(infoForGame.level);

        infoForGame = new GameInfo();

        if (infoForGame != null) Debug.Log(infoForGame.level);
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

public class GameInfo
{
    public int level;
    public string directyName;
    public int gold;
}
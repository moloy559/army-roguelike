using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spire : ArmyUnit
{
    public bool amCentralSpire;
    protected override void Die()
    {
        gameObject.SetActive(false);
        if (amCentralSpire)
        {
            GameManager.Instance.GameOver();
        }
    }
}

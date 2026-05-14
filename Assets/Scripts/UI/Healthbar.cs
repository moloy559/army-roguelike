using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Slider healthbar;
    public ArmyUnit unit;

    private void FixedUpdate()
    {
        healthbar.value = unit.CurrentHealth/unit.maxHealth;
    }
}

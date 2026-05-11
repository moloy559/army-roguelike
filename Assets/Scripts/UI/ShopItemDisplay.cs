using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItemDisplay : MonoBehaviour
{
    public Image structureImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI goldCostText;
    public ResourceDisplay resourceDisplayInput;
    public ResourceDisplay resourceDisplayOutput;

    public void Fill(StructureData data)
    {
        nameText.text = data.name;
        structureImage.sprite = data.sprite;
        goldCostText.text = data.goldCost + " Gold";

        resourceDisplayInput.Fill(data.inputResource);
        resourceDisplayOutput.Fill(data.outputResource);

    }

}

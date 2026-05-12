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
    [SerializeField]
    private Toggle toggle;

    private StructureData structureData;
    private bool purchased;

  
    public void Fill(StructureData data)
    {
        purchased = false;

        structureData = data;
        nameText.text = data.name;
        structureImage.sprite = data.sprite;
        goldCostText.text = data.goldCost + " Gold";

        resourceDisplayInput.Fill(data.inputResource);
        resourceDisplayOutput.Fill(data.outputResource);

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        CheckAfford();

        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(OnToggled);
    }

    public void CheckPurchased(StructureData structureData)
    {
        if(structureData == this.structureData) purchased = true;
        CheckAfford();
        toggle.isOn = false;
    }

    public void CheckAfford()
    {
        if (purchased)
        {
            toggle.interactable = false;
            return;
        }
        toggle.interactable = (GameManager.Instance.Gold >= structureData.goldCost);
    }


    private void OnToggled(bool toggle)
    {
        if (toggle) 
        {
            ShopManager.Instance.SelectShopItem(structureData);
        }
        else
        {
            ShopManager.Instance.ToggleOffReset();
        } 
    }

}

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
    public TextMeshProUGUI rarityText;

    public ResourceDisplay resourceDisplayInput;
    public ResourceDisplay resourceDisplayOutput;

    public Transform resourceDisplayHolder;
    public GameObject resourceDisplayPrefab;
    public Transform breaker;

    [SerializeField]
    private Toggle toggle;

    private StructureData structureData;
    private bool purchased;

    private List<GameObject> displays = new();
  
    public void Fill(StructureData data)
    {
        purchased = false;

        structureData = data;
        nameText.text = data.name;
        structureImage.sprite = data.sprite;
        goldCostText.text = data.goldCost + " Gold";

        rarityText.text = data.rarity.ToString();

        for (int i = displays.Count - 1; i >= 0; i--)
        {
            if (displays[i] != null) Destroy(displays[i]);
        }
        displays.Clear();

        foreach (ResourceSet inputSet in data.transaction.inputResources) AddDisplay(inputSet);
        breaker.transform.SetAsLastSibling();
        foreach (ResourceSet outputSet in data.transaction.outputResources) AddDisplay(outputSet);

        //resourceDisplayInput.Fill(data.inputResource);
        //resourceDisplayOutput.Fill(data.outputResource);

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        CheckAfford();

        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(OnToggled);
    }

    private void AddDisplay(ResourceSet resourceSet)
    {
        GameObject newObj = Instantiate(resourceDisplayPrefab, resourceDisplayHolder);
        newObj.GetComponent<ResourceDisplay>().Fill(resourceSet);
        displays.Add(newObj);
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

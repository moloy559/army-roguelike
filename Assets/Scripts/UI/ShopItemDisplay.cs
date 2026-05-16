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

    public Transform transactionDisplayHolder;
    public GameObject transactionDisplayPrefab;


    public Transform resourceDisplayHolder;
    public GameObject resourceDisplayPrefab;
    public Transform breaker;

    [SerializeField]
    private Toggle toggle;

    [SerializeField]
    private float defaultHeight = 144f;
    [SerializeField]
    private float displayHeight = 100f;

    private RectTransform rect;
    private StructureData structureData;
    private bool purchased;

    private List<GameObject> displays = new();
      
    public void Fill(StructureData data)
    {
        rect = GetComponent<RectTransform>();
        purchased = false;

        structureData = data;
        nameText.text = data.name;
        structureImage.sprite = data.sprite;
        goldCostText.text = data.goldCost + " Gold";

        rarityText.text = data.rarity.ToString();

        rect.sizeDelta = new Vector2(rect.sizeDelta.x, defaultHeight + (data.transactions.Count * displayHeight));

        for (int i = displays.Count - 1; i >= 0; i--)
        {
            if (displays[i] != null) Destroy(displays[i]);
        }
        displays.Clear();

        foreach (ResourceTransaction transaction in data.transactions) 
        {
            AddDisplay(transaction);
        }


        LayoutRebuilder.ForceRebuildLayoutImmediate(transactionDisplayHolder.GetComponent<RectTransform>());
        Helpers.Delay(() => LayoutRebuilder.ForceRebuildLayoutImmediate(rect));
        CheckAfford();

        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(OnToggled);
    }

    private void AddDisplay(ResourceTransaction transaction)
    {
        GameObject newObj = Instantiate(transactionDisplayPrefab, transactionDisplayHolder);
        newObj.GetComponent<TransactionDisplay>().Fill(transaction);
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

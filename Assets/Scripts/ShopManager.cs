using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    public event Action<StructureData> onSelectStructure;
    public event Action onShopReset;

    public GameObject shopHolder;
    public List<ShopItemDisplay> shopItems;
    
    public ToggleGroup shopItemToggles;

    private bool open = false;
    public bool Open { get { return open; } }

    public StructureData selectedStructure;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        shopHolder.SetActive(false);
    }

    public void OpenShop()
    {
        shopHolder.SetActive(true);

        List<int> used = new();

        foreach (ShopItemDisplay item in shopItems)
        {
            int structIndex;
            do structIndex = Random.Range(0, GameManager.Instance.structureData.Count);
            while (used.Contains(structIndex));

            used.Add(structIndex);
            item.Fill(GameManager.Instance.structureData.ElementAt(structIndex).Value);
        }

        open = true;
    }

    public void CloseShop() 
    {
        shopHolder.SetActive(false);
        open = false;
    }

    /// <summary>
    /// A stupid function that uselesslly triggers twice every time a toggle is toggled, 
    /// but needs to trigger once out of 3 times when nothing is toggled. Thank you unity.
    /// </summary>
    public void ToggleOffReset()
    {
        if (!shopItemToggles.AnyTogglesOn())
        {
            onShopReset?.Invoke();
        }
    }


    public void SelectShopItem(StructureData structureData)
    {
        selectedStructure = structureData;
        onSelectStructure?.Invoke(selectedStructure);
    }

    public void PurchaseSelectedItem()
    {
        GameManager.Instance.Gold -= selectedStructure.goldCost;


        foreach (ShopItemDisplay item in shopItems)
        {
            item.CheckPurchased(selectedStructure);
        }
        selectedStructure = null;
        onShopReset?.Invoke();
    }
}

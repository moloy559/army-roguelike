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

        ShopGenerationData genData = GameManager.Instance.GetShopGeneration();

        List<StructureData> used = new();

        bool shouldForceUncommon = genData.forcedUncommon;
        bool shouldForceRare = genData.forcedRare;

        foreach (ShopItemDisplay item in shopItems)
        {
            Rarity rarity;
            if (shouldForceUncommon)
            {
                rarity = Rarity.Uncommon;
                shouldForceUncommon = false;
            }
            else if (shouldForceRare) 
            { 
                rarity = Rarity.Rare;
                shouldForceRare = false;
            }
            else
            {
                rarity = GetRandomRarity(genData);
            }

            List<StructureData> validStructures =
                GameManager.Instance.structureData.Values
                .Where(x => x.rarity == rarity && !used.Contains(x))
                .ToList();

            // if none exist of chosen rarity
            if (validStructures.Count == 0)
            {
                validStructures =
                    GameManager.Instance.structureData.Values
                    .Where(x => !used.Contains(x))
                    .ToList();
            }

            StructureData chosen =
                validStructures[Random.Range(0, validStructures.Count)];

            used.Add(chosen);

            item.Fill(chosen);
        }

        open = true;
    }

    private Rarity GetRandomRarity(ShopGenerationData data)
    {
        float total =
            data.commonWeight +
            data.uncommonWeight +
            data.rareWeight;

        float roll = Random.value * total;

        if (roll < data.commonWeight)
            return Rarity.Common;

        roll -= data.commonWeight;

        if (roll < data.uncommonWeight)
            return Rarity.Uncommon;

        return Rarity.Rare;
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

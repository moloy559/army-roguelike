using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TransactionDisplay : MonoBehaviour
{
    public TextMeshProUGUI transactionDescriptionText;

    public Transform resourceDisplayHolder;
    public GameObject resourceDisplayPrefab;
    public Transform breaker;

    private List<GameObject> displays = new();

    public void Fill(ResourceTransaction resourceTransaction)
    {
        string descText = "";

        if (resourceTransaction.cooldown > 0)
        {
            descText += "Now, and every " + resourceTransaction.cooldown + " turns, do";
        }
        else
        {
            descText += "Once, do";
        }

        if (resourceTransaction.repeats > 0)
        {
            descText += " " + (resourceTransaction.repeats + 1) + " times";
        }

        descText += ":";

        transactionDescriptionText.text = descText;

        for (int i = displays.Count - 1; i >= 0; i--)
        {
            if (displays[i] != null) Destroy(displays[i]);
        }
        displays.Clear();

        foreach (ResourceSet inputSet in resourceTransaction.inputResources) AddDisplay(inputSet);
        breaker.transform.SetAsLastSibling();
        foreach (ResourceSet outputSet in resourceTransaction.outputResources) AddDisplay(outputSet);
    }

    private void AddDisplay(ResourceSet resourceSet)
    {
        GameObject newObj = Instantiate(resourceDisplayPrefab, resourceDisplayHolder);
        newObj.GetComponent<ResourceDisplay>().Fill(resourceSet);
        displays.Add(newObj);
    }

}

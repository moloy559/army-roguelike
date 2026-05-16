using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class Structure : MonoBehaviour
{

    private StructureData data;

    private bool hasRecievedInput;
    public bool HasRecievedInput { get { return hasRecievedInput; } }

    private SpriteRenderer spriteRenderer;

    
    public List<StructureTransaction> structureTransactions;


    private List<ResourceTransaction> outstandingTransactions = new();

    public List<ResourceTransaction> OutstandingTransactions { get { return outstandingTransactions; } }

    public void Fill(StructureData structureData)
    {
        data = structureData;   
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = structureData.sprite;

        foreach(ResourceTransaction resourceTransaction in structureData.transactions)
        {
            StructureTransaction structureTransaction = new(resourceTransaction);
            structureTransactions.Add(structureTransaction);
            structureTransaction.AddTransactions(outstandingTransactions);
        }
    }

    public void RoundUpdate()
    {
        foreach (StructureTransaction structureTransaction in structureTransactions)
        {
            structureTransaction.RoundUpdate();
        }
    }

    public bool CanGiveOutput(SerializedDictionary<string, int> resources)
    {
        if (data.transactions[0].inputResources.Count == 0) return true;
        foreach (ResourceSet input in data.transactions[0].inputResources) 
        { 
            if (!resources.ContainsKey(input.resourceName))
            {
                if (input.amount > 0) return false;
            }
            else
            {
                if (resources[input.resourceName] < input.amount)
                {
                    return false;
                } 
            }
            
        }

        return true;
    }

    public void ModifyResources(SerializedDictionary<string, int> resources)
    {
        if(hasRecievedInput)
        {
            Debug.LogError("Structure already modified resources");
            return;
        }

        foreach (ResourceSet input in data.transactions[0].inputResources)
        {
            resources[input.resourceName] -= input.amount;
        }

        foreach (ResourceSet output in data.transactions[0].outputResources)
        {
            if (!resources.ContainsKey(output.resourceName))
            {
                resources.Add(output.resourceName, 0);
            }
            resources[output.resourceName] += output.amount;
        }

        hasRecievedInput = true;
    }

}

[System.Serializable]
public class StructureTransaction
{
    public int maxRoundsCooldown;
    public bool hasCooldown = false;

    public ResourceTransaction transactionData;

    private int outstandingTransactions = 0;
    private int roundsForCooldown = 0;

    public StructureTransaction(ResourceTransaction transactionData)
    {
        this.transactionData = transactionData;
        maxRoundsCooldown = transactionData.cooldown;
        if (maxRoundsCooldown >= 0) hasCooldown = true;
        RefillTransactions();
    }

    private void RefillTransactions()
    {
        outstandingTransactions += (transactionData.repeats + 1); 
    }

    public void RoundUpdate()
    {
        if (hasCooldown)
        {
            roundsForCooldown++;
            if(roundsForCooldown >= maxRoundsCooldown)
            {
                RefillTransactions();
                roundsForCooldown = 0;
            }
        }
    }

    public void AddTransactions(List<ResourceTransaction> transactionToAddTo)
    {
        for(int i = 0; i < outstandingTransactions; i++)
        {
            transactionToAddTo.Add(transactionData);
        }
        outstandingTransactions = 0;
    }

}
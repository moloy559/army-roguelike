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

    public void Fill(StructureData structureData)
    {
        data = structureData;   
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = structureData.sprite;


    }

    public bool CanGiveOutput(SerializedDictionary<string, int> resources)
    {
        if (data.transaction.inputResources.Count == 0) return true;
        foreach (ResourceSet input in data.transaction.inputResources) 
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

        foreach (ResourceSet input in data.transaction.inputResources)
        {
            resources[input.resourceName] -= input.amount;
        }

        foreach (ResourceSet output in data.transaction.outputResources)
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
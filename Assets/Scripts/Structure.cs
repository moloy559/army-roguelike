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
        if (data.inputResource.amount == 0) return true;
        if (resources.ContainsKey(data.inputResource.resourceName))
        {
            if (resources[data.inputResource.resourceName] >= data.inputResource.amount) return true;
        }
        return false;
    }

    public void ModifyResources(SerializedDictionary<string, int> resources)
    {
        if(hasRecievedInput)
        {
            Debug.LogError("Structure already modified resources");
            return;
        }

        if (data.inputResource.amount !=0)
            resources[data.inputResource.resourceName] -= data.inputResource.amount;

        if (!resources.ContainsKey(data.outputResource.resourceName))
        {
            resources.Add(data.outputResource.resourceName, 0);
        }
        resources[data.outputResource.resourceName] += data.outputResource.amount;
        hasRecievedInput = true;
    }

}
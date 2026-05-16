using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class LayoutObjects : MonoBehaviour
{
    public List<GameObject> objects;

    public Vector2 offset;

    public int width;

    public Transform startingPosition;

    public int numberOfLanes = 200;
    public GameObject lanePrefab;

    [Button]
    public void Layout()
    {
        Vector2 currentOffset = Vector2.zero;
        int currWidth = 0;

        LaneSimulationManager manager = GetComponent<LaneSimulationManager>();

        manager.lanes.Clear();

        for (int i = objects.Count - 1; i >= 0; i--)
        {
            DestroyImmediate(objects[i]);
        }
        objects = new();

        for (int i = 0; i < numberOfLanes; i++) 
        {
           Object obj = PrefabUtility.InstantiatePrefab(lanePrefab,transform);
            manager.lanes.Add(obj.GetComponent<SimulatorLane>());
            objects.Add((GameObject)obj);
        }


        foreach (GameObject obj in objects)
        {
            obj.transform.position = startingPosition.position + (Vector3)currentOffset;
            currWidth++;
            if (currWidth >= width)
            {
                currentOffset = new Vector2(0f, currentOffset.y + offset.y);
                currWidth = 0;
            }
            else
            {
                currentOffset = new Vector2(currentOffset.x + offset.x, currentOffset.y);
            }
        }
    }
}

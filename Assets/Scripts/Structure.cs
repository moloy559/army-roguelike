using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Structure : MonoBehaviour
{
    [SerializeField]
    private GameObject unitPrefab;

    [SerializeField]
    private Transform unitSpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTurnStart()
    {
        Instantiate(unitPrefab, unitSpawnPoint.position, Quaternion.identity);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SimulationManager : MonoBehaviour
{

    public TextAsset textAssetData;


    [System.Serializable]
    public class UnitDataList
    {
        public UnitData[] unitData;
    }

    public UnitDataList myUnitDataList = new UnitDataList();

    public int NumberOfColumns;
    public GameObject QuickSim;

    public float SimOffset;
    public float SimulationsOffset;
    public float UnitSeperation;
    public int numberPerSim;

    public List<GameObject> quickSimList;

    string filename = "";
    [System.Serializable]
    public class OutputData
    {
        public string Name;
        public string remainingHealth;
    }

    [System.Serializable]
    public class OutputDataList
    {
        public OutputData[] outputData;
    }

    public OutputDataList myOutputDataList = new OutputDataList();


    void ReadCSV()
    {
        //Split up data from csv
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        //Divide up data into sets
        int tableSize = data.Length / NumberOfColumns - 1;
        myUnitDataList.unitData = new UnitData[tableSize];

        for(int i =0; i<tableSize; i++)
        {
            myUnitDataList.unitData[i] = new UnitData();
            myUnitDataList.unitData[i].name = data[7 * (i + 1)];
            myUnitDataList.unitData[i].maxHealth = float.Parse(data[NumberOfColumns * (i + 1)+1]);
            myUnitDataList.unitData[i].attackDamage = float.Parse(data[NumberOfColumns * (i + 1) + 1]);
            myUnitDataList.unitData[i].attackRange = float.Parse(data[NumberOfColumns * (i + 1) + 1]);
            myUnitDataList.unitData[i].attackSpeed = float.Parse(data[NumberOfColumns * (i + 1) + 1]);
            myUnitDataList.unitData[i].moveSpeed = float.Parse(data[NumberOfColumns * (i + 1) + 1]);
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        ReadCSV();

        createQuickSimGroup();

        filename = Application.dataPath + "/test.csv";


    }

    // Update is called once per frame
    void Update()
    {

        if (IsAllSimOver())
        {

            WriteCSV();
            return;
        }
        

        
    }


    private GameObject CreateQuickSim(int SimNumber)
    {
        return Instantiate(QuickSim, Vector3.zero + new Vector3(SimulationsOffset + (SimNumber * SimOffset), 0, 0),Quaternion.identity); 
    }


    private void createQuickSimGroup()
    {
        for(int i = 0; i<numberPerSim; i++)
        {
            quickSimList.Add(CreateQuickSim(i));
            quickSimList[i].GetComponent<QuickSim>().unitData1 = myUnitDataList.unitData[i];
            quickSimList[i].GetComponent<QuickSim>().unitDist = UnitSeperation;
        }

    }


    public void WriteCSV()
    {
        if(myOutputDataList.outputData.Length>0)
        {
            TextWriter tw = new StreamWriter(filename, false);
            tw.WriteLine("Name, Remaining Health");
            tw.Close();

            tw = new StreamWriter(filename, true);
            
            for(int i=0; i<myOutputDataList.outputData.Length;i++ )
            {
                tw.WriteLine(myOutputDataList.outputData[i].Name + "," + myOutputDataList.outputData[i].remainingHealth);
            }
            tw.Close();

        }
    }

    private void addOutputData()
    {

    }



    private bool IsAllSimOver()
    {
        for(int i =0; i< quickSimList.Count;i++)
        {
            if (quickSimList[i].GetComponent<QuickSim>().simOver==false)
            {
                return false;
            }
        }
        return true;

    }
}

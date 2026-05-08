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
    public int simsPerBatch;
    private int currentSimBatch = 0;

    public List<GameObject> quickSimList;

    public bool dataIsWritten = false;
    public bool done = false;

    string filename = "";
    [System.Serializable]
    public class OutputData
    {
        public string Matchup;
        public string Name;
        public float remainingHealth;
    }

    [System.Serializable]
    public class OutputDataList
    {
        public List<OutputData> outputData;
    }

    public OutputDataList myOutputDataList = new OutputDataList();

    public class Combination
    {
        public int unit1;
        public int unit2;
    }


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
            myUnitDataList.unitData[i].name = data[NumberOfColumns * (i + 1)];
            myUnitDataList.unitData[i].maxHealth = float.Parse(data[NumberOfColumns * (i + 1) + 1]);
            myUnitDataList.unitData[i].attackDamage = float.Parse(data[NumberOfColumns * (i + 1)+2]);
            myUnitDataList.unitData[i].attackRange = float.Parse(data[NumberOfColumns * (i + 1)+3]);
            myUnitDataList.unitData[i].attackSpeed = float.Parse(data[NumberOfColumns * (i + 1) + 4]);
            myUnitDataList.unitData[i].moveSpeed = float.Parse(data[NumberOfColumns * (i + 1) + 5]);
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        ReadCSV();

        createQuickSimGroup(currentSimBatch);

        filename = Application.dataPath + "/test.csv";

        for (int i = 0; i < CreateCombinations().Count; i++)
            Debug.Log(CreateCombinations()[i].unit1 + ", " + CreateCombinations()[i].unit2);

        //Debug.Log(CreateCombinations().Count / simsPerBatch);


    }

    // Update is called once per frame
    void Update()
    {



        if (IsAllSimOver())
        {
            if (dataIsWritten)
            {

                CleanUpAfterBatch();
                GameManager.Instance.inCombat = false;
                if (currentSimBatch < (CreateCombinations().Count / simsPerBatch)-1)
                {

                    currentSimBatch++;
                    dataIsWritten = false;
                    createQuickSimGroup(currentSimBatch);
                    GameManager.Instance.inCombat = true;
                }
                else if (done == false)
                {

                    WriteCSV();

                    Debug.Log("The Simulations are done.");
                    done = true;

                }
                return;
            }
            else
            {
                addOutputData();

                
                dataIsWritten = true;

                
            }

            
            return;
        }
        

        
    }


    private GameObject CreateQuickSim(int SimNumber)
    {
        return Instantiate(QuickSim, Vector3.zero + new Vector3(SimulationsOffset + (SimNumber * SimOffset), 0, 0),Quaternion.identity); 
    }


    private void createQuickSimGroup(int currentSimBatch)   
    {

        for (int i = 0; i<simsPerBatch; i++)
        {
            int combinationNum = new int();
            combinationNum = i + (simsPerBatch * currentSimBatch);

            //Debug.Log(combinationNum);

                quickSimList.Add(CreateQuickSim(i));
                quickSimList[i].GetComponent<QuickSim>().unitData1 = myUnitDataList.unitData[CreateCombinations()[combinationNum].unit1-1];
                quickSimList[i].GetComponent<QuickSim>().unitData2 = myUnitDataList.unitData[CreateCombinations()[combinationNum].unit2-1];
                quickSimList[i].GetComponent<QuickSim>().unitDist = UnitSeperation;
       

        }

    }

    private void CleanUpAfterBatch()
    {
        for(int i = 0; i<quickSimList.Count;i++)
        {
            if (quickSimList[i].GetComponent<QuickSim>().unit1 != null)
            {
                Destroy(quickSimList[i].GetComponent<QuickSim>().unit1.gameObject);
            }

            if (quickSimList[i].GetComponent<QuickSim>().unit2 != null)
            {
                Destroy(quickSimList[i].GetComponent<QuickSim>().unit2.gameObject);
            }
                
            Destroy(quickSimList[i].gameObject);

        }

        quickSimList.Clear();

    }


    public void WriteCSV()
    {
        if(myOutputDataList.outputData.Count>0)
        {
            TextWriter tw = new StreamWriter(filename, false);
            tw.WriteLine("Matchup, Winner, Remaining Health");
            tw.Close();

            tw = new StreamWriter(filename, true);
            
            for(int i=0; i<myOutputDataList.outputData.Count;i++ )
            {
                tw.WriteLine(myOutputDataList.outputData[i].Matchup+ ","+ myOutputDataList.outputData[i].Name + "," + myOutputDataList.outputData[i].remainingHealth);
            }
            tw.Close();

        }
    }

    private void addOutputData()
    {
        for (int i = 0; i < quickSimList.Count; i++)
        {

            OutputData outputData = new OutputData();

            outputData.Matchup = new string(CreateCombinations()[i+ (simsPerBatch * currentSimBatch)].unit1 + "--" + CreateCombinations()[i+ (simsPerBatch * currentSimBatch)].unit2);
            if (quickSimList[i].GetComponent<QuickSim>().unit1 == null)
            {
                
                outputData.Name = quickSimList[i].GetComponent<QuickSim>().unitData2.name;
                outputData.remainingHealth = quickSimList[i].GetComponent<QuickSim>().unit2.CurrentHealth;
            }

            if (quickSimList[i].GetComponent<QuickSim>().unit2 == null)
            {
                outputData.Name = quickSimList[i].GetComponent<QuickSim>().unitData1.name;
                outputData.remainingHealth = quickSimList[i].GetComponent<QuickSim>().unit1.CurrentHealth;
            }

            myOutputDataList.outputData.Add(outputData);
  
        }
    }

    private List<Combination> CreateCombinations()
    {
       List<Combination> newCombinations = new List<Combination>();
        foreach (var unit1 in myUnitDataList.unitData)
        {
            foreach (var unit2 in myUnitDataList.unitData)
            {
                Combination TempCombination = new Combination();

                TempCombination.unit1 = int.Parse(unit1.name);
                TempCombination.unit2 = int.Parse(unit2.name);


                if (TempCombination.unit1 != TempCombination.unit2) 
                { 
                    newCombinations.Add(TempCombination);
                }
            }

        }

        return newCombinations;
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

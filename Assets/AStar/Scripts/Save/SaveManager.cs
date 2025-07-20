using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;


[Serializable]
public class SaveData
{
    [Space(5)][Header("Grid Data")]
    public int GridSize;
    public int MaxHeight;
    public Vector3 Offset;
    public float OffsetRandomization;
    public float NoiseScale;
    public float ObstacleDensity;
    
    [Space(5)][Header("Pathfinding Data")]
    [Space(5)][Header("Time")]
    public float AStarTime;
    public float GBFSTime;
    public float ILSWithAStarTime;

    [Space(5)][Header("Space")]
    public float AStarSpace;
    public float GBFSSpace;
    public float ILSWithAStarSpace;
    
    [Space(5)][Header("Path")]
    public float ILSIterations;
}

public class SaveManager
{
    public List<SaveData> saveDataList = new List<SaveData>();
    
    public void SaveAndExport(SaveData data, string fileName)
    {
        Save(data);
        Export(fileName);
    }
    
    public void Save(SaveData data)
    {
        saveDataList.Add(data);
        Debug.Log("Data saved successfully!");
    }
    public void Export(string fileName)
    {
        string json = JsonUtility.ToJson(saveDataList, true);
        System.IO.File.WriteAllText(fileName, json);
        Debug.Log($"Data exported to {fileName} successfully!");
    }
    
    public void Clear()
    {
        saveDataList.Clear();
        Debug.Log("All saved data cleared!");
    }
    
}

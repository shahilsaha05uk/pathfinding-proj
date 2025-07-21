using System;
using System.Collections.Generic;
using UnityEngine;


public class SaveManager
{
    private List<SaveData> saveData = new List<SaveData>();

    public void AddSaveData(SaveData data)
    {
        if (data != null)
        {
            saveData.Add(data);
        }
        else
        {
            Debug.LogWarning("Attempted to add null SaveData.");
        }
    }

    public string SaveAndExport(string fileName = "data", string directory = "Exported Data")
    {
        return CSVExporter.ExportToCSV(saveData, fileName, directory);
    }

    public SaveData CreateSaveData(GridConfig config, List<EvaluationResult> results)
    {
        return new SaveData
        {
            GridSize = config.GridSize,
            MaxHeight = config.MaxHeight,
            Offset = config.Offset,
            OffsetRandomization = config.OffsetRandomization,
            NoiseScale = config.NoiseScale,
            EvaluationResult = results,
        };
    }

    public void ClearSaveData() => saveData.Clear();
}

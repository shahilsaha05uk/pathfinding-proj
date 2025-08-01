using System.Collections.Generic;
using UnityEngine;

public class EvaluationDataSaver
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


    public string SaveAndExport(int gridSize, int obstacleDensity)
    {
        var result = SaveManager.SaveAndExport(
            saveData,
            fileName: $"{gridSize}x_{gridSize}x_{gridSize}x_ob{obstacleDensity}",
            directory: "Exported Data"
        );

        saveData.Clear();
        return result;
    }

    public SaveData CreateSaveData(GridData data, List<EvaluationResult> results)
    {
        return new SaveData
        {
            GridSize = data.GridSize,
            ObstacleDensity = data.ObstacleDensity * 100,
            EvaluationResult = results,
        };
    }
}

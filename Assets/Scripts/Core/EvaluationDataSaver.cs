using System.Collections.Generic;
using UnityEngine;

public class EvaluationDataSaver
{
    private readonly List<SaveData> saveData = new();

    public void AddToMemory(
        AutoEvaluationConfig settings,
        GridConfig config,
        List<EvaluationResult> results)
    {
        var save = CreateSaveData(settings, config, results);
        if (save != null)
        {
            saveData.Add(save);
        }
        else
        {
            Debug.LogWarning("Attempted to add null SaveData.");
        }
    }


    public string Export(GridConfig config, AutoEvaluationConfig settings)
    {
        var gridSize = config.GridSize;
        var obstacleDensity = Mathf.RoundToInt(config.ObstacleDensity * 100);
        var result = SaveManager.Export(
            config,
            settings,
            saveData,
            fileName: $"{gridSize}x_{gridSize}x_{gridSize}x_ob{obstacleDensity}",
            directory: "Exported Data"
        );

        saveData.Clear();
        return result;
    }

    private SaveData CreateSaveData(
        AutoEvaluationConfig settings,
        GridConfig config, 
        List<EvaluationResult> results)
    {
        return new SaveData
        {
            GridSize = config.GridSize,
            ObstacleDensity = config.ObstacleDensity * 100,
            EvaluationResult = results,
            MaxHeight = config.MaxHeight,
            NoiseScale = config.NoiseScale,
            BatchSize = settings.BatchSize,
        };
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EvaluationDataSaver
{
    private readonly List<SaveData> saveData = new();
    public int ResultDataCount => saveData.Select(s => s.EvaluationResult.Select(r => r.ResultCount).Sum()).Sum();
    public int SaveDataCount => saveData.Count;

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
        var obstacleSeed = config.ObstacleSeed;
        var result = SaveManager.Export(
            config,
            settings,
            saveData,
            fileName: $"{gridSize}x_{gridSize}x_{gridSize}",
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
            ObstacleSeed = config.ObstacleSeed,
            EvaluationResult = results,
            MaxHeight = config.MaxHeight,
            NoiseScale = config.NoiseScale,
            BatchSize = settings.BatchSize,
        };
    }
}

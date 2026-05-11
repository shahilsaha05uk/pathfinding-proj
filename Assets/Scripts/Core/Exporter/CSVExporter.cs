using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public static class CSVExporter
{
    private static string extension = ".csv";

    public static string ExportToCSV(
        GridConfig config,
        AutoEvaluationConfig settings,
        List<SaveData> saveData, 
        string filename, 
        string directory)
    {
        if (saveData == null || string.IsNullOrEmpty(filename))
        {
            Debug.LogWarning("No data to export.");
            return default;
        }

        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string folderPath = Path.Combine(projectRoot, directory);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        int fileIndex = GetNextFileIndex(folderPath, filename);
        string fileName = (filename == "data") ? $"{filename}_{fileIndex}.csv" : $"{filename}.csv";
        string fullPath = Path.Combine(folderPath, fileName);

        bool success = CreateCSV(config, settings, saveData, fullPath);
        return success ? fullPath : null;
    }

    private static bool CreateCSV(
        GridConfig config,
        AutoEvaluationConfig settings, 
        List<SaveData> saveData, 
        string fullpath)
    {
        var sb = new StringBuilder();

        sb.AppendLine(
            "GridSize," +
            "MaxHeight," +
            "NoiseScale," +
            "ObstacleDensity," +
            "BatchSize," +
            "Algorithm," +
            "TimeTaken," +
            "PathLength," +
            "PathCost," +
            "VisitedNodes," +
            "MeasuredTimeMs," +
            "MemoryUsedBytes," +
            "MemoryUsedInKb," + 
            "Message");

        foreach (var data in saveData)
        {
            if (data.EvaluationResult == null) continue;

            foreach (var result in data.EvaluationResult)
            {
                var r = result.Results;
                foreach (var item in r)
                    AppendRow(
                        sb, 
                        data, 
                        config, 
                        settings, 
                        item.Key.ToString(), 
                        item.Value);
            }
        }

        File.WriteAllText(fullpath, sb.ToString());
        Debug.Log($"✅ CSV successfully exported to: {fullpath}");
        return true;
    }

    private static void AppendRow(
        StringBuilder sb,
        SaveData saveData,
        GridConfig config,
        AutoEvaluationConfig settings,
        string algorithmName,
        EvaluationData data)
    {
        if (data == null) return;

        sb.AppendLine(string.Join(",", new string[]
        {
            config.GridSize.ToString(),
            saveData.MaxHeight.ToString(),
            saveData.NoiseScale.ToString("F3"),
            saveData.ObstacleDensity.ToString("F3"),
            settings.BatchSize.ToString(),
            algorithmName,
            data.TimeTaken.ToString("F3"),
            data.PathLength.ToString(),
            data.PathCost.ToString("F3"),
            data.VisitedNodes.ToString(),
            data.MeasuredTimeMs.ToString("F3"),
            data.MemoryUsedBytes.ToString(),
            (data.MemoryUsedBytes / 1024f).ToString("F3"),
            data.Message,
        }));
    }

    private static int GetNextFileIndex(string folderPath, string baseFileName)
    {
        var files = Directory.GetFiles(folderPath, $"{baseFileName}_*{extension}");
        int maxIndex = 0;

        foreach (var file in files)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            string[] parts = name.Split('_');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int index))
            {
                if (index > maxIndex)
                    maxIndex = index;
            }
        }

        return maxIndex + 1;
    }
}

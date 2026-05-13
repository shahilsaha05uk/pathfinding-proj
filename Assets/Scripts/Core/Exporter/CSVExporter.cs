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
        string folderPath = PrepareDir(directory);
        string fileName = CreateFileName(filename, folderPath);
        string fullPath = Path.Combine(folderPath, fileName);

        bool success = CreateCSV(config, settings, saveData, fullPath);
        return success ? fullPath : null;
    }

    private static string CreateFileName(string filename, string folderPath)
    {
        int fileIndex = GetNextFileIndex(folderPath, filename);
        string fileName = (fileIndex == 0) ? $"{filename}.csv" : $"{filename}_{fileIndex}.csv";
        return fileName;
    }

    private static string PrepareDir(string directory)
    {
        string folderPath = GetExportDir(directory);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        else
        {
            // if there are any files in there, delete them first
            //if (Directory.GetFiles(folderPath).Length > 0)
            //{
            //    foreach (var file in Directory.GetFiles(folderPath))
            //    {
            //        File.Delete(file);
            //    }
            //}
        }

        return folderPath;
    }

    private static string GetExportDir(string directory)
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string folderPath = Path.Combine(projectRoot, directory);
        return folderPath;
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
        if(Directory.GetFiles(folderPath).Length == 0)
            return 0;

        var files = Directory.GetFiles(folderPath, $"{baseFileName}_*{extension}");
        return files.Length + 1;
    }
}

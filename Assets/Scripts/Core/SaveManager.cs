using System.Collections.Generic;

public static class SaveManager
{
    public static string Export(
        GridConfig config,
        AutoEvaluationConfig settings,
        List<SaveData>saveData, 
        string fileName = "data", 
        string directory = "Exported Data")
    {
        return CSVExporter.ExportToCSV(
            config,
            settings,
            saveData, 
            fileName, 
            directory);
    }

    public static void ClearSaveData(List<SaveData> saveData) => saveData.Clear();
}

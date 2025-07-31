using System;
using System.Collections.Generic;
using UnityEngine;


public static class SaveManager
{
    public static string SaveAndExport(List<SaveData>saveData, string fileName = "data", string directory = "Exported Data")
    {
        return CSVExporter.ExportToCSV(saveData, fileName, directory);
    }

    public static void ClearSaveData(List<SaveData> saveData) => saveData.Clear();
}

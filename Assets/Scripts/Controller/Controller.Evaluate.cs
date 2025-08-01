using UnityEngine;
using UnityEngine.Profiling;

public partial class Controller
{
    public EvaluationResult OnEvaluate(int evalSize, EvaluateAlgorithms evaluateAlgorithms)
    {
        if (evalSize <= 0)
        {
            Debug.LogError("Evaluation size must be greater than 0.");
            return null;
        }

        // Evaluate the algorithms and collect the results
        var result = evaluator.Evaluate(evalSize, evaluateAlgorithms);

        if (result == null) return null;

        SaveAndExport();

        return result;
    }

    public string SaveAndExport()
    {
        var results = evaluator.GetEvaluationResults();
        var gridData = mGrid.GetGridData();

        var data = evaluationDataSaveManager.CreateSaveData(gridData, results);
        evaluationDataSaveManager.AddSaveData(data);
        var status = evaluationDataSaveManager.SaveAndExport(gridData.GridSize, Mathf.FloorToInt(gridData.ObstacleDensity * 100));

        evaluator.ClearResults();
        return status;
    }
}
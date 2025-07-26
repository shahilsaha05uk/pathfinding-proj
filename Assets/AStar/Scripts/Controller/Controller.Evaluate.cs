using UnityEngine;
using UnityEngine.Profiling;

public partial class Controller
{
    public EvaluationResult OnEvaluate(int evalSize)
    {
        if (evalSize <= 0)
        {
            Debug.LogError("Evaluation size must be greater than 0.");
            return null;
        }

        // Evaluate the algorithms and collect the results
        var result = Evaluate(evalSize);

        if (result == null) return null;

        // If bSave is true, save the evaluation results
        SaveAndExport();

        return result;
    }

    private EvaluationResult Evaluate(int evalSize) => evaluator.Evaluate(evalSize);

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
using UnityEngine;
using UnityEngine.Profiling;

public partial class Controller
{
    public EvaluationResult OnEvaluate(int evalSize, bool bSaveAndExport)
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
        if (bSaveAndExport)
            Export();
        
        return result;
    }

    private EvaluationResult Evaluate(int evalSize) => evaluator.Evaluate(evalSize);

    private void Export()
    {
        var results = evaluator.GetEvaluationResults();
        
        var data = saveManager.CreateSaveData(mGrid.GetGridConfig(), results);
        saveManager.AddSaveData(data);
        saveManager.SaveAndExport();

        evaluator.ClearResults();
    }
}
using System.Collections.Generic;
using UnityEngine;

public partial class Controller
{
    private List<EvaluationResult> evaluationResults = new List<EvaluationResult>();

    public EvaluationResult OnEvaluate(int evalSize, bool bSaveAndExport)
    {
        if (evalSize <= 0)
        {
            Debug.LogError("Evaluation size must be greater than 0.");
            return null;
        }

        // Evaluate the algorithms and collect the results
        Evaluate(evalSize);

        if (evaluationResults.Count <= 0) return null;

        // If bSave is true, save the evaluation results
        if (bSaveAndExport)
            Export();
        
        return evaluationResults[evalSize - 1];
    }

    private void Evaluate(int evalSize)
    {
        // Delete the last evaluation results
        if (evaluationResults != null && evaluationResults.Count > 0)
            evaluationResults.Clear();

        int count = 0;

        // collect the data from the algorithms and add it to the list
        while (count < evalSize)
        {
            var results = evaluator.Evaluate();
            evaluationResults.Add(results);
            count++;
        }
    }

    private void Export()
    {
        var data = saveManager.CreateSaveData(mGrid.GetGridConfig(), evaluationResults);
        saveManager.AddSaveData(data);
        saveManager.SaveAndExport();
    }
}
using UnityEngine;

public partial class Controller
{
    public EvaluationData OnNavigate(AlgorithmType algorithmType, int corridorWidth = 1)
    {
        grid.ResetPath();

        var (start, end) = grid.GetStartEndNodes();
        PathResult result = pathfindingManager.RunAlgorithm(algorithmType, start, end);
        if (result != null)
            grid.HighlightPath(result.Path);

        return EvaluationResult.FromPathResult(result);
    }

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
}
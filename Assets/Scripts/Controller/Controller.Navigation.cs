using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public partial class Controller
{
    public EvaluationData OnNavigate(AlgorithmType algorithmType, int corridorWidth = 1, bool enableStats = false)
    {
        grid.ResetPath();
        var (start, end) = grid.GetStartEndNodes();

        if (enableStats)
        {
            var (result, stats) = Stats.RecordStats(() =>
            {
                PathResult result = pathfindingManager.RunAlgorithm(algorithmType, start, end);
                if (result != null)
                    grid.HighlightPath(result.Path);
                return result;
            });

            return EvaluationResult.FromPathResult(result, stats);
        }
        else
        {
            PathResult result = pathfindingManager.RunAlgorithm(algorithmType, start, end);
            if (result != null)
                grid.HighlightPath(result.Path);
            return EvaluationResult.FromPathResult(result);
        }
    }

    public EvaluationResult OnEvaluate(int evalSize, HashSet<AlgorithmType> algorithms)
    {
        if (evalSize <= 0)
        {
            UnityEngine.Debug.LogError("Evaluation size must be greater than 0.");
            return null;
        }

        // Evaluate the algorithms and collect the results
        var result = evaluator.Evaluate(evalSize, algorithms);

        if (result == null) return null;

        SaveAndExport();

        return result;
    }
}
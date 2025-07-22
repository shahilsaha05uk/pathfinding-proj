using System.Collections.Generic;
using UnityEngine;

public class PathfindingEvaluator : MonoBehaviour
{
    [SerializeField] private Grid3D mGrid;
    [SerializeField] private PathfindingManager mPathManager;
    private List<EvaluationResult> evaluationResults = new List<EvaluationResult>();

    public List<EvaluationResult> GetEvaluationResults() => evaluationResults;

    public EvaluationResult Evaluate(int evalSize)
    {
        // Pre evaluation
        if (evalSize <= 0)
        {
            Debug.LogError("Evaluation size must be greater than 0.");
            return null;
        }

        // -- Clear data if exists.
        ClearResults();

        var nodes = mGrid.GetStartEndNodes();

        // Evaluate the algorithms
        StartEvaluation(evalSize, nodes.start, nodes.goal);

        // Post evaluation
        if (evaluationResults.Count <= 0) return null;

        return evaluationResults[evalSize - 1];
    }

    public void ClearResults()
    {
        if (evaluationResults != null && evaluationResults.Count > 0)
            evaluationResults.Clear();
    }

    private void StartEvaluation(int evalSize, Node start, Node goal)
    {
        int count = 0;

        while (count < evalSize)
        {
            var results = GatherEvaluationData(start, goal);
            evaluationResults.Add(results);
            count++;
        }
    }

    private EvaluationResult GatherEvaluationData(Node start, Node goal)
    {
        var aStar = EvaluationResult.FromPathResult(mPathManager.RunAStar(start, goal));
        var ilsWithAStar = EvaluationResult.FromPathResult(mPathManager.RunILSWithAStar(start, goal));
        var gbfs = EvaluationResult.FromPathResult(mPathManager.RunGBFS(start, goal));
        var ilsWithGBFS = EvaluationResult.FromPathResult(mPathManager.RunILSWithGBFS(start, goal));

        return new EvaluationResult
        {
            AStar = aStar,
            ILSWithAStar = ilsWithAStar,
            GBFS = gbfs,
            ILSWithGBFS = ilsWithGBFS,
        };
    }
}

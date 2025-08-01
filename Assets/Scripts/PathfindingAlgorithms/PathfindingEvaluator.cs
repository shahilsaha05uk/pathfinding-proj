using System.Collections.Generic;
using UnityEngine;

public class PathfindingEvaluator : MonoBehaviour
{
    [SerializeField] private Grid3D mGrid;
    [SerializeField] private PathfindingManager mPathManager;
    private List<EvaluationResult> evaluationResults = new List<EvaluationResult>();

    public List<EvaluationResult> GetEvaluationResults() => evaluationResults;

    public EvaluationResult Evaluate(int evalSize, EvaluateAlgorithms evaluateAlgorithms)
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
        StartEvaluation(evalSize, nodes.start, nodes.goal, evaluateAlgorithms);

        // Post evaluation
        if (evaluationResults.Count <= 0) return null;

        return evaluationResults[evalSize - 1];
    }

    public void ClearResults()
    {
        if (evaluationResults != null && evaluationResults.Count > 0)
            evaluationResults.Clear();
    }

    private void StartEvaluation(int evalSize, Node start, Node goal, EvaluateAlgorithms evaluateAlgorithms)
    {
        int count = 0;

        while (count < evalSize)
        {
            var results = GatherEvaluationData(start, goal, evaluateAlgorithms);
            evaluationResults.Add(results);
            count++;
        }
    }

    private EvaluationResult GatherEvaluationData(Node start, Node goal, EvaluateAlgorithms evaluateAlgorithms)
    {
        var aStar = (evaluateAlgorithms.AStar)? EvaluationResult.FromPathResult(mPathManager.RunAStar(start, goal)) : null;
        var gbfs = (evaluateAlgorithms.GBFS)? EvaluationResult.FromPathResult(mPathManager.RunGBFS(start, goal)) : null;
        var jps = (evaluateAlgorithms.JPS) ? EvaluationResult.FromPathResult(mPathManager.RunJPS(start, goal)) : null;
        var dijkstra = (evaluateAlgorithms.Dijkstra) ? EvaluationResult.FromPathResult(mPathManager.RunDijkstra(start, goal)) : null;
        var ilsWithAStar = (evaluateAlgorithms.ILSAStar) ? EvaluationResult.FromPathResult(mPathManager.RunILSWithAStar(start, goal)) : null;
        var ilsWithGBFS = (evaluateAlgorithms.ILSGBFS) ? EvaluationResult.FromPathResult(mPathManager.RunILSWithGBFS(start, goal)) : null;
        var ilsWithDijkstra = (evaluateAlgorithms.ILSDijkstra) ? EvaluationResult.FromPathResult(mPathManager.RunILSWithDijkstra(start, goal)) : null;

        return new EvaluationResult
        {
            AStar = aStar,
            GBFS = gbfs,
            JPS = jps,
            Dijkstra = dijkstra,
            ILSWithAStar = ilsWithAStar,
            ILSWithDijkstra = ilsWithDijkstra,
            ILSWithGBFS = ilsWithGBFS,
        };
    }
}

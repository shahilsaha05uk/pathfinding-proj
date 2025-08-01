using System.Collections.Generic;
using UnityEngine;

public class PathfindingEvaluator : MonoBehaviour
{
    [SerializeField] private Grid3D grid;
    [SerializeField] private PathfindingManager pathManager;
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

        var nodes = grid.GetStartEndNodes();

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
        // Basic algorithms
        var aStar = (evaluateAlgorithms.AStar)? GetEvaluationData<AStar>(start, goal) : null;
        var gbfs = (evaluateAlgorithms.GBFS)? GetEvaluationData<Dijkstra>(start, goal) : null;
        var jps = (evaluateAlgorithms.JPS) ? GetEvaluationData<JPS>(start, goal) : null;
        var dijkstra = (evaluateAlgorithms.Dijkstra) ? GetEvaluationData<Dijkstra>(start, goal) : null;

        // ILS algorithms
        var ilsWithAStar = (evaluateAlgorithms.ILSAStar) ? GetILSEvaluationData<AStar>(start, goal) : null;
        var ilsWithGBFS = (evaluateAlgorithms.ILSGBFS) ? GetILSEvaluationData<GBFS>(start, goal) : null;
        var ilsWithDijkstra = (evaluateAlgorithms.ILSDijkstra) ? GetILSEvaluationData<Dijkstra>(start, goal) : null;

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

    private EvaluationData GetEvaluationData<T>(Node start, Node end) where T : INavigate
    {
        var result = RunAlgorithm<T>(start, end);
        return EvaluationResult.FromPathResult(result);
    }

    private EvaluationData GetILSEvaluationData<T>(Node start, Node end) where T : INavigate
    {
        var result = RunILSAlgorithm<T>(start, end);
        return EvaluationResult.FromPathResult(result);
    }

    private PathResult RunAlgorithm<T>(Node start, Node end) where T: INavigate
    {
        return pathManager.RunAlgorithm<T>(start, end);
    }
    
    private PathResult RunILSAlgorithm<T>(Node start, Node end) where T: INavigate
    {
        return pathManager.RunILSWith<T>(start, end);
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

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

        var (start, goal) = grid.GetStartEndNodes();
        // Evaluate the algorithms
        StartEvaluation(evalSize, start, goal, evaluateAlgorithms);

        // Post evaluation
        if (evaluationResults.Count <= 0) return null;

        return evaluationResults[evalSize - 1];
    }

    public void ClearResults()
    {
        if (evaluationResults is { Count: > 0 })
            evaluationResults.Clear();
    }

    private void StartEvaluation(
        int evalSize, 
        Node start, 
        Node goal, 
        EvaluateAlgorithms evaluateAlgorithms)
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
        return new EvaluationResult
        {
            AStar = (evaluateAlgorithms.AStar) ? GetEvaluationData<AStar>(start, goal) : null,
            GBFS = (evaluateAlgorithms.GBFS) ? GetEvaluationData<GBFS>(start, goal) : null,
            JPS = (evaluateAlgorithms.JPS) ? GetEvaluationData<JPS>(start, goal) : null,
            Dijkstra = (evaluateAlgorithms.Dijkstra) ? GetEvaluationData<Dijkstra>(start, goal) : null,
            ILSWithAStar = (evaluateAlgorithms.ILSAStar) ? GetEvaluationData<AStar>(start, goal, true) : null,
            ILSWithDijkstra = (evaluateAlgorithms.ILSDijkstra) ? GetEvaluationData<Dijkstra>(start, goal, true) : null,
            ILSWithGBFS = (evaluateAlgorithms.ILSGBFS) ? GetEvaluationData<GBFS>(start, goal, true) : null,
        };
    }

    private EvaluationData GetEvaluationData<T>(Node start, Node end, bool ils = false) where T : INavigate
        => ils 
            ? Evaluator(
                start, 
                end, 
                (start, end) => pathManager.RunILSWith<T>(start, end)) 
            : Evaluator(
                start, 
                end, 
                (start, end) => pathManager.RunAlgorithm<T>(start, end));

    private EvaluationData Evaluator(
        Node start,
        Node end,
        Func<Node, Node, PathResult> fn)
    {
        var (results, stats) = Stats.RecordStats(() => fn(start, end));

        return new EvaluationData
        {
            PathLength = results?.PathLength ?? 0,
            PathCost = results?.PathCost ?? 0f,
            VisitedNodes = results?.VisitedNodes ?? 0,
            CorridorIterations = results?.CorridorIterations ?? 0,

            TimeTaken = stats.TimeTaken,
            MeasuredTimeMs = stats.MeasuredTimeMs,
            MemoryUsedBytes = stats.MemoryUsedBytes,
        };
    }
}

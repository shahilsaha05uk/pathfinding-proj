using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PathfindingEvaluator : MonoBehaviour
{
    [SerializeField] private Grid3D grid;
    [SerializeField] private PathfindingManager pathManager;
    private readonly List<EvaluationResult> evaluationResults = new();

    public List<EvaluationResult> GetEvaluationResults() => evaluationResults;

    public EvaluationResult Evaluate(int evalSize, HashSet<AlgorithmType> algorithms)
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
        StartEvaluation(evalSize, start, goal, algorithms);

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
        HashSet<AlgorithmType> algorithms)
    {
        int count = 0;

        while (count < evalSize)
        {
            var results = GatherEvaluationData(start, goal, algorithms);
            evaluationResults.Add(results);
            count++;
        }
    }

    private EvaluationResult GatherEvaluationData(
        Node start, 
        Node goal,
        HashSet<AlgorithmType> algorithms)
    {
        var result = new EvaluationResult();

        foreach (var type in algorithms)
        {
            if (!IsSupported(type))
            {
                Debug.LogWarning($"Algorithm {type} is not supported for evaluation.");
                continue;
            }

            var data = GetEvaluationData(type, start, goal);
            result.AddResult(type, data);
        }

        return result;
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

    private bool IsSupported(AlgorithmType type)
    {
        return type == AlgorithmType.AStar
            || type == AlgorithmType.GBFS
            || type == AlgorithmType.Dijkstra
            || type == AlgorithmType.JPS
            || type == AlgorithmType.ILS_AStar
            || type == AlgorithmType.ILS_GBFS
            || type == AlgorithmType.ILS_Dijkstra;
    }

    private EvaluationData GetEvaluationData(AlgorithmType type, Node start, Node end)
    {
        return type switch
        {
            AlgorithmType.AStar => Evaluator(start, end, (s, e) => pathManager.RunAlgorithm<AStar>(s, e)),
            AlgorithmType.GBFS => Evaluator(start, end, (s, e) => pathManager.RunAlgorithm<GBFS>(s, e)),
            AlgorithmType.Dijkstra => Evaluator(start, end, (s, e) => pathManager.RunAlgorithm<Dijkstra>(s, e)),
            AlgorithmType.JPS => Evaluator(start, end, (s, e) => pathManager.RunAlgorithm<JPS>(s, e)),
            AlgorithmType.ILS_AStar => Evaluator(start, end, (s, e) => pathManager.RunILSWith<AStar>(s, e)),
            AlgorithmType.ILS_GBFS => Evaluator(start, end, (s, e) => pathManager.RunILSWith<GBFS>(s, e)),
            AlgorithmType.ILS_Dijkstra => Evaluator(start, end, (s, e) => pathManager.RunILSWith<Dijkstra>(s, e)),
            _ => new EvaluationData { Message = $"Unsupported algorithm: {type}" },
        };
    }
}

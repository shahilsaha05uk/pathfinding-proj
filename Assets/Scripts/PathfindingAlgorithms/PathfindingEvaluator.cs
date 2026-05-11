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

    public void AddEvaluationResult(EvaluationResult result)
    {
        if (result != null)
            evaluationResults.Add(result);
    }

    public void ClearResults()
    {
        if (evaluationResults.Count > 0)
            evaluationResults.Clear();
    }

    // keep your existing Evaluate(...) if you still need it for the old flow

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

    private EvaluationData Evaluator(
        Node start,
        Node end,
        Func<Node, Node, PathResult> fn)
    {
        var (results, stats) = Stats.RecordStats(() => fn(start, end));

        return EvaluationResult.FromPathResult(results, stats);
    }
}

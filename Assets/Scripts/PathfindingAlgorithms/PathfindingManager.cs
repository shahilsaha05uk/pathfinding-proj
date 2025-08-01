using System;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    [SerializeField] private Grid3D grid;

    [SerializeField] private AStar aStar;
    [SerializeField] private GBFS gbfs;
    [SerializeField] private Dijkstra dijkstra;
    [SerializeField] private ILS ils;
    [SerializeField] private JPS jps;

    private Dictionary<Type, INavigate> algorithms;
    private Dictionary<AlgorithmType, Func<Node, Node, PathResult>> runners;

    private void Awake()
    {
        algorithms = new Dictionary<Type, INavigate>
        {
            { typeof(AStar), aStar },
            { typeof(GBFS), gbfs },
            { typeof(Dijkstra), dijkstra },
            { typeof(JPS), jps },
        };

        runners = new Dictionary<AlgorithmType, Func<Node, Node, PathResult>>
        {
            { AlgorithmType.AStar,        (s,e) => RunAlgorithm<AStar>(s, e) },
            { AlgorithmType.GBFS,         (s,e) => RunAlgorithm<GBFS>(s, e) },
            { AlgorithmType.Dijkstra,     (s,e) => RunAlgorithm<Dijkstra>(s, e) },
            { AlgorithmType.JPS,          (s,e) => RunAlgorithm<JPS>(s, e) },
            { AlgorithmType.ILS_AStar,    (s,e) => RunILSWith<AStar>(s, e) },
            { AlgorithmType.ILS_GBFS,     (s,e) => RunILSWith<GBFS>(s, e) },
            { AlgorithmType.ILS_Dijkstra, (s,e) => RunILSWith<Dijkstra>(s, e) },
        };
    }

    public PathResult RunAlgorithm<T>(Node start, Node end) where T: INavigate
    {
        if (!algorithms.TryGetValue(typeof(T), out var algo) || algo == null)
        {
            Debug.LogError($"Algorithm {typeof(T).Name} is not registered on {nameof(PathfindingManager)}.");
            return null;
        }

        return algo.Navigate(start, end);
    }

    public PathResult RunILSWith<T>(Node start, Node end, int corridorWidth = 10) where T : INavigate
    {
        if (!algorithms.TryGetValue(typeof(T), out var algo) || algo == null)
        {
            Debug.LogError($"Inner algorithm {typeof(T).Name} is not registered on {nameof(PathfindingManager)}.");
            return null;
        }

        return ils.Navigate(grid, start, end, corridorWidth, algo);
    }

    public PathResult RunAlgorithm(AlgorithmType type, Node start, Node end)
    {
        if (!runners.TryGetValue(type, out var run))
        {
            Debug.LogWarning($"AlgorithmType '{type}' not found. Falling back to A*.");
            run = runners[AlgorithmType.AStar];
        }
        return run(start, end);
    }
}
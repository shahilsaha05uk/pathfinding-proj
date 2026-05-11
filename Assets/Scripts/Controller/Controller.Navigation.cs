using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class Controller
{
    public float PathAnimationDelay = 0.1f;
    public SO_EvaluatorConfig EvaluatorConfig;
    public SO_AlgorithmConfig AlgorithmConfig;
   
    public EvaluationData OnNavigate(
        AlgorithmType algorithmType,
        int corridorWidth,
        bool enableStats)
    {
        // Implementation for navigation logic
        grid.ResetPath();
        var (start, end) = grid.GetStartEndNodes();
        var (data, path) = EvaluateAlgorithm(algorithmType, start, end);

        if(data == null)
        {
            Debug.LogError("Evaluation data is null.");
            return null;
        }

        StartCoroutine(AnimatePath(path, PathAnimationDelay));
        return data;
    }
    
    public void OnEvaluate()
    {
        var config = EvaluatorConfig.ToConfig();
        StartCoroutine(RunAutoEvaluation(config));
    }

    private IEnumerator RunAutoEvaluation(AutoEvaluationConfig settings)
    {
        if (settings.BatchSize <= 0)
        {
            Debug.LogError("Batch size must be greater than 0.");
            yield break;
        }

        if (settings.Algorithms == null || settings.Algorithms.Count == 0)
        {
            Debug.LogError("No algorithms selected for auto evaluation.");
            yield break;
        }

        evaluator.ClearResults();

        // For every grid size
        foreach(var size in settings.GridSizes)
        {
            // Batch to run for each grid size and obstacle density
            // for each obstacle density
            foreach (var density in settings.ObstacleDensities)
            {
                for (int i = 0; i < settings.BatchSize; i++)
                {
                    Debug.Log($"Starting batch {i + 1} of {settings.BatchSize} for grid size {size}x{size}.");

                    var config = BuildRandomGridConfig(settings, size, density);
                    grid.Clear();
                    grid.Create(config);

                    var start = FindStartNodeAtOrigin();
                    var goal = FindFarthestAvailableNode(start);

                    if (start == null || goal == null)
                    {
                        Debug.LogWarning(
                            $"Skipping batch {settings.BatchSize + 1}: " +
                            $"start or goal node not found.");
                        grid.Clear();
                        continue;
                    }

                    grid.SetStartNode(start);
                    grid.SetEndNode(goal);

                    var batchResult = new EvaluationResult();
                    var offsetX = (config.OffsetX.min + config.OffsetX.max) / 2f;
                    var offsetY = (config.OffsetY.min + config.OffsetY.max) / 2f;

                    UpdateConfigPanel(new EvaluationLog(
                        batchSize: settings.BatchSize,
                        gridSize: config.GridSize,
                        obstacleDensity: config.ObstacleDensity,
                        noiseScale: config.NoiseScale,
                        offsets: (X: offsetX, Y: offsetY),
                        height: config.MaxHeight));

                    foreach (var algorithm in settings.Algorithms)
                    {
                        UpdateConfigPanelCompletedAlgorithmType(algorithm);
                        var (data, path) = EvaluateAlgorithm(algorithm, start, goal);

                        batchResult.AddResult(algorithm, data);

                        if (settings.Animate && path != null && path.Count > 0)
                        {
                            var algorithmColor = GetColorForAlgorithm(algorithm);
                            yield return AnimatePath(path, settings.AnimationDelay, algorithmColor);
                            grid.ResetPath();
                        }
                        else
                        {
                            yield return null;
                        }

                        yield return null;
                    }

                    evaluator.AddEvaluationResult(batchResult);

                    SaveToMemory(config, settings);

                    grid.ResetPath();
                    grid.Clear();
                }
            }
            Export(settings);
            evaluator.ClearResults();
        }
    }

    void SaveToMemory(GridConfig config, AutoEvaluationConfig settings)
    {
        saveManager.AddToMemory(
            settings,
            config,
            evaluator.GetEvaluationResults());
    }
    void Export(AutoEvaluationConfig settings)
    {
        var config = grid.GetGridConfig();
        saveManager.Export(config, settings);
    }

    private (EvaluationData data, List<Node> path) EvaluateAlgorithm(
        AlgorithmType type, 
        Node start, 
        Node goal)
    {
        PathResult result = null;

        var (_, stats) = Stats.RecordStats(() =>
        {
            result = type switch
            {
                AlgorithmType.AStar => pathfindingManager.RunAlgorithm<AStar>(start, goal),
                AlgorithmType.GBFS => pathfindingManager.RunAlgorithm<GBFS>(start, goal),
                AlgorithmType.Dijkstra => pathfindingManager.RunAlgorithm<Dijkstra>(start, goal),
                AlgorithmType.JPS => pathfindingManager.RunAlgorithm<JPS>(start, goal),
                AlgorithmType.ILS_AStar => pathfindingManager.RunILSWith<AStar>(start, goal),
                AlgorithmType.ILS_GBFS => pathfindingManager.RunILSWith<GBFS>(start, goal),
                AlgorithmType.ILS_Dijkstra => pathfindingManager.RunILSWith<Dijkstra>(start, goal),
                _ => null
            };

            return result;
        });
        
        //if(type == AlgorithmType.GBFS && stats.MemoryUsedBytes <=0 && result.Success)
        //{
        //    Debug.DebugBreak();
        //}

        return (EvaluationResult.FromPathResult(result, stats), result?.Path);
    }

    private IEnumerator AnimatePath(List<Node> path, float delay, Color pathColor = default)
    {
        if (path == null || path.Count == 0)
            yield break;

        grid.ResetPath();

        for (int i = 1; i < path.Count - 1; i++)
        {
            path[i].SetColor(pathColor);
            yield return new WaitForSeconds(delay);
        }
    }

    private Color GetColorForAlgorithm(AlgorithmType algorithm)
        => AlgorithmConfig.ColorKeys.TryGetValue(algorithm, out var color) 
        ? color 
        : Color.white;

    private GridConfig BuildRandomGridConfig(
        AutoEvaluationConfig settings, 
        int size, 
        float density)
    {
        return new GridConfig
        {
            GridSize = size,
            MaxHeight = (int)(DeviatedValue(settings.HeightRange, settings.HeightDeviation)),
            NoiseScale = DeviatedValue(settings.NoiseRange, settings.NoiseMultiplier),
            ObstacleDensity = DeviatedValue(density, settings.ObstacleDensityDeviation),
            OffsetX = (settings.OffsetXRange.Min, settings.OffsetXRange.Max),
            OffsetY = (settings.OffsetYRange.Min, settings.OffsetYRange.Max),
        };
    }

    private float DeviatedValue(float value, float deviation)
    {
        return Random.Range(value - deviation, value + deviation);
    }

    private float DeviatedValue(Vector2 value, float deviation)
    {
        return Random.Range(
            minInclusive: DeviatedValue(value.x, deviation), 
            maxInclusive: DeviatedValue(value.y, deviation));
    }

    private float DeviatedValue((float x, float y) value, float deviation)
    {
        return Random.Range(
            minInclusive: DeviatedValue(value.x, deviation), 
            maxInclusive: DeviatedValue(value.y, deviation));
    }

    private Node FindStartNodeAtOrigin()
    {
        if (!grid.IsInsideGrid(0, 0, 0))
            return null;

        var allNodes = grid.GetAllNodes();
        if (allNodes == null)
            return null;

        for (int y = 0; y < allNodes.GetLength(1); y++)
        {
            var node = grid.GetNodeAt(0, y, 0);
            if (node != null && !node.bIsBlocked)
                return node;
        }

        return null;
    }

    private Node FindFarthestAvailableNode(Node start)
    {
        if (start == null)
            return null;

        var allNodes = grid.GetAllNodes();
        if (allNodes == null)
            return null;

        Node farthest = null;
        float farthestDistance = float.MinValue;
        var startPos = start.GetNodePositionOnGrid();

        for (int x = 0; x < allNodes.GetLength(0); x++)
        {
            for (int y = 0; y < allNodes.GetLength(1); y++)
            {
                for (int z = 0; z < allNodes.GetLength(2); z++)
                {
                    var node = allNodes[x, y, z];
                    if (node == null || node.bIsBlocked || node == start)
                        continue;

                    Vector3Int delta = node.GetNodePositionOnGrid() - startPos;
                    float dist = delta.sqrMagnitude;

                    if (dist > farthestDistance)
                    {
                        farthestDistance = dist;
                        farthest = node;
                    }
                }
            }
        }

        return farthest;
    }
}
using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

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

        if (settings.DensityRanges == null || settings.DensityRanges.Count == 0)
        {
            Debug.LogError("No density ranges configured for evaluation.");
            yield break;
        }

        evaluator.ClearResults();

        // For every grid size
        var gridSizes = new List<int>(settings.GridSizes);
        GridConfig config = null;

        for(int gi = 0; gi < gridSizes.Count; gi++)
        {
            var gridSize = gridSizes[gi];
            
            // For every density range
            for (int di = 0; di < settings.DensityRanges.Count; di++)
            {
                var densityRange = settings.DensityRanges[di];
                
                // For every batch 
                for (int i = 0; i < settings.BatchSize; i++)
                {
                    UpdateBatchCount(i + 1);
                    Debug.Log($"Starting batch {i + 1} of {settings.BatchSize} for grid size {gridSize}x{gridSize}, density range {di + 1}.");

                    config = BuildRandomGridConfig(
                        gridSize,
                        settings, 
                        densityRange);
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

                    var offsetX = (config.OffsetX.min + config.OffsetX.max) / 2f;
                    var offsetY = (config.OffsetY.min + config.OffsetY.max) / 2f;

                    UpdateConfigPanel(new EvaluationLog(
                        batchSize: settings.BatchSize,
                        gridSize: config.GridSize,
                        obstacleSeed: config.ObstacleSeed,
                        noiseScale: config.NoiseScale,
                        offsets: (X: offsetX, Y: offsetY),
                        height: config.MaxHeight));

                    // Execute for every algorithm
                    var algoEvalResults = new EvaluationResult();
                    foreach (var algorithm in settings.Algorithms)
                    {
                        UpdateAlgorithmType(algorithm);
                        var (data, path) = EvaluateAlgorithm(algorithm, start, goal);

                        algoEvalResults.AddResult(algorithm, data);

                        HandleSaveAndExport(
                            settings.ExportOption == EExportType.EveryAlgorithm
                            || (settings.ExportOption == EExportType.EveryIteration
                            && saveManager.ResultDataCount >= settings.NumberOfIterations),
                            config,
                            settings);

                        if (settings.Animate && path != null && path.Count > 0)
                        {
                            var algorithmColor = GetColorForAlgorithm(algorithm);
                            yield return AnimatePath(path, settings.AnimationDelay, algorithmColor);
                            grid.ResetPath();
                        }

                        // Yield once per algorithm to prevent main thread lockup
                        // without doubling the artificial delay
                        yield return null;
                    }

                    evaluator.AddEvaluationResult(algoEvalResults);
                    HandleSaveAndExport(settings.ExportOption == EExportType.EveryBatch, config, settings);
                    grid.ResetPath();
                    grid.Clear();
                }

                // This will save and export after every density range
                HandleSaveAndExport(settings.ExportOption == EExportType.EveryGridSize, config, settings);
            }
        }

        // Final export: ensure all remaining data is saved and exported
        Debug.Log("Auto evaluation complete. Performing final data export...");
        if (evaluator.GetEvaluationResults().Count > 0)
        {
            SaveToMemory(config, settings);
            Export(settings);
            evaluator.ClearResults();
            Debug.Log("Final export completed successfully.");
        }
        else
        {
            Debug.Log("No remaining data to export.");
        }
    }

    void HandleSaveAndExport(bool condition, GridConfig config, AutoEvaluationConfig settings)
    {
        if (condition)
        {
            SaveToMemory(config, settings);
            Export(settings);
            evaluator.ClearResults();
        }
        else
        {
            SaveToMemory(config, settings);
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
        int gridSize, 
        AutoEvaluationConfig settings, 
        DensityRange densityRange)
    {
        // Get randomized density within the range
        var randomDensity = densityRange.GetRandomDensity();

        // Invert the density: lesser value = fewer obstacles
        float invertedDensity = DensityRange.InvertDensity(randomDensity);

        var density = (int)(randomDensity * settings.ObstacleSeedScale);
        return new GridConfig
        {
            GridSize = gridSize,
            MaxHeight = (int)(GridConfigHelper.DeviatedValue(
                settings.HeightRange, 
                settings.HeightDeviation)),
            NoiseScale = GridConfigHelper.DeviatedValue(
                settings.NoiseRange, 
                settings.NoiseMultiplier),
            ObstacleSeed = density,
            DensityThreshold = invertedDensity,
            OffsetX = (
                settings.OffsetXRange.Min, 
                settings.OffsetXRange.Max),
            OffsetY = (
                settings.OffsetYRange.Min, 
                settings.OffsetYRange.Max),
        };
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

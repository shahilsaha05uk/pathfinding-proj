using JetBrains.Annotations;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ConfigPanel : MonoBehaviour
{
    public Action<EvaluationLog> OnConfigChanged;
    public Action<AlgorithmType> OnAlgorithmComplete;

    public PanelLabel lblBatchSize;
    public PanelLabel lblCurrentAlgorithm;
    public PanelLabel lblCurrentGridSize;
    public PanelLabel lblCurrentObstacleDensity;
    public PanelLabel lblNoiseRange;
    public PanelLabel lblOffsets;
    public PanelLabel lblHeight;
    public PanelLabel lblAlgorithmsComplete;

    private HashSet<AlgorithmType> algorithmsComplete = new();

    public void Start()
    {
        OnConfigChanged += HandleConfigChanged;
        OnAlgorithmComplete += HandleAlgorithmComplete;
    }

    private void HandleAlgorithmComplete(AlgorithmType type)
    {
        var label = type switch
        {
            AlgorithmType.AStar => "A*",
            AlgorithmType.JPS => "JPS",
            AlgorithmType.GBFS => "GBFS",
            AlgorithmType.Dijkstra => "Dijkstra",
            AlgorithmType.ILS_Dijkstra => "ILS Dijkstra",
            AlgorithmType.ILS_AStar => "ILS A*",
            AlgorithmType.ILS_GBFS => "ILS GBFS",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        lblCurrentAlgorithm.SetValue(label);
        algorithmsComplete.Add(type);
        lblAlgorithmsComplete.SetValue(string.Join(", ", algorithmsComplete));
    }

    private void HandleConfigChanged(EvaluationLog config)
    {
        algorithmsComplete.Clear();
        lblBatchSize.SetValue(config.BatchSize.ToString());
        lblCurrentAlgorithm.SetValue("N/A");
        lblCurrentGridSize.SetValue(config.GridSize.ToString());
        lblCurrentObstacleDensity.SetValue(config.ObstacleDensity.ToString());
        lblNoiseRange.SetValue(config.NoiseScale.ToString());
        lblOffsets.SetValue($"({config.Offsets.X}, {config.Offsets.Y})");
        lblHeight.SetValue(config.Height.ToString());
    }
}

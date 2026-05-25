using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TogglerData
{
    public string Label;
    public bool IsOn;
    public Color ColorKey;
}

[CreateAssetMenu(fileName = "AlgorithmConfig", menuName = "AlgorithmConfig", order = 1)]
public class SO_AlgorithmConfig : ScriptableObject
{

    [SerializedDictionary("Type", "Data")]
    [SerializeField]
    private SerializedDictionary<AlgorithmType, TogglerData> Algorithms;

    public Dictionary<AlgorithmType, Color> ColorKeys 
        => Algorithms.ToDictionary(kv => kv.Key, kv => kv.Value.ColorKey);

    public SerializedDictionary<AlgorithmType, TogglerData> GetData() => Algorithms;

    public HashSet<AlgorithmType> FilterEnabledAlgorithms()
    {
        return Algorithms
            .Keys
            .Where(type => Algorithms[type].IsOn)
            .ToHashSet();
    }
}

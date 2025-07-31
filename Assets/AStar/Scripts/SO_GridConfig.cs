using UnityEngine;

[CreateAssetMenu(fileName = "GridConfig", menuName = "AStar/GridConfig", order = 1)]
public class SO_GridConfig : ScriptableObject
{
    [SerializeField] private int GridSize;
    [SerializeField] private int MaxHeight;
    [SerializeField] private float NoiseScale;
    [SerializeField] private float ObstacleDensity;

    [Space(5)]
    [SerializeField] private float MinOffsetX;
    [SerializeField] private float MaxOffsetX;
    [Space(2)]
    [SerializeField] private float MinOffsetY;
    [SerializeField] private float MaxOffsetY;
    [Space(2)]
    [SerializeField] private float MinOffsetZ;
    [SerializeField] private float MaxOffsetZ;

    [SerializeField] private Vector3Int StartNode;
    [SerializeField] private Vector3Int EndNode;

    [SerializeField] private AlgorithmType AlgorithmType;

    public GridConfig GetConfig() => new GridConfig
    {
        GridSize = GridSize,
        MaxHeight = MaxHeight,
        NoiseScale = NoiseScale,
        ObstacleDensity = ObstacleDensity,
        OffsetX = (MinOffsetX, MaxOffsetX),
        OffsetY = (MinOffsetY, MaxOffsetY),
    };
    
    public (Vector3Int start, Vector3Int end) GetStartEndNodes()
    {
        return (StartNode, EndNode);
    }

    public AlgorithmType GetAlgorithmType() => AlgorithmType;
}

using UnityEngine;

[CreateAssetMenu(fileName = "GridColorConfig", menuName = "AStar/GridColorConfig", order = 1)]
public class SO_GridColor : ScriptableObject
{
    [SerializeField] private GridColor Color;

    public GridColor GetGridColor() => Color;
}

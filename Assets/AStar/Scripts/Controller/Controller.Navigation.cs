using System.Collections.Generic;
using UnityEngine;

public partial class Controller
{
    public void OnNavigateAStar()
    {
        var nodes = mGrid.GetStartEndNodes();
        var pathResult = pathfindingManager.RunAStar(nodes.start, nodes.end);
        SetNodePath(pathResult.Path);
    }
    public void OnNavigateILS(string maxCorridorWidth)
    {
        if(UIHelper.ValidateInputAsInt(maxCorridorWidth, out int corridorWidth))
        {
            var nodes = mGrid.GetStartEndNodes();
            var pathResult = pathfindingManager.RunILS(nodes.start, nodes.end, corridorWidth, PathfindingAlgorithm.AStar);
            SetNodePath(pathResult.Path);
            return;
        }
        Debug.LogWarning("Invalid corridor width input.");
    }
    public void OnNavigateGBFS()
    {
        var nodes = mGrid.GetStartEndNodes();
        var pathResult = GBFS.Navigate(nodes.start, nodes.end);
        SetNodePath(pathResult.Path);
    }
    
    public EvaluationResult OnEvaluate() => evaluator.Evaluate();

    private void SetNodePath(List<Node> path)
    {
        if(path != null)
            mGrid.SetPathNode(path);
    }
}
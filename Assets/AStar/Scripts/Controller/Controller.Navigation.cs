using System.Collections.Generic;
using UnityEngine;

public partial class Controller
{
    public void OnNavigateAStar()
    {
        var nodes = mGrid.GetStartEndNodes();
        var path = pathfindingManager.RunAStar(nodes.start, nodes.end);
        SetNodePath(path);
    }
    public void OnNavigateILS(string maxCorridorWidth)
    {
        if(UIHelper.ValidateInputAsInt(maxCorridorWidth, out int corridorWidth))
        {
            var nodes = mGrid.GetStartEndNodes();
            var path = pathfindingManager.RunILS(nodes.start, nodes.end, corridorWidth);
            SetNodePath(path);
            return;
        }
        Debug.LogWarning("Invalid corridor width input.");
    }
    public void OnNavigateGBFS()
    {
        var nodes = mGrid.GetStartEndNodes();
        var path = GBFS.Navigate(nodes.start, nodes.end);
        SetNodePath(path);
    }
    
    public EvaluationResult OnEvaluate() => evaluator.Evaluate();

    private void SetNodePath(List<Node> path)
    {
        if(path != null)
            mGrid.SetPathNode(path);
    }
}
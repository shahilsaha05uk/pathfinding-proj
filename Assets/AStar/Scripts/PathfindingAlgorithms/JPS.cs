using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class JPS : BasePathfinding
{
    protected override PathResult FindPath(Node start, Node goal, HashSet<Node> allowedNodes = null)
    {
        // Initialise the data: openlist, camfrom and gCost  
        var openList = new PriorityQueue<Node, float>();

        // set up the start node  

        // initialise the start node  

        // add the start node to the open list  

        // while the open list is not empty  

        // get the current node and check if the goal is reached  

        // if the goal is reached, reconstruct the path and return the result  

        // get all the neighbors of the current node  

        // for each of the neighbor:  

        // Jump:  

        return base.FindPath(start, goal, allowedNodes);
    }

    private void Jump(Node current, Vector3Int direction)
    {
        //  
    }
}

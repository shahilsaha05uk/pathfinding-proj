using System.Collections.Generic;

public interface INavigate
{
    public PathResult Navigate(Node start, Node end, HashSet<Node> allowedNodes = null, bool trackStats = true);
}


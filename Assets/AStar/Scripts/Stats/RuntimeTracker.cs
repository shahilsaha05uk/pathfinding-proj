
using NUnit.Framework;
using System.Collections.Generic;

public static class RuntimeTracker
{
    public static int NodesCreated = 0;
    public static int PeakOpenListSize = 0;
    public static int PeakClosedListSize = 0;


    public static void Reset()
    {
        NodesCreated = 0;
        PeakOpenListSize = 0;
        PeakClosedListSize = 0;
    }

    public static void TrackOpenList(List<Node> openList)
    {
        if (openList.Count > PeakOpenListSize)
            PeakOpenListSize = openList.Count;
    }

    public static void TrackClosedList(HashSet<Node> closedList)
    {
        if (closedList.Count > PeakClosedListSize)
            PeakClosedListSize = closedList.Count;
    }
}

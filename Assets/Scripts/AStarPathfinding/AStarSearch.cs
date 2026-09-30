using System;
using System.Collections.Generic;

namespace AStarPathfinding
{
    // Pure A* search. Knows nothing about hex/square grids or Unity - callers supply
    // topology and costs via IAStarGrid<TPosition>. The game's hex grid is adapted in
    // Application/Pathfinding/Pathfinding.cs.
    public static class AStarSearch
    {
        private class NodeRecord<TPosition>
        {
            public int GCost = int.MaxValue;
            public int HCost;
            public int FCost => GCost + HCost;
            public TPosition CameFrom;
            public bool HasCameFrom;
        }

        public static List<TPosition> FindPath<TPosition>(IAStarGrid<TPosition> grid, TPosition start, TPosition end, out int pathLength)
            where TPosition : IEquatable<TPosition>
        {
            Dictionary<TPosition, NodeRecord<TPosition>> records = new Dictionary<TPosition, NodeRecord<TPosition>>();
            List<TPosition> openList = new List<TPosition>();
            HashSet<TPosition> closedSet = new HashSet<TPosition>();

            NodeRecord<TPosition> GetRecord(TPosition position)
            {
                if (!records.TryGetValue(position, out NodeRecord<TPosition> record))
                {
                    record = new NodeRecord<TPosition>();
                    records[position] = record;
                }
                return record;
            }

            NodeRecord<TPosition> startRecord = GetRecord(start);
            startRecord.GCost = 0;
            startRecord.HCost = grid.GetHeuristicCost(start, end);
            openList.Add(start);

            while (openList.Count > 0)
            {
                TPosition current = GetLowestFCostPosition(openList, records);

                if (current.Equals(end))
                {
                    pathLength = records[end].FCost;
                    return BuildPath(records, end);
                }

                openList.Remove(current);
                closedSet.Add(current);

                foreach (TPosition neighbour in grid.GetNeighbours(current))
                {
                    if (closedSet.Contains(neighbour))
                    {
                        continue;
                    }
                    if (!grid.IsWalkable(neighbour))
                    {
                        closedSet.Add(neighbour);
                        continue;
                    }

                    NodeRecord<TPosition> neighbourRecord = GetRecord(neighbour);
                    int tentativeGCost = records[current].GCost + grid.GetMoveCost(current, neighbour);
                    if (tentativeGCost < neighbourRecord.GCost)
                    {
                        neighbourRecord.CameFrom = current;
                        neighbourRecord.HasCameFrom = true;
                        neighbourRecord.GCost = tentativeGCost;
                        neighbourRecord.HCost = grid.GetHeuristicCost(neighbour, end);

                        if (!openList.Contains(neighbour))
                        {
                            openList.Add(neighbour);
                        }
                    }
                }
            }

            // no path found
            pathLength = 0;
            return null;
        }

        private static TPosition GetLowestFCostPosition<TPosition>(List<TPosition> openList, Dictionary<TPosition, NodeRecord<TPosition>> records)
            where TPosition : IEquatable<TPosition>
        {
            TPosition lowest = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (records[openList[i]].FCost < records[lowest].FCost)
                {
                    lowest = openList[i];
                }
            }
            return lowest;
        }

        private static List<TPosition> BuildPath<TPosition>(Dictionary<TPosition, NodeRecord<TPosition>> records, TPosition end)
            where TPosition : IEquatable<TPosition>
        {
            List<TPosition> path = new List<TPosition> { end };
            TPosition current = end;

            while (records[current].HasCameFrom)
            {
                current = records[current].CameFrom;
                path.Add(current);
            }

            path.Reverse();
            return path;
        }
    }
}

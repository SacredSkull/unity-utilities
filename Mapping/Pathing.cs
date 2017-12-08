using System;
using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using UnityEngine;
using UnityUtilities.Collections;
using UnityUtilities.Collections.Grid;

namespace UnityUtilities.Mapping
{
    public class Pathing
    {
        public static int SnakeDistance(Vector2 start, Vector2 finish) {
            return (int)(Math.Abs(start.x - finish.x) + Math.Abs(start.y - finish.y));
        }
        
        public static int SnakeDistance(IGridLocator one, IGridLocator two) {
            return Pathing.SnakeDistance(one.GetPosition(), two.GetPosition());
        }
        
        public static KeyValuePair<bool, IList<Vector2>> AStarMovement(Vector2 startPos, Vector2 destinationPos, Vector2[] whitelist, IGridGraph Graph) {
            SimplePriorityQueue<Vector2> frontier = new SimplePriorityQueue<Vector2>();
            frontier.Enqueue(startPos, 1);

            Dictionary<Vector2, Vector2> cameFromNode = new Dictionary<Vector2, Vector2>();
            Dictionary<Vector2, int> costSoFar = new Dictionary<Vector2, int>();
            cameFromNode[startPos] = startPos;
            costSoFar[startPos] = 0;

            bool pathable = false;
            Vector2 currentPos = new Vector2();

            while (frontier.Count > 0) {
                currentPos = frontier.Dequeue();
                //Logger.UnityLog($"[AI][PATHING] Visiting [{0},{1}]", currentPos.x, currentPos.y);

                // "Early exit" and check if the start and finish are identical
                if (currentPos.Equals(destinationPos)) {
                    break;
                }

                foreach (Vector2 nextPos in Graph.Neighbours(currentPos, whitelist)) {
                    pathable = true;
                    int newCost = costSoFar[currentPos] + Graph.Cost(nextPos);
                    if (!costSoFar.ContainsKey(nextPos) || newCost < costSoFar[nextPos]) {
                        costSoFar[nextPos] = newCost;
                        int priority = newCost + SnakeDistance(nextPos, destinationPos);

                        cameFromNode[nextPos] = currentPos;
                        if (frontier.Contains(nextPos))
                            frontier.UpdatePriority(nextPos, priority);
                        else
                            frontier.Enqueue(nextPos, priority);
                    }
                }
            }

            List<Vector2> path = new List<Vector2> {currentPos};

            // Add the final vector...

            // Unravel shortest path
            while (currentPos != startPos) {
                if (!cameFromNode.ContainsKey(currentPos) && currentPos != destinationPos)
                    break;

                path.Add(cameFromNode[currentPos]);
                currentPos = cameFromNode[currentPos];
            }
            
            return new KeyValuePair<bool, IList<Vector2>>(pathable, preparePath(path, startPos));
        }
        
        public static IList<Vector2> AllPathsOf(Vector2 startingPos, Vector2 destination, Vector2[] whitelist, IGridGraph Graph) {
            SortedList<int, IList<Vector2>> viablePaths = new SortedList<int, IList<Vector2>>(new DuplicateKeyComparer<int>());
            SortedList<int, IList<Vector2>> backupPaths = new SortedList<int, IList<Vector2>>(new DuplicateKeyComparer<int>());
            foreach(Vector2 neighbour in Graph.Neighbours(destination)) {
                KeyValuePair<bool, IList<Vector2>> result = AStarMovement(startingPos, neighbour, whitelist, Graph);
                if(result.Key)
                    viablePaths.Add(result.Value.Count, result.Value);
                else
                    backupPaths.Add(result.Value.Count, result.Value);
            }

            if(viablePaths.Count != 0) {
                //Logger.UnityLog($"Shortest ideal path is {viablePaths.First().Value.Count} long");
                return viablePaths.First().Value;
            }
            return backupPaths.Count != 0 ? backupPaths.First().Value : new List<Vector2>();
        }

        protected static IList<Vector2> preparePath(List<Vector2> path, Vector2 startPos) {
            // We don't need/want the starting position.
            path.Remove(startPos);
            path.Reverse();
            return path;
        }
    }
}
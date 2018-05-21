using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;
using UnityUtilities.Collections.Grid;

namespace UnityUtilities.Mapping.Algorithms
{
    public class AStar : Pathing
    {
        public AStar(IGridGraph _graph) : base(_graph) { }
        
        public override KeyValuePair<bool, IList<Vector2>> ComputePaths(Vector2 startPos, Vector2 destinationPos) {
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

                // "Early exit" and check if the start and finish are identical
                if (currentPos.Equals(destinationPos)) {
                    break;
                }

                foreach (Vector2 nextPos in graph.Neighbours(currentPos, UnravelPassingFilters(_passingFilters), UnravelBlockingFilters(_blockingFilters))) {
                    pathable = true;
                    int newCost = costSoFar[currentPos] + graph.Cost(nextPos);
                    if (!costSoFar.ContainsKey(nextPos) || newCost < costSoFar[nextPos]) {
                        costSoFar[nextPos] = newCost;
                        int priority = newCost + Pathing.SnakeDistance(nextPos, destinationPos);

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
        
        protected IList<Vector2> preparePath(List<Vector2> path, Vector2 startPos) {
            // We don't need/want the starting position.
            path.Remove(startPos);
            path.Reverse();
            return path;
        }
    }
}
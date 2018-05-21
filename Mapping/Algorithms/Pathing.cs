using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtilities.Collections;
using UnityUtilities.Collections.Grid;

namespace UnityUtilities.Mapping.Algorithms {
    public abstract class Pathing {
        protected IGridGraph graph;
        protected PassingFilter[] _passingFilters;
        protected BlockingFilter[] _blockingFilters;
        
        public static Vector2[] UnravelPassingFilters(PassingFilter[] passing) => passing.SelectMany(x => x.GetVectors()).ToArray();

        public static Vector2[] UnravelBlockingFilters(BlockingFilter[] blocking) => blocking.SelectMany(x => x.GetVectors()).ToArray();
        
        public static int SnakeDistance(Vector2 start, Vector2 finish) {
            return (int)(Math.Abs(start.x - finish.x) + Math.Abs(start.y - finish.y));
        }
        
        public static int SnakeDistance(IGridLocator one, IGridLocator two) {
            return Pathing.SnakeDistance(one.GetPosition(), two.GetPosition());
        }

        public Vector2[] PathableRadius(Vector2 startPos, int radius) {
            Queue<Vector2> frontier = new Queue<Vector2>(graph.CountNodes());
            List<Vector2> pathableList = new List<Vector2>();
            frontier.Enqueue(startPos);
            
            while (frontier.Count > 0) {
                foreach (Vector2 target in graph.Neighbours(frontier.Dequeue())) {
                    if (SnakeDistance(startPos, target) > radius) continue;
                    if (pathableList.Contains(target)) continue;
                    frontier.Enqueue(target);
                    pathableList.Add(target);
                }
            }

            return pathableList.ToArray();
        }

        public Pathing(IGridGraph _graph) {
            graph = _graph;
        }

        public void AddPassingFilters(params PassingFilter[] passingFilters) {
            _passingFilters = _passingFilters.Concat(passingFilters).ToArray();
        }
        
        public void AddBlockingFilters(params BlockingFilter[] blockingFilters) {
            _blockingFilters = _blockingFilters.Concat(blockingFilters).ToArray();
        }
        
        public void SetPassingFilters(params PassingFilter[] passingFilters) {
            _passingFilters = passingFilters;
        }

        public void SetBlockingFilters(params BlockingFilter[] blockingFilters) {
            _blockingFilters = blockingFilters;
        }
        
        public abstract KeyValuePair<bool, IList<Vector2>> ComputePaths(Vector2 startPos, Vector2 destinationPos);
        
        public virtual IList<Vector2> AllPathsOf(Vector2 startingPos, Vector2 destination) {
            SortedList<int, IList<Vector2>> viablePaths = new SortedList<int, IList<Vector2>>(new DuplicateKeyComparer<int>());
            SortedList<int, IList<Vector2>> backupPaths = new SortedList<int, IList<Vector2>>(new DuplicateKeyComparer<int>());
            foreach(Vector2 neighbour in graph.Neighbours(destination)) {
                KeyValuePair<bool, IList<Vector2>> result = ComputePaths(startingPos, neighbour);
                if(result.Key)
                    viablePaths.Add(result.Value.Count, result.Value);
                else
                    backupPaths.Add(result.Value.Count, result.Value);
            }

            if(viablePaths.Count != 0) {
                return viablePaths.First().Value;
            }
            return backupPaths.Count != 0 ? backupPaths.First().Value : new List<Vector2>();
        }
    }
}
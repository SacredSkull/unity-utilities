using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityUtilities.Collections.Grid {
    public interface INodeValidator {
        bool isGeometryPathable(int x, int y);
        bool isGeometryPathable(Vector2 coord);
    }

    public class GridGraph<T> : INodeValidator, IGridGraph where T : IGridLocator {
        readonly Vector2 DIR_DOWN = new Vector2(-1, 0);
        readonly Vector2 DIR_UP = new Vector2(1, 0);
        readonly Vector2 DIR_LEFT = new Vector2(0, -1);
        readonly Vector2 DIR_RIGHT = new Vector2(0, 1);

        public LayeredGrid<T> LayeredGrid { get; set; }

        private GridCollection<T> GeometryGrid;
        private GridCollection<T> EntityGrid;
        private HashSet<int> ValidPathIDs;
        private HashSet<int> ValidEntityIDs;

        public GridGraph(LayeredGrid<T> layered, HashSet<int> validPathIDs, HashSet<int> validEntityIDs) {
            LayeredGrid = layered;
            GeometryGrid = layered.GetLayer("geometry");
            EntityGrid = layered.GetLayer("entity");
            ValidPathIDs = validPathIDs;
            ValidEntityIDs = validEntityIDs;

            if (GeometryGrid == null)
                throw new ArgumentNullException("layered", "The geometry grid is null");
            if (EntityGrid == null)
                throw new ArgumentNullException("layered", "The entity grid is null");
        }

        public int CountNodes() {
            return (int)LayeredGrid.Average(x => x.Area);
        }

        public bool isGeometryPathable(int x, int y) {
            GridPiece<T> geomePiece = GeometryGrid.Get(x, y);
            GridPiece<T> entPiece = EntityGrid.Get(x, y);

            if (geomePiece == null || entPiece == null)
                return false;

            return ValidPathIDs.Contains(geomePiece.ID);
        }

        public bool isGeometryPathable(Vector2 coord) {
            return isGeometryPathable((int)coord.x, (int)coord.y);
        }

        public int Cost(Vector2 vect) {
            return GeometryGrid.Get(vect).Cost;
        }

        public int Cost(int x, int y) {
            return Cost(new Vector2(x, y));
        }

        public List<Vector2> Neighbours(Vector2 originVector, Vector2[] whitelistedCoords, Vector2[] blacklistedCoords) {
            int x = (int)originVector.x;
            int y = (int)originVector.y;

            List<Vector2> edges = new List<Vector2>();

            if(whitelistedCoords == null)
                whitelistedCoords = new Vector2[0];
            if(blacklistedCoords == null)
                blacklistedCoords = new Vector2[0];

            // Anything present in both lists is removed, with priority going to the whitelist.
            Vector2[] overridenBlacklist = blacklistedCoords.Except(whitelistedCoords).ToArray();

            // Left
            if (CheckSide(originVector, DIR_LEFT, overridenBlacklist)) {
                edges.Add(originVector + DIR_LEFT);
            }

            // Up
            if (CheckSide(originVector, DIR_UP, overridenBlacklist)) {
                edges.Add(originVector + DIR_UP);
            }

            // Down
            if (CheckSide(originVector, DIR_DOWN, overridenBlacklist)) {
                edges.Add(originVector + DIR_DOWN);
            }

            // Right
            if (CheckSide(originVector, DIR_RIGHT, overridenBlacklist)) {
                edges.Add(originVector + DIR_RIGHT);
            }

            return edges;
        }

        // 1. The geometry must be able to contain the destination
        // 2. If the blacklist contains the vector, it is not pathable - unless it exists in the whitelist (whitelist priority is greater - see @Neighbours function).
        // 3. The geometry must have a floor of a passable kind. A null GridPiece will not work.
        private bool CheckSide(Vector2 originVector, Vector2 direction, IEnumerable<Vector2> blacklist) =>
            GeometryGrid.Contains(originVector + direction)
            && !blacklist.Contains(originVector + direction)
            && isGeometryPathable(originVector + direction);
    }
}
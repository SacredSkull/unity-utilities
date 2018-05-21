using System.Collections.Generic;
using UnityEngine;

namespace UnityUtilities.Collections.Grid {
    public interface IGridGraph {
        int Cost(Vector2 vect);
        int Cost(int x, int y);
        int CountNodes();
        bool isGeometryPathable(Vector2 coord);
        bool isGeometryPathable(int x, int y);
        List<Vector2> Neighbours(Vector2 originVector, Vector2[] WhitelistedCoords = null, Vector2[] BlacklistedCoords = null);
    }
}
using System;

namespace UnityUtilities.Collections.Grid {
    public class GridCollectionEventArgs<T> : EventArgs where T : IGridLocator {
        public GridPiece<T> GridPiece { get; private set; }

        public GridCollectionEventArgs(GridPiece<T> piece) {
            GridPiece = piece;
        }
    }
}
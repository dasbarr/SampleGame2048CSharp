using System;

namespace sample_game {
    
    /// <summary>
    /// Contains info about board tile movement.
    /// </summary>
    public class BoardTileMoveInfo {

        /// <summary>
        /// Tile with that coords was moved.
        /// </summary>
        public Tuple<int, int> tileStartCoords;
        
        /// <summary>
        /// Tile was moved to that coords.
        /// </summary>
        public Tuple<int, int> tileEndCoords;
    }
} // namespace sample_game
namespace SampleGame2048 {
    
    /// <summary>
    /// Represents a 2D index of a tile on the game board.
    /// </summary>
    public struct BoardTileIndex {

        /// <summary>
        /// Row index.
        /// </summary>
        public int I { get; }
        /// <summary>
        /// Column index.
        /// </summary>
        public int J { get; }

        public BoardTileIndex(int i, int j) {
            I = i;
            J = j;
        }
    }
}

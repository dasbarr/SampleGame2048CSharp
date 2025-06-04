namespace SampleGame2048 {
    
    /// <summary>
    /// Data that represents a single tile move during the game.
    /// </summary>
    public struct TileMoveData {

        /// <summary>
        /// Tile started its movement from that position on the board.
        /// </summary>
        public BoardTileIndex StartTileIndex { get; }
        /// <summary>
        /// Tile ended its movement at that position on the board.
        /// </summary>
        public BoardTileIndex EndTileIndex { get; }
        
        /// <summary>
        /// Player earns that number of score points as a result of the current tile move.
        /// </summary>
        public int Score { get; }

        public TileMoveData(BoardTileIndex startTileIndex, BoardTileIndex endTileIndex, int score) {
            StartTileIndex = startTileIndex;
            EndTileIndex = endTileIndex;
            Score = score;
        }
    }
}

namespace SampleGame2048 {
    
    /// <summary>
    /// Common interface for all game bots.
    /// </summary>
    public interface IBot {
        
        /// <summary>
        /// Calculates the next move based on the provided game state.
        /// </summary>
        /// <param name="gameBoardState">Current game board state.</param>
        /// <param name="availableMoves">All available moves for that state.</param>
        /// <returns>
        /// Next move (if a valid move can be made from the current game board state),
        /// null if a valid move can't be made.
        /// </returns>
        Move? CalcNextMove(ReadOnlyArray2D<int> gameBoardState, ReadOnlyHashSet<Move> availableMoves);
    }
}

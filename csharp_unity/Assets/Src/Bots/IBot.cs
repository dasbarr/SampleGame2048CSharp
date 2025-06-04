using System.Collections.Generic;

namespace sample_game {
    
    /// <summary>
    /// Common interface for all bots.
    /// </summary>
    public interface IBot {
        
        /// <summary>
        /// Calculates the next move based on the game state provided.
        /// </summary>
        /// <param name="gameBoardState">State of the game board.</param>
        /// <param name="availableMoves">Available moves for that state.</param>
        /// <returns>Next move from that game state.</returns>
        Move CalcNextMove(IReadOnlyList<IReadOnlyList<int>> gameBoardState, HashSet<Move> availableMoves);
    }
} // namespace sample_game
using System.Drawing;
using R3;

namespace SampleGame2048 {
    
    /// <summary>
    /// Game board interface for external use (outside of the game controller).
    /// </summary>
    public interface IGameBoard {
        
        /// <summary>
        /// Represents a current board state. Contains 'power-of-2' values for corresponding tiles (for example,
        /// it will contain 10 for the tile number 1024 (=2^10). Can contain a special value that represents empty tiles.
        /// First index is the row index, second one is the column index. (0, 0) is the top left tile.
        /// </summary>
        ReadOnlyArray2D<int> BoardState { get; }
        
        /// <summary>
        /// Size of the game board.
        /// </summary>
        Size BoardSize { get; }
        
        /// <summary>
        /// Contains all available moves for the current game board state.
        /// </summary>
        ReadOnlyHashSet<Move> AvailableMoves { get; }
        
        /// <summary>
        /// Data stream that contains changes on the game board.
        /// </summary>
        Observable<(BoardStateChangeType changeType, object additionalData)> BoardStateChanges { get; }
    }
}

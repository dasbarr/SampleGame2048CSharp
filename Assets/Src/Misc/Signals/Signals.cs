namespace SampleGame2048 {
    
    /// <summary>
    /// Fired when tile movement animation finishes.
    /// </summary>
    public class TileMovementFinishedSignal : ISignal {
        
        /// <summary>
        /// Total score earned by the current game move.
        /// </summary>
        public int TotalMoveScore { get; }

        public TileMovementFinishedSignal(int totalMoveScore) => TotalMoveScore = totalMoveScore;
    }
    
    /// <summary>
    /// Fired when game state changes.
    /// </summary>
    public class GameStateChangedSignal : ISignal {
        
        public GameState NewGameState { get; }

        public GameStateChangedSignal(GameState newGameState) => NewGameState = newGameState;
    }
}

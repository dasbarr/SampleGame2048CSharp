namespace SampleGame2048 {
    
    /// <summary>
    /// Represents a game state.
    /// </summary>
    public enum GameState {
        Uninitialized,      // game state before initialization
        NewGamePreparation, // preparing for the new game start
        WaitingForMove,     // game is waiting for the player/bot move
        PerformingMove,     // making an actual move on the game board
        GameTurnInProgress  // waiting for game turn end
    }
    
    /// <summary>
    /// Possible in-game moves.
    /// </summary>
    public enum Move {
        Up,
        Down,
        Left,
        Right
    }

    /// <summary>
    /// Represents different types of game board changes.
    /// </summary>
    public enum BoardStateChangeType {
        BoardCleared,
        NewTilesPlaced,
        TilesMoved
    }
}

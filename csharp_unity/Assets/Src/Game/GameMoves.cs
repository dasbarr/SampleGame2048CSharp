namespace sample_game {
    
    /// <summary>
    /// Possible in-game moves.
    /// </summary>
    public enum Move {
        IncorrectMove = 0, // auxiliary state, represents a move that cannot be performed
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }
} // namespace sample_game
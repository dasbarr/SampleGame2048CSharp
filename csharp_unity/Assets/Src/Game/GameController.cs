using System;
using sample_game.utils;

namespace sample_game {
    
    /// <summary>
    /// Holds main game logic.
    /// </summary>
    public class GameController : DependencyInjectable {
        
        //-------------------------------------------------------------
        // Nested
        //-------------------------------------------------------------
        
        /// <summary>
        /// Represents game state.
        /// </summary>
        public enum GameState {
            Uninitialized,      // game state before initialization
            NewGamePreparation, // preparing for the new game start
            WaitingForInput,    // game is waiting for the player/bot input
            GameTurnInProgress, // waiting for game turn end
            GameEnded           // game ended (win or lose)
        }
        
        //-------------------------------------------------------------
        // Dependencies
        //-------------------------------------------------------------
        
        [DependencyInjector.DependencyAttribute]
        private GameBoardProxy _gameBoardProxy = default;
        [DependencyInjector.DependencyAttribute]
        private ScoreProxy _scoreProxy = default;
        [DependencyInjector.DependencyAttribute]
        private InputManager _inputManager = default;
        [DependencyInjector.DependencyAttribute]
        private GameView _gameView = default;
        [DependencyInjector.DependencyAttribute]
        private GameConfig _gameConfig = default;
        
        //-------------------------------------------------------------
        // Class constants
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Class variables
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Class methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Constructor/destructor
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Variables
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
        public event Action GameStateChanged;
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------
        
        public GameState currentGameState { get; private set; } = GameState.Uninitialized;
        
        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------
        
        /// <summary>
        /// Interrupts current game and starts a new one.
        /// </summary>
        public void StartNewGame() {
            SetGameState(GameState.NewGamePreparation);

            _gameBoardProxy.Reset();
            _scoreProxy.ClearCurrentScore();

            SetGameState(GameState.WaitingForInput);
        }
        
        /// <summary>
        /// Makes a game move (if possible).
        /// </summary>
        /// <param name="move">Move to make.</param>
        /// <returns>True if move was successful, false otherwise.</returns>
        public bool MakeMove(Move move) {
            if (currentGameState == GameState.WaitingForInput) {
                // make a move
                var moveSuccess = _gameBoardProxy.MakeMove(move, out var moveScore);
                if (moveSuccess) {
                    SetGameState(GameState.GameTurnInProgress);
                
                    // update score
                    _scoreProxy.AddScore(moveScore);
                    return true;
                }
            }

            return false;
        }
        
        //-------------------------------------------------------------
        // Protected methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private methods
        //-------------------------------------------------------------
        
        /// <summary>
        /// Sets a new game state.
        /// </summary>
        /// <param name="newGameState">New game state to set.</param>
        private void SetGameState(GameState newGameState) {
            if (currentGameState == newGameState)
                return;

            currentGameState = newGameState;
            GameStateChanged?.Invoke();
        }

        /// <summary>
        /// Ends current turn, starts a new game turn (if possible) or ends the game.
        /// </summary>
        private void EndCurrentTurn() {
            if (_gameBoardProxy.winTileAcquired) {
                SetGameState(GameState.GameEnded);
            }
            else {
                // place new tiles before new turn
                _gameBoardProxy.PlaceRandomTiles(_gameConfig.numRandomTilesEachTurn);
                if (_gameBoardProxy.hasAvailableMoves) {
                    // start new turn
                    SetGameState(GameState.WaitingForInput);
                }
                else {
                    // no more moves on the game field
                    SetGameState(GameState.GameEnded);
                }
            }
        }
        
        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Handlers
        //-------------------------------------------------------------

        protected override void OnDependenciesFulfilled() {
            _inputManager.PlayedMoveInput += playerMove => MakeMove(playerMove);
            _gameView.TileMovementAnimationEnded += EndCurrentTurn;
            
            // start the game
            StartNewGame();
        }
    }
} // namespace sample_game
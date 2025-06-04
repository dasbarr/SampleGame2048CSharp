using System;
using sample_game.utils;

namespace sample_game {
    
    /// <summary>
    /// Controls a game bot.
    /// </summary>
    public class BotController : DependencyInjectable {
        
        //-------------------------------------------------------------
        // Nested
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Dependencies
        //-------------------------------------------------------------
        
        [DependencyInjector.DependencyAttribute]
        private GameController _gameController = default;
        [DependencyInjector.DependencyAttribute]
        private InputManager _inputManager = default;
        [DependencyInjector.DependencyAttribute]
        private GameBoardProxy _gameBoardProxy = default;
        
        //-------------------------------------------------------------
        // Class constants
        //-------------------------------------------------------------
        
        /// <summary>
        /// If bot can't make a valid move after that number of attempts, it should be stopped.
        /// </summary>
        private const uint sMaxMoveAttempts = 10;
        
        //-------------------------------------------------------------
        // Class variables
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Class methods
        //-------------------------------------------------------------

        /// <summary>
        /// Creates a new bot controller with a specific bot.
        /// </summary>
        /// <typeparam name="TBot">Bot type.</typeparam>
        /// <returns>Created bot controller.</returns>
        public static BotController Create<TBot>() where TBot : IBot, new() {
            return new BotController { _bot = new TBot() };
        }

        //-------------------------------------------------------------
        // Constructor/destructor
        //-------------------------------------------------------------

        private BotController() {
            // no implementation needed
        }

        //-------------------------------------------------------------
        // Variables
        //-------------------------------------------------------------

        private IBot _bot;

        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
        /// <summary>
        /// Dispatched when bot state (enabled or disabled) was changed.
        /// </summary>
        public event Action BotStateChanged;

        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------

        public bool botEnabled { get; private set; } = false;

        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------

        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------

        /// <summary>
        /// Toggles the bot.
        /// </summary>
        public void ToggleBot() {
            botEnabled = !botEnabled;

            // disable user input when bot is active
            _inputManager.playerInputAllowed = !botEnabled;
            
            if (botEnabled) {
                // subscribe to game state change to make moves
                _gameController.GameStateChanged += OnGameStateChanged;

                if (_gameController.currentGameState == GameController.GameState.WaitingForInput) {
                    // make 1st bot move
                    MakeMove();
                }
            }
            else {
                // unsubscribe
                _gameController.GameStateChanged -= OnGameStateChanged;
            }

            BotStateChanged?.Invoke();
        }
        
        //-------------------------------------------------------------
        // Protected methods
        //-------------------------------------------------------------

        //-------------------------------------------------------------
        // Private methods
        //-------------------------------------------------------------

        /// <summary>
        /// Makes a game move.
        /// </summary>
        private void MakeMove() {
            uint numAvailableMoveAttempts = sMaxMoveAttempts;
            while (numAvailableMoveAttempts > 0) {
                var successMove = 
                    _gameController.MakeMove(_bot.CalcNextMove(_gameBoardProxy.boardState, _gameBoardProxy.availableMoves));
                
                if (successMove)
                    return;

                numAvailableMoveAttempts--;
            }

            // if we are here, the bot wasn't able to make a move, so we should disable it
            ToggleBot();
        }
        
        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------

        //-------------------------------------------------------------
        // Handlers
        //-------------------------------------------------------------

        private void OnGameStateChanged() {
            if (!botEnabled)
                return;

            if (_gameController.currentGameState == GameController.GameState.NewGamePreparation ||
                _gameController.currentGameState == GameController.GameState.GameEnded
            ) {
                // game was ended or new game is about to start, in that case bot should be disabled
                ToggleBot();
            }
            else if (_gameController.currentGameState == GameController.GameState.WaitingForInput) {
                // make next bot move
                MakeMove();
            }
        }
    }
} // namespace sample_game
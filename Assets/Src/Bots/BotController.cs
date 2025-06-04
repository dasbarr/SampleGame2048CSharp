using System;
using R3;
using UnityEngine;

namespace SampleGame2048 {
    
    /// <summary>
    /// Controls a game bot.
    /// </summary>
    public class BotController {
        
        //-------------------------------------------------------------
        // Nested
        //-------------------------------------------------------------

        [Serializable]
        public class Settings {
            [Tooltip("If bot can't make a valid move after that number of attempts, it should be stopped")]
            [field: SerializeField]
            public int MaxMoveAttempts { get; private set; } = 10;
        }
        
        //-------------------------------------------------------------
        // Dependencies
        //-------------------------------------------------------------
        
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
        // Constructor/finalizer
        //-------------------------------------------------------------
        
        private BotController(Settings settings, GameController gameController, IGameBoard gameBoard,
                              PlayerInputManager playerInputManager, IBot bot
        ) {
            _settings = settings;
            _gameController = gameController;
            _gameBoard = gameBoard;
            
            _bot = bot;
            
            Enabled
                .Subscribe(x => {
                    // toggle player input (player should not be able to control the game when the bot is enabled)
                    playerInputManager.Active.Value = !x;

                    if (x && gameController.State.CurrentValue == GameState.WaitingForMove) {
                        // make first bot move
                        MakeBotMove();
                    }
                });

            // make a bot move when the game is in appropriate state
            gameController.State
                .Where(x => x == GameState.WaitingForMove && Enabled.Value)
                .Subscribe(_ => {
                    var success = MakeBotMove();
                    if (!success) {
                        // bot is not able to make a valid move, so disable it
                        Enabled.Value = false;
                    }
                });

            // disable bot on the new game start
            gameController.State
                .Where(x => x == GameState.NewGamePreparation)
                .Select(_ => false)
                .Subscribe(Enabled);
        }
        
        //-------------------------------------------------------------
        // Variables
        //-------------------------------------------------------------

        private readonly Settings _settings;
        private readonly GameController _gameController;
        private readonly IGameBoard _gameBoard;

        private readonly IBot _bot;

        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------
        
        public ReactiveProperty<bool> Enabled { get; } = new ReactiveProperty<bool>(false);
        
        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Protected methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private methods
        //-------------------------------------------------------------
        
        /// <summary>
        /// Makes a bot move.
        /// </summary>
        /// <returns>True if bot was able to make a valid move, false otherwise.</returns>
        private bool MakeBotMove() {
            var numAvailableMoveAttempts = _settings.MaxMoveAttempts;
            while (numAvailableMoveAttempts > 0) {
                numAvailableMoveAttempts--;
                
                var nextMove = _bot.CalcNextMove(_gameBoard.BoardState, _gameBoard.AvailableMoves);
                if (!nextMove.HasValue)
                    continue;

                var success = _gameController.MakeMove(nextMove.Value);
                if (success)
                    return true;
            }

            // no more move attempts
            return false;
        }
        
        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------
    }
}

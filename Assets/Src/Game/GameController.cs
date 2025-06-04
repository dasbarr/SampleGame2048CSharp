using System;
using System.Linq;
using System.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

namespace SampleGame2048 {
    
    /// <summary>
    /// Contains main game logic.
    /// </summary>
    public class GameController {
        
        //-------------------------------------------------------------
        // Nested
        //-------------------------------------------------------------

        [Serializable]
        public class Settings {
            [field: Tooltip("Number of random tiles that will be added to game board at the game start")]
            [field: SerializeField]
            public int NumInitialRandomTiles { get; private set; }
            
            [field: Tooltip("Number of random tiles that will be added to game board each turn (if possible)")]
            [field: SerializeField]
            public int NumRandomTilesEachTurn { get; private set; }
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

        public GameController(GeneralGameSettings generalSettings, Settings settings, SignalBus signalBus,
                              GameBoard gameBoard, PlayerInputManager playerInputManager, EndGameWindow endGameWindow
        ) {
            _generalSettings = generalSettings;
            _settings = settings;
            _gameBoard = gameBoard;
            _endGameWindow = endGameWindow;

            // expose readonly property
            State = _state.ToReadOnlyReactiveProperty();

            // make a move on player input
            playerInputManager.PlayerInput
                .Where(_ => _state.Value == GameState.WaitingForMove)
                .Subscribe(x => MakeMove(x));

            // proceed to the next move when tile movement will be completed
            signalBus.GetStream<TileMovementFinishedSignal>()
                .Where(_ => _state.Value == GameState.GameTurnInProgress)
                .Subscribe(x => _ = EndCurrentTurnAsync());
            
            // indicate game state changes for those who doesn't have direct access to the game controller
            State
                .Subscribe(x => signalBus.Fire(new GameStateChangedSignal(x)));
        }
        
        //-------------------------------------------------------------
        // Variables
        //-------------------------------------------------------------

        private readonly GeneralGameSettings _generalSettings;
        private readonly Settings _settings;
        
        private readonly GameBoard _gameBoard;
        private readonly EndGameWindow _endGameWindow;

        /// <summary>
        /// If true, player can continue the game even if it has the win tile (or greater tile) on the board
        /// (win window will not be shown again)
        /// </summary>
        private bool _gameContinuedAfterWin;
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------

        private readonly ReactiveProperty<GameState> _state = new ReactiveProperty<GameState>(GameState.Uninitialized);
        /// <summary>
        /// Current game state.
        /// </summary>
        public ReadOnlyReactiveProperty<GameState> State { get; }

        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------

        /// <summary>
        /// Interrupts current game and starts a new one.
        /// </summary>
        public void StartNewGame() {
            _state.Value = GameState.NewGamePreparation;

            _gameContinuedAfterWin = false;
            
            _gameBoard.Clear();
            _gameBoard.PlaceRandomTiles(_settings.NumInitialRandomTiles);

            _state.Value = GameState.WaitingForMove;
        }

        /// <summary>
        /// Makes a move (if possible).
        /// </summary>
        /// <param name="move">Move to make.</param>
        /// <returns>True if move was made, false otherwise.</returns>
        public bool MakeMove(Move move) {
            if (_state.Value != GameState.WaitingForMove)
                return false;
            
            _state.Value = GameState.PerformingMove;
            var moveSuccess = _gameBoard.MakeMove(move);

            _state.Value = moveSuccess
                ? GameState.GameTurnInProgress
                : GameState.WaitingForMove;

            return moveSuccess;
        }

        //-------------------------------------------------------------
        // Protected methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private methods
        //-------------------------------------------------------------

        private async Task EndCurrentTurnAsync() {
            if (!_gameContinuedAfterWin) {
                // check win condition first
                if (_gameBoard.MaxTileNumber >= _generalSettings.NumberOnWinTile) {
                    // show win window and ask if the player wants to continue the game or to start a new one
                    _gameContinuedAfterWin = await _endGameWindow.ShowAsync(true);
                    
                    if (!_gameContinuedAfterWin) {
                        StartNewGame();
                        return;
                    }
                }
            }
            
            // place new tiles
            _gameBoard.PlaceRandomTiles(_settings.NumRandomTilesEachTurn);

            if (_gameBoard.AvailableMoves.Any()) {
                // continue the game
                _state.Value = GameState.WaitingForMove;
            }
            else {
                // show lose window (the only option here is to start a new game, so discard the window show result)
                await _endGameWindow.ShowAsync(false);
                StartNewGame();
            }
        }
        
        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------
    }
}

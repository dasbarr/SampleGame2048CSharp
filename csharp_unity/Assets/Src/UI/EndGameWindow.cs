using sample_game.utils;
using UnityEngine;
using UnityEngine.UI;

namespace sample_game {
    
    /// <summary>
    /// Window that will be shown at the end of the game.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class EndGameWindow : DependencyInjectableUnity {
        
        //-------------------------------------------------------------
        // Nested
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Dependencies
        //-------------------------------------------------------------
        
        [DependencyInjector.DependencyAttribute]
        private GameController _gameController = default;
        [DependencyInjector.DependencyAttribute]
        private GameBoardProxy _gameBoardProxy = default;
        [DependencyInjector.DependencyAttribute]
        private ScoreProxy _scoreProxy = default;
        
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
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------

        private bool visible {
            set {
                var canvasGroup = GetComponent<CanvasGroup>();
                canvasGroup.alpha = value ? 1.0f : 0.0f;
                canvasGroup.interactable = value;
                canvasGroup.blocksRaycasts = value;
            }
        }
        
        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------

        [SerializeField]
        private AutoTranslator _headerLabel = default;

        [SerializeField]
        private Text _scoreLabel = default;
        [SerializeField]
        private GameObject _newRecordLabel = default;
        
        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Protected methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private methods
        //-------------------------------------------------------------

        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------

        protected override void Awake() {
            base.Awake();

            // that window should be hidden by default
            visible = false;
        }
        //-------------------------------------------------------------
        // Handlers
        //-------------------------------------------------------------
        
        protected override void OnDependenciesFulfilled() {
            _gameController.GameStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged() {
            if (_gameController.currentGameState != GameController.GameState.GameEnded)
                return;
            
            // prepare window
            _headerLabel.localizedStringId = _gameBoardProxy.winTileAcquired
                ? "EndGameWindowWin"
                : "EndGameWindowLose";
            _scoreLabel.text = _scoreProxy.currentScore.ToString();
            _newRecordLabel.SetActive(_scoreProxy.isNewRecord);
            
            // show window
            visible = true;
        }

        public void StartNewGameButtonHandler() {
            _gameController.StartNewGame();
            
            // hide window
            visible = false;
        }
    }
} // namespace sample_game
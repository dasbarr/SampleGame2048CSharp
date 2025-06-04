using sample_game.utils;
using UnityEngine;
using UnityEngine.UI;

namespace sample_game {
    
    /// <summary>
    /// Top UI line.
    /// </summary>
    public class TopUI : DependencyInjectableUnity {
        
        //-------------------------------------------------------------
        // Nested
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Dependencies
        //-------------------------------------------------------------
        
        [DependencyInjector.DependencyAttribute]
        private GameController _gameController = default;
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
        
        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------
        
        /// <summary>
        /// Label for current score.
        /// </summary>
        [SerializeField]
        private Text _currentScoreLabel = default;
        /// <summary>
        /// Label for best score.
        /// </summary>
        [SerializeField]
        private Text _bestScoreLabel = default;
        
        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Protected methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private methods
        //-------------------------------------------------------------

        private void UpdateScores() {
            _currentScoreLabel.text = _scoreProxy.currentScore.ToString();
            _bestScoreLabel.text = _scoreProxy.bestScore.ToString();
        }
        
        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Handlers
        //-------------------------------------------------------------

        protected override void OnDependenciesFulfilled() {
            _scoreProxy.ScoreChanged += UpdateScores;
            UpdateScores();
        }
        
        public void NewGameButtonHandler() {
            _gameController.StartNewGame();
        }
    }
} // namespace sample_game
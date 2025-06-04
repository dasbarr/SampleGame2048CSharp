using System.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SampleGame2048 {
    
    /// <summary>
    /// Window that shows the game result (win or lose).
    /// </summary>
    public class EndGameWindow : WindowBase {
        
        //-------------------------------------------------------------
        // Nested
        //-------------------------------------------------------------
        
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
        
        //-------------------------------------------------------------
        // Variables
        //-------------------------------------------------------------

        private ScoreStorage _scoreStorage;
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------

        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------

        /// <summary>
        /// Header that will be shown if player won the game.
        /// </summary>
        [SerializeField]
        private GameObject _winHeader;
        /// <summary>
        /// Header that will be shown if player lost the game.
        /// </summary>
        [SerializeField]
        private GameObject _loseHeader;

        /// <summary>
        /// Label for the current game score.
        /// </summary>
        [SerializeField]
        private TMP_Text _scoreLabel;
        /// <summary>
        /// If player earned a new best score, that label will be shown.
        /// </summary>
        [SerializeField]
        private GameObject _newRecordLabel;

        /// <summary>
        /// By pressing that button player will continue the current game.
        /// </summary>
        [SerializeField]
        private Button _continueButton;
        /// <summary>
        /// By pressing that button player will start a new game.
        /// </summary>
        [SerializeField]
        private Button _newGameButton;

        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------
        
        [Inject]
        public void Construct(ScoreStorage scoreStorage) {
            _scoreStorage = scoreStorage;
        }

        /// <summary>
        /// Shows endgame window.
        /// </summary>
        /// <param name="playerWon">If true, 'win' window will be shown, otherwise 'lose' one.</param>
        /// <returns>True if player decided to continue the current game, false if player decided to start a new one.</returns>
        public async Task<bool> ShowAsync(bool playerWon) {
            // show appropriate header
            _winHeader.SetActive(playerWon);
            _loseHeader.SetActive(!playerWon);
            
            // set actual score
            _scoreLabel.text = _scoreStorage.CurrentScore.CurrentValue.ToString();
            _newRecordLabel.SetActive(_scoreStorage.IsNewRecord.CurrentValue);
            
            // player can continue only after winning the game
            _continueButton.gameObject.SetActive(playerWon);

            Show();

            // wait for pressing either 'continue' or 'new game' button
            var continueGame = await _continueButton.OnClickAsObservable().Select(_ => true)
                .Race(_newGameButton.OnClickAsObservable().Select(_ => false))
                .FirstAsync();
            
            Hide();

            return continueGame;
        }
        
        //-------------------------------------------------------------
        // Protected methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private methods
        //-------------------------------------------------------------

        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------
    }
}

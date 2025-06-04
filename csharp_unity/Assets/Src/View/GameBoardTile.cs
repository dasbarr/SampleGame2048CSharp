using System.Linq;
using sample_game.utils;
using UnityEngine;
using UnityEngine.UI;

namespace sample_game {

    /// <summary>
    /// Represents one square item on the game board.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class GameBoardTile : DependencyInjectableUnity {

        //-------------------------------------------------------------
        // Nested
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Dependencies
        //-------------------------------------------------------------

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
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------

        public RectTransform rectTransform { get; private set; }

        /// <summary>
        /// Value (power-of-2) that represents number displayed on that tile (2^(tileValue) = number_to_show).
        /// </summary>
        public int numberValue {
            set {
                // set proper color for tile background and number label
                if (value >= 1) {
                    var tileColors = _gameConfig.tileColors;
                    
                    _backgroundImage.color = value <= tileColors.Count
                        ? tileColors[value - 1]
                        : tileColors.Last(); // use max possible color

                    _tileNumberLabel.gameObject.SetActive(true);
                    _tileNumberLabel.color = value <= 2
                        ? _gameConfig.smallNumberLabelColor
                        : _gameConfig.largeNumberLabelColor;
                    _tileNumberLabel.text = (1 << value).ToString();
                }
                else {
                    // empty tile
                    _backgroundImage.color = _gameConfig.emptyTileColor;
                    _tileNumberLabel.gameObject.SetActive(false);
                }
            }
        }

        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------

        /// <summary>
        /// Label that contains a number displayed on the current tile.
        /// </summary>
        [SerializeField]
        private Text _tileNumberLabel = default;

        /// <summary>
        /// Tile background.
        /// </summary>
        [SerializeField]
        private Image _backgroundImage = default;
        
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
            
            rectTransform = GetComponent<RectTransform>();
        }

        //-------------------------------------------------------------
        // Handlers
        //-------------------------------------------------------------
    }
} // namespace sample_game
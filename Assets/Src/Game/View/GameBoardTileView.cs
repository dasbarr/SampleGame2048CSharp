using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SampleGame2048 {

    /// <summary>
    /// View for the single game board tile.
    /// </summary>
    public class GameBoardTileView : MonoBehaviour {

        //-------------------------------------------------------------
        // Nested
        //-------------------------------------------------------------

        /// <summary>
        /// Creates new tile views using the provided prefab.
        /// </summary>
        public class Factory : PlaceholderFactory<GameBoardTileView> {
        }
        
        [Serializable]
        public class Settings {
            [Tooltip("Empty tiles will have that background color")]
            [field: SerializeField]
            public Color EmptyTileColor { get; private set; }
            
            [Tooltip("Color data for specific tile values")]
            [field: SerializeField]
            public TileViewColorData[] TilesColorData { get; private set; }
            
            [Tooltip("Color data for tile values that are not in the list above")]
            [field: SerializeField]
            public TileViewColorData LargeTileColorData { get; private set; }

            [Tooltip("Duration of new tile show animation (in seconds)")]
            [field: SerializeField]
            public float NewTileShowAnimationDuration { get; private set; } = 0.5f;
            [Tooltip("Duration of tile movement animation (in seconds)")]
            [field: SerializeField]
            public float TileMovementAnimationDuration { get; private set; } = 0.5f;
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
        
        //-------------------------------------------------------------
        // Variables
        //-------------------------------------------------------------

        private Settings _settings;

        /// <summary>
        /// Contains pairs 'tile value (power-of-2) -> color data for that tile'.
        /// </summary>
        private Dictionary<int, TileViewColorData> _colorData;
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------

        private int _powerOf2Value;
        /// <summary>
        /// 'Power-of-2' value that should be shown on the tile.
        /// </summary>
        public int PowerOf2Value {
            set {
                // hide moveable tile part in case of an empty tile
                var isEmptyTile = value == GameConstants.EmptyTileValue;
                _movableTile.gameObject.SetActive(!isEmptyTile);

                if (!isEmptyTile) {
                    // convert 'power-of-2' value to the actual one
                    _valueLabel.text = (1 << value).ToString();
                
                    // update colors
                    if (!_colorData.TryGetValue(value, out var tileColorData)) {
                        // tile value is too large and doesn't have specific color data
                        tileColorData = _settings.LargeTileColorData;
                    }
                    _valueLabel.color = tileColorData.ValueLabelColor;
                    _movableTileBackground.color = tileColorData.BackgroundColor;
                }
            }
        }

        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------

        /// <summary>
        /// Immoveable tile part, always stays on the specific place in the board view grid.
        /// </summary>
        [SerializeField]
        private Image _immovableBackground;
        /// <summary>
        /// Tile that can be animated during new tiles placement or tile movement.
        /// </summary>
        [SerializeField]
        private RectTransform _movableTile;
        
        /// <summary>
        /// Background of the movable tile. Changes its color according to the tile value.
        /// </summary>
        [SerializeField]
        private Image _movableTileBackground;
        /// <summary>
        /// Value label on the movable tile. Changes its color according to the tile value.
        /// </summary>
        [SerializeField]
        private TMP_Text _valueLabel;
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------

        [Inject]
        public void Construct(Settings settings) {
            _settings = settings;
            
            // convert color data to the dictionary for easy access
            _colorData = settings.TilesColorData
                .ToDictionary(x => x.PowerOf2Value);

            _immovableBackground.color = settings.EmptyTileColor;
        }

        /// <summary>
        /// Creates an animation for a new tile placement.
        /// </summary>
        /// <returns>Created animation.</returns>
        public Tween CreateNewTileAnimation() {
            // change tile size a bit
            return _movableTile
                .DOPunchScale(
                    new Vector3(-0.2f, -0.2f),
                    _settings.NewTileShowAnimationDuration,
                    1,
                    0.5f
                );
        }
        
        /// <summary>
        /// Creates an animation for tile movement.
        /// </summary>
        /// <param name="movementDelta">Tile position change.</param>
        /// <returns>Created animation</returns>
        public Tween CreateTileMovementAnimation(Vector3 movementDelta) {
            // move movable tile in the delta direction
            return _movableTile
                .DOLocalMove(movementDelta, _settings.TileMovementAnimationDuration)
                .SetEase(Ease.InSine)
                .OnComplete(() => _movableTile.localPosition = Vector3.zero); // reset tile position at the end of the animation
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

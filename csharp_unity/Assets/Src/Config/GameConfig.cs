using System;
using UnityEngine;
using System.Collections.Generic;

namespace sample_game {
    
    /// <summary>
    /// Scriptable object for the asset that will store game config.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "2048 game config")]
    public class GameConfig : ScriptableObject {
        
        //-------------------------------------------------------------
        // Base
        //-------------------------------------------------------------
        
        [Header("Base")]
        [Tooltip("Default language for localization")]
        [SerializeField]
        private LanguageId _defaultLanguageId = LanguageId.En;
        public LanguageId defaultLanguageId => _defaultLanguageId;

        //-------------------------------------------------------------
        // Game
        //-------------------------------------------------------------
        
        [Header("Game")]
        [Tooltip("Game board will contain that number of items horizontally and the same amount of items vertically")]
        [SerializeField]
        private int _gameBoardSize = 4;
        public int gameBoardSize => _gameBoardSize;
        
        [Tooltip("Tile movement animation duration (in seconds)")]
        [SerializeField]
        private float _tileMovementDuration = 0.6f;
        public float tileMovementDuration => _tileMovementDuration;
        
        [Tooltip("Number of random tiles that will be on the empty board at the start of the new game")]
        [SerializeField]
        private int _numInitialRandomTiles = 2;
        public int numInitialRandomTiles => _numInitialRandomTiles;
        
        [Tooltip("Number of random tiles that will be added to game board each turn (if possible)")]
        [SerializeField]
        private int _numRandomTilesEachTurn = 2;
        public int numRandomTilesEachTurn => _numRandomTilesEachTurn;
        
        [Tooltip("Number that will be shown on a new tiles created each turn. Showld be the power of 2")]
        [SerializeField]
        private int _numberOnNewTile = 2;
        public int numberOnNewTile => _numberOnNewTile;
        
        [Tooltip("Game ends when player gets a tile with that number on it. Showld be the power of 2")]
        [SerializeField]
        private uint _numberOnWinTile = 2048;
        public uint numberOnWinTile => _numberOnWinTile;

        //-------------------------------------------------------------
        // Colors
        //-------------------------------------------------------------
        
        [Header("Colors")]
        [Tooltip("Empty tiles will have that color")]
        [SerializeField]
        private Color _emptyTileColor;
        public Color emptyTileColor => _emptyTileColor;

        [Tooltip("Tile colors. Each color represents a tile with the number 2^N on it (2^1, 2^2 and so on)")]
        [SerializeField]
        private Color[] _tileColors;
        public IReadOnlyList<Color> tileColors => _tileColors;
        
        [Tooltip("Text color for 2^1 a 2^2 tiles")]
        [SerializeField]
        private Color _smallNumberLabelColor;
        public Color smallNumberLabelColor => _smallNumberLabelColor;
        
        [Tooltip("Text color for all other tiles")]
        [SerializeField]
        private Color _largeNumberLabelColor;
        public Color largeNumberLabelColor => _largeNumberLabelColor;

        //-------------------------------------------------------------
        // Internal properties
        //-------------------------------------------------------------

        /// <summary>
        /// Initial value for game tiles, represents a tile with number 2^{initial value}
        /// </summary>
        public int tileInitialValue => (int)Math.Log(numberOnNewTile, 2);
    }
} // namespace sample_game
using System;
using System.Collections.Generic;
using sample_game.utils;
using DG.Tweening;
using UnityEngine;

namespace sample_game {
    
    /// <summary>
    /// Game board visual representation.
    /// </summary>
    public class GameView : DependencyInjectableUnity {
        
        //-------------------------------------------------------------
        // Nested
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Dependencies
        //-------------------------------------------------------------
        
        [DependencyInjector.DependencyAttribute]
        private GameBoardProxy _gameBoardProxy = default;
        [DependencyInjector.DependencyAttribute]
        private GameConfig _gameConfig = default;
        
        //-------------------------------------------------------------
        // Class constants
        //-------------------------------------------------------------
        
        /// <summary>
        /// Indents between game board tiles and between game view and UI lines.
        /// </summary>
        private const int cGameBoardTileIndent = 10;
        
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

        /// <summary>
        /// Contains views for all tiles on the board.
        /// </summary>
        private List<List<GameBoardTile>> _tiles;

        /// <summary>
        /// Size of a single tile view.
        /// </summary>
        private float _tileSize;

        /// <summary>
        /// Animation sequence for tiles movement.
        /// </summary>
        private Sequence _tilesMovementAnimation = null;
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------

        public event Action TileMovementAnimationEnded;
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------

        [SerializeField]
        private RectTransform _background = default;
        
        /// <summary>
        /// All tile views will be inside that container.
        /// </summary>
        [SerializeField]
        private RectTransform _tileContainer = default;

        [SerializeField]
        private GameObject _gameBoardTilePrefab = default;
        
        /// <summary>
        /// Placeholders below tiles. That placeholders will mark tile initial positions.
        /// </summary>
        [SerializeField]
        private GameObject _gameBoardTilePlaceholderPrefab = default;
        
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
        /// Shows actual state of the game board.
        /// </summary>
        private void ShowActualBoardState() {
            SetInitialTilePositions();
            
            // update numbers on tiles
            var gameBoardState = _gameBoardProxy.boardState;
            for (var i = 0; i < _gameConfig.gameBoardSize; i++) {
                for (var j = 0; j < _gameConfig.gameBoardSize; j++) {
                    _tiles[i][j].numberValue = gameBoardState[i][j];
                }
            }
        }

        /// <summary>
        /// Shows tiles movement with animation.
        /// </summary>
        private void ShowTilesMovement(List<BoardTileMoveInfo> tilesMoveData) {
            SetInitialTilePositions();

            _tilesMovementAnimation = DOTween
                .Sequence()
                .SetEase(Ease.InSine)
                .OnComplete(TileMovementAnimationEndHandler);
            
            foreach (var tileMoveData in tilesMoveData) {
                var tileToMove = _tiles[tileMoveData.tileStartCoords.Item1][tileMoveData.tileStartCoords.Item2];
                // move tile to the top
                tileToMove.rectTransform.SetAsLastSibling();

                var tileDestination = CalcTileViewPosition(
                    tileMoveData.tileEndCoords.Item1, tileMoveData.tileEndCoords.Item2
                );
                var tileTween = tileToMove.rectTransform.DOAnchorPos(tileDestination, _gameConfig.tileMovementDuration);

                _tilesMovementAnimation.Join(tileTween);
            }

            _tilesMovementAnimation.Play();
        }

        /// <summary>
        /// Moves all tiles to their initial positions.
        /// </summary>
        private void SetInitialTilePositions() {
            if (_tilesMovementAnimation != null) {
                // discard tiles movement animation
                _tilesMovementAnimation.Kill();
                _tilesMovementAnimation = null;
            }

            // place tiles to their initial positions
            for (var i = 0; i < _gameConfig.gameBoardSize; i++) {
                for (var j = 0; j < _gameConfig.gameBoardSize; j++) {
                    _tiles[i][j].rectTransform.anchoredPosition = CalcTileViewPosition(i, j);
                }
            }
        }

        /// <summary>
        /// Calculates position of tile view based on its coord on the game board.
        /// </summary>
        /// <param name="tileRow">Vertical tile coord.</param>
        /// <param name="tileColumn">Horizontal tile coord.</param>
        /// <returns>Tile view coords.</returns>
        private Vector2 CalcTileViewPosition(int tileRow, int tileColumn) {
            return new Vector2(
                tileColumn * (_tileSize + cGameBoardTileIndent),
                -tileRow * (_tileSize + cGameBoardTileIndent)
            );
        }
        
        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Handlers
        //-------------------------------------------------------------
        
        protected override void OnDependenciesFulfilled() {
            var viewAreaSize = GetComponent<RectTransform>().sizeDelta;
            // view will be in the square area in the center
            var squareAreaSize = Math.Min(viewAreaSize.x, viewAreaSize.y);

            // resize background
            var backgroundSize = squareAreaSize - 2 * cGameBoardTileIndent;
            _background.sizeDelta = new Vector2(backgroundSize, backgroundSize);
            
            _tileSize = (backgroundSize -
                         (_gameConfig.gameBoardSize + 1) * cGameBoardTileIndent
                        ) / _gameConfig.gameBoardSize;
            
            // resize tile container
            var tileContainerSize = backgroundSize - _tileSize - 2 * cGameBoardTileIndent;
            _tileContainer.sizeDelta = new Vector2(tileContainerSize, tileContainerSize);

            // create tiles and tile placeholders
            var tileSizeVec = new Vector2(_tileSize, _tileSize);
            _tiles = new List<List<GameBoardTile>>(_gameConfig.gameBoardSize);
            for (var i = 0; i < _gameConfig.gameBoardSize; i++) {
                _tiles.Add(new List<GameBoardTile>(_gameConfig.gameBoardSize));

                for (var j = 0; j < _gameConfig.gameBoardSize; j++) {
                    // placeholder
                    var tilePlaceholder = Instantiate(_gameBoardTilePlaceholderPrefab, _tileContainer).GetComponent<RectTransform>();
                    tilePlaceholder.sizeDelta = tileSizeVec;
                    tilePlaceholder.anchoredPosition = CalcTileViewPosition(i, j);
                    
                    // tile
                    var tile = Instantiate(_gameBoardTilePrefab, _tileContainer).GetComponent<GameBoardTile>();
                    tile.rectTransform.sizeDelta = tileSizeVec;
                    
                    _tiles[i].Add(tile);
                }
            }

            // subscribe to board changes
            _gameBoardProxy.BoardStateSet += ShowActualBoardState;
            _gameBoardProxy.TilesMoved += ShowTilesMovement;
            
            ShowActualBoardState();
            
            // register itself as a service
            ServiceLocator.AddService(this);
        }

        private void TileMovementAnimationEndHandler() {
            ShowActualBoardState();
            
            TileMovementAnimationEnded?.Invoke();
        }
    }
} // namespace sample_game
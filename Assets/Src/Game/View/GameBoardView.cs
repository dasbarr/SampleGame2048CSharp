using System.Collections.ObjectModel;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using R3;

namespace SampleGame2048 {
    
    /// <summary>
    /// Game board visual representation.
    /// </summary>
    public class GameBoardView : MonoBehaviour {
        
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

        private IGameBoard _gameBoard;
        private SignalBus _signalBus;
        
        private Array2D<GameBoardTileView> _tileViews;

        /// <summary>
        /// Currently playing tiles animation.
        /// </summary>
        private Sequence _tilesAnimation;
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------

        /// <summary>
        /// Contains the view itself.
        /// </summary>
        [SerializeField]
        private RectTransform _viewContainer;
        /// <summary>
        /// LayoutElement component on the view game object.
        /// </summary>
        [SerializeField]
        private LayoutElement _viewLayoutElement;
        
        /// <summary>
        /// Parent object for tile views.
        /// </summary>
        [SerializeField]
        private RectTransform _tileViewsContainer;
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------

        [Inject]
        public void Construct(IGameBoard gameBoard, SignalBus signalBus, GameBoardTileView.Factory tileViewFactory) {
            _gameBoard = gameBoard;
            _signalBus = signalBus;
            
            // create tile views
            _tileViews = new Array2D<GameBoardTileView>(_gameBoard.BoardSize);
            for (var i = 0; i < _gameBoard.BoardSize.Height; i++) {
                for (var j = 0; j < _gameBoard.BoardSize.Width; j++) {
                    var tileView = tileViewFactory.Create();
                    tileView.transform.SetParent(_tileViewsContainer, false);

                    _tileViews[i, j] = tileView;
                }
            }

            // subscribe to board changes
            _gameBoard.BoardStateChanges
                .Subscribe(x => UpdateView(x.changeType, x.additionalData));
        }
        
        //-------------------------------------------------------------
        // Protected methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private methods
        //-------------------------------------------------------------

        /// <summary>
        /// Updates the view when the game board changes.
        /// </summary>
        /// <param name="changeType">Board change type.</param>
        /// <param name="additionalData">Additional data needed to process the board change (can be null if not needed).</param>
        private void UpdateView(BoardStateChangeType changeType, object additionalData) {
            // get rid of previous tile animation (if any)
            _tilesAnimation?.Kill(true);
            _tilesAnimation = null;
            
            switch (changeType) {
                case BoardStateChangeType.BoardCleared:
                    // clear the board
                    _tileViews.ForEach(x => x.PowerOf2Value = GameConstants.EmptyTileValue);
                    break;
                case BoardStateChangeType.NewTilesPlaced:
                    PlaceNewTiles(additionalData as ReadOnlyCollection<BoardTileIndex>);
                    break;
                case BoardStateChangeType.TilesMoved:
                    MoveTileViews(additionalData as ReadOnlyCollection<TileMoveData>);
                    break;
                default:
                    Debug.LogError($"Unknown board state change type {changeType}");
                    break;
            }
        }
        
        private void PlaceNewTiles(ReadOnlyCollection<BoardTileIndex> placedTilesIndices) {
            _tilesAnimation = DOTween.Sequence();

            // update values on tiles with animation
            foreach (var tileIndex in placedTilesIndices) {
                var tileView = _tileViews[tileIndex];
                        
                tileView.PowerOf2Value = _gameBoard.BoardState[tileIndex];
                _tilesAnimation.Insert(0, tileView.CreateNewTileAnimation());
            }

            // animate
            _tilesAnimation
                .OnComplete(() => _tilesAnimation = null) // remove animation after completion
                .Play();
        }
        
        private void MoveTileViews(ReadOnlyCollection<TileMoveData> tilesMovementData) {
            _tilesAnimation = DOTween.Sequence();
            
            // move tile views with animation
            foreach (var tileMovementData in tilesMovementData) {
                var tileToMove = _tileViews[tileMovementData.StartTileIndex];
                var destinationTile = _tileViews[tileMovementData.EndTileIndex];
                
                // place moving tile above other tiles for correct movement animation
                tileToMove.transform.SetAsLastSibling();
                
                var movementDelta = destinationTile.transform.localPosition - tileToMove.transform.localPosition;
                _tilesAnimation.Insert(0, tileToMove.CreateTileMovementAnimation(movementDelta));
            }

            // animate
            _tilesAnimation
                .OnComplete(() => CompleteTilesMovement(tilesMovementData))
                .Play();
        }

        private void CompleteTilesMovement(ReadOnlyCollection<TileMoveData> tilesMovementData) {
            _tilesAnimation = null; // remove animation after completion
            
            int totalMoveScore = 0;
            foreach (var tileMovementData in tilesMovementData) {
                // accumulate move score
                totalMoveScore += tileMovementData.Score;
                
                // set new tile values
                _tileViews[tileMovementData.StartTileIndex].PowerOf2Value = GameConstants.EmptyTileValue;
                _tileViews[tileMovementData.EndTileIndex].PowerOf2Value = _gameBoard.BoardState[tileMovementData.EndTileIndex];
            }
            
            // indicate that movement ended
            _signalBus.Fire(new TileMovementFinishedSignal(totalMoveScore));
        }
        
        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------

        private void Start() {
            var viewContainerSize = _viewContainer.rect.size;
            var tileViewsContainerLayoutGroup = _tileViewsContainer.GetComponent<GridLayoutGroup>();
            var containerPadding = tileViewsContainerLayoutGroup.padding;
            
            // game tiles should be squares, so calculate and set proper parameters to the game view and layouts
            float tileSize;
            float viewSize;
            if (viewContainerSize.x <= viewContainerSize.y) {
                viewSize = viewContainerSize.x;
                tileSize = (viewSize
                                - containerPadding.left
                                - containerPadding.right
                                - tileViewsContainerLayoutGroup.spacing.x * (_gameBoard.BoardSize.Width - 1)
                            ) / _gameBoard.BoardSize.Width;
            }
            else {
                viewSize = viewContainerSize.y;
                tileSize = (viewSize
                            - containerPadding.top
                            - containerPadding.bottom
                            - tileViewsContainerLayoutGroup.spacing.y * (_gameBoard.BoardSize.Height - 1)
                           ) / _gameBoard.BoardSize.Height;
            }

            _viewLayoutElement.preferredWidth = viewSize;
            _viewLayoutElement.preferredHeight = viewSize;
            
            tileViewsContainerLayoutGroup.cellSize = new Vector2(tileSize, tileSize);
            tileViewsContainerLayoutGroup.constraintCount = _gameBoard.BoardSize.Width;
            
            // calculate new layout and remove layout group after (we need to rearrange tile view container's children,
            // so layout group is not necessary anymore)
            Canvas.ForceUpdateCanvases();
            Destroy(tileViewsContainerLayoutGroup);
        }
    }
}

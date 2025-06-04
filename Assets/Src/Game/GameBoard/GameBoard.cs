using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using R3;
using UnityEngine;

namespace SampleGame2048 {
    
    /// <summary>
    /// Representation of the game board - square grid with tiles that contain numbers.
    /// </summary>
    public class GameBoard : IGameBoard {
        
        //-------------------------------------------------------------
        // Nested
        //-------------------------------------------------------------

        [Serializable]
        public class Settings {
            [field: Tooltip("Game board will contain that number of tiles horizontally")]
            [field: SerializeField]
            public int BoardWidth { get; private set; } = 4;

            [field: Tooltip("Game board will contain that number of tiles vertically")]
            [field: SerializeField]
            public int BoardHeight { get; private set; } = 4;
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

        public GameBoard(Settings settings, TileValueGenerator tileValueGenerator) {
            _tileValueGenerator = tileValueGenerator;
            
            // create the board
            _boardState = new Array2D<int>(settings.BoardWidth, settings.BoardHeight);

            // expose readonly properties
            BoardState = _boardState.AsReadOnly();
            AvailableMoves = _availableMoves.AsReadOnly();
            BoardStateChanges = _boardStateChanges.AsObservable();
        }
        
        //-------------------------------------------------------------
        // Variables
        //-------------------------------------------------------------
        
        /// <summary>
        /// Generator for random values for the new tiles.
        /// </summary>
        private readonly TileValueGenerator _tileValueGenerator;

        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------

        private readonly Array2D<int> _boardState;
        /// <inheritdoc />
        public ReadOnlyArray2D<int> BoardState { get; }

        /// <inheritdoc />
        public Size BoardSize => _boardState.Size;

        private readonly HashSet<Move> _availableMoves = new HashSet<Move>();
        /// <inheritdoc />
        public ReadOnlyHashSet<Move> AvailableMoves { get; }
        
        private readonly Subject<(BoardStateChangeType, object)> _boardStateChanges = new Subject<(BoardStateChangeType, object)>();
        /// <inheritdoc />
        public Observable<(BoardStateChangeType changeType, object additionalData)> BoardStateChanges { get; }

        /// <summary>
        /// Max actual number (for example 1024 (=2^10) instead of 10) on the tile the current game board has.
        /// </summary>
        public int MaxTileNumber { get; private set; }

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
        /// Replaces all numbers on the game board with the special 'empty tile' value.
        /// </summary>
        public void Clear() {
            for (var i = 0; i < BoardSize.Height; i++) {
                for (var j = 0; j < BoardSize.Width; j++) {
                    _boardState[i, j] = GameConstants.EmptyTileValue;
                }
            }

            // board doesn't have any tile with actual number now
            MaxTileNumber = 0;
            
            // indicate that board was cleared
            _boardStateChanges.OnNext((BoardStateChangeType.BoardCleared, null));
        }

        /// <summary>
        /// Places tiles with randomly generated numbers to the empty spaces on the game board (if possible).
        /// </summary>
        /// <param name="numTiles">Number of tiles that will be placed.</param>
        public void PlaceRandomTiles(int numTiles) {
            // find all empty tiles
            var emptyTilesIndices = new List<BoardTileIndex>();
            for (var i = 0; i < BoardSize.Height; i++) {
                for (var j = 0; j < BoardSize.Width; j++) {
                    if (_boardState[i, j] == GameConstants.EmptyTileValue)
                        emptyTilesIndices.Add(new BoardTileIndex(i, j));
                }
            }

            // select random empty tiles
            var newTilesIndices = emptyTilesIndices
                .Shuffle()
                .Take(numTiles)
                .ToList();
            // generate and place values to that tiles
            newTilesIndices.ForEach(x => {
                var tileValue = _tileValueGenerator.Generate();
                _boardState[x] = tileValue;
                
                // max number on a tile can be changed after a new tile was placed (most likely at the new game start)
                UpdateMaxTileNumber(1 << tileValue);
            });
            
            UpdateAvailableMoves();
            
            // indicate that tiles were added (if any)
            if (newTilesIndices.Any())
                _boardStateChanges.OnNext((BoardStateChangeType.NewTilesPlaced, newTilesIndices.AsReadOnly()));
        }
        
        /// <summary>
        /// Makes a move (if that move is available).
        /// </summary>
        /// <param name="move">Move to make.</param>
        /// <returns>True if move was performed successfully, false otherwise.</returns>
        public bool MakeMove(Move move) {
            if (!_availableMoves.Contains(move))
                return false; // can't make a move

            // for each row/column tile movement will go from start to end index in the needed direction
            var lineStartIndex = 0;
            if (move == Move.Right) {
                lineStartIndex = BoardSize.Width - 1;
            }
            else if (move == Move.Down) {
                lineStartIndex = BoardSize.Height - 1;
            }

            var lineEndIndex = 0;
            if (move == Move.Left) {
                lineEndIndex = BoardSize.Width - 1;
            }
            else if (move == Move.Up) {
                lineEndIndex = BoardSize.Height - 1;
            }
            
            // outer index enumerates all rows/columns that will be merged
            var isVerticalMove = move == Move.Up || move == Move.Down;
            var maxOuterIndex = isVerticalMove
                ? BoardSize.Width - 1
                : BoardSize.Height - 1;

            // merge tiles and collect tile movement data
            var tilesMoveData = new List<TileMoveData>();
            for (var i = 0; i <= maxOuterIndex; i++) {
                MergeLine(i, lineStartIndex, lineEndIndex, isVerticalMove, ref tilesMoveData);
            }
            
            UpdateAvailableMoves();
            
            // indicate that some tiles were moved (if move is available, there always will be some tile merges/movements)
            _boardStateChanges.OnNext((BoardStateChangeType.TilesMoved, tilesMoveData.AsReadOnly()));

            return true;
        }

        //-------------------------------------------------------------
        // Protected methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private methods
        //-------------------------------------------------------------

        /// <summary>
        /// Performs tile movement/merging for a single game board line (it can be either row or column). Tiles in line
        /// will be moved/merged in the direction from the end index towards the start one.
        /// </summary>
        /// <param name="outerIndex">Index of the row/column that will be merged.</param>
        /// <param name="lineStartIndex">Merge process will be started from the tile with that index in line.</param>
        /// <param name="lineEndIndex">Merge process will be ended with the tile with that index in line.</param>
        /// <param name="flipIndices">
        /// If true, index in line and outer index will be flipped (this allows to use the same merging algorithm
        /// for both rows and columns).
        /// </param>
        /// <param name="tilesMoveData">Data about tile movement will be added here.</param>
        private void MergeLine(int outerIndex, int lineStartIndex, int lineEndIndex, bool flipIndices,
                               ref List<TileMoveData> tilesMoveData
        ) {
            // Constructs a 2D index for the tile determined by the outer index and index in line
            BoardTileIndex ConstructBoardTileIndex(int indexInLine) {
                return flipIndices
                    ? new BoardTileIndex(indexInLine, outerIndex)
                    : new BoardTileIndex(outerIndex, indexInLine);
            }

            // delta determines the traversal direction along the line
            var delta = lineStartIndex < lineEndIndex
                ? 1
                : -1;
            
            for (var i = lineStartIndex; i != lineEndIndex; i += delta) {
                var currentTileIndex = ConstructBoardTileIndex(i);
                var currentTileValue = _boardState[currentTileIndex];
                
                // check next tiles in the line if they can be merged with the current one or moved to the current tile position
                for (var j = i + delta; j != lineEndIndex + delta; j += delta) {
                    var tileToCheckIndex = ConstructBoardTileIndex(j);
                    var tileToCheckValue = _boardState[tileToCheckIndex];
                    
                    if (tileToCheckValue == GameConstants.EmptyTileValue)
                        continue; // can't move or merge the empty tile
                    
                    // score earned by the interaction between the 'tile to check' and the current one
                    int tileScore;
                    // if true, the 'tile to check' had the same value as the current one and was merged with it
                    bool merged;
                    
                    if (currentTileValue == GameConstants.EmptyTileValue) {
                        // current tile is empty, move 'tile to check' to the current one
                        _boardState[currentTileIndex] = currentTileValue = tileToCheckValue;

                        merged = false;
                        // no score for 'tile moved without merge'
                        tileScore = 0;
                    }
                    else if (currentTileValue == tileToCheckValue) {
                        // merge 'tile to check' with the current one
                        _boardState[currentTileIndex] = ++currentTileValue;

                        // max number on a tile can be changed after the merge
                        var newTileNumber = 1 << currentTileValue;
                        UpdateMaxTileNumber(newTileNumber);

                        merged = true;
                        // only merges can produce score points
                        tileScore = newTileNumber; // actual tile value after the merge (2^new_tile_value);
                        
                    }
                    else {
                        // current tile is not empty and value in it is not the same as in the check tile
                        // => merge is not possible (note that merge with any next tile is also not possible)
                        break;
                    }
                    
                    // 'tile to check' becomes an empty one after the successful move/merge
                    _boardState[tileToCheckIndex] = GameConstants.EmptyTileValue;
                    
                    // add move/merge data
                    tilesMoveData.Add(new TileMoveData(tileToCheckIndex, currentTileIndex, tileScore));

                    // tile can still be merged with another tile after move, so we should continue if there was no merge
                    if (merged)
                        break;
                }
            }
        }
        
        /// <summary>
        /// Finds all available moves for the current game board state and updates 'available moves' list.
        /// Move is considered available if there are two adjacent non-empty tiles with the same values
        /// or if one of the adjacent tiles is empty and the other one is not.
        /// </summary>
        private void UpdateAvailableMoves() {
            _availableMoves.Clear();
            
            // Checks if value on the source tile can be moved/merged with the value on the destination tile
            bool CheckMoveAvailable(int destTileRowIndex, int destTileColumnIndex, int sourceTileValue) {
                var destTileValue = _boardState[destTileRowIndex, destTileColumnIndex];
                return destTileValue == GameConstants.EmptyTileValue 
                       || destTileValue == sourceTileValue;
            }
            
            for (var i = 0; i < BoardSize.Height; i++) {
                for (var j = 0; j < BoardSize.Width; j++) {
                    var currentCellValue = _boardState[i, j];
                    
                    if (currentCellValue == GameConstants.EmptyTileValue )
                        continue; // empty tile can't be moved

                    // left
                    if (j - 1 >= 0 && CheckMoveAvailable(i, j - 1, currentCellValue))
                        _availableMoves.Add(Move.Left);
                    // right
                    if (j + 1 < BoardSize.Width && CheckMoveAvailable(i, j + 1, currentCellValue))
                        _availableMoves.Add(Move.Right);
                    // up
                    if (i - 1 >= 0 && CheckMoveAvailable(i - 1, j, currentCellValue))
                        _availableMoves.Add(Move.Up);
                    // down
                    if (i + 1 < BoardSize.Height && CheckMoveAvailable(i + 1, j, currentCellValue))
                        _availableMoves.Add(Move.Down);
                }
            }
        }

        /// <summary>
        /// Updates max tile number if previous max tile number is lower than a new one.
        /// </summary>
        /// <param name="maxTileNumberCandidate">New possible max tile number.</param>
        private void UpdateMaxTileNumber(int maxTileNumberCandidate) {
            if (MaxTileNumber < maxTileNumberCandidate)
                MaxTileNumber = maxTileNumberCandidate;
        }

        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------
    }
}

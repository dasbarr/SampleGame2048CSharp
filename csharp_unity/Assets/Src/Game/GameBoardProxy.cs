using System;
using System.Collections.Generic;
using System.Linq;
using sample_game.utils;

namespace sample_game {
    
    /// <summary>
    /// Representation of the game board - square grid with cells that contain numbers.
    /// </summary>
    public class GameBoardProxy : DependencyInjectable {
        
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
        
        /// <summary>
        /// Value that represents an empty tile on the game board.
        /// </summary>
        private const int cEmptyTileValue = -1;
        
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

        /// <summary>
        /// Dispatched when the new state set for the whole board.
        /// </summary>
        public event Action BoardStateSet;

        /// <summary>
        /// Dispatched when some tiles were moved. Contains info about that movements.
        /// </summary>
        public event Action<List<BoardTileMoveInfo>> TilesMoved; 
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------

        private List<List<int>> _boardState;
        /// <summary>
        /// Represents current board state. Contains 'power-of-2' values for corresponding tiles (for example,
        /// it will contain 10 for tile number 2^10 = 1024). Note that -1 represents an empty tile. Also note that first
        /// index is the index of the row, second index is the index of column. (0, 0) is the top left cell.
        /// </summary>
        public IReadOnlyList<IReadOnlyList<int>> boardState => _boardState;
        
        /// <summary>
        /// True if win tile was acquired, false otherwise.
        /// </summary>
        public bool winTileAcquired { get; private set; } = false;

        /// <summary>
        /// Contains all available moves for current game board state.
        /// </summary>
        public HashSet<Move> availableMoves { get; } = new HashSet<Move>();

        /// <summary>
        /// True if at least one move is available, false otherwise.
        /// </summary>
        public bool hasAvailableMoves => availableMoves.Count > 0;

        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------
        
        /// <summary>
        /// Resets game board to initial state.
        /// </summary>
        public void Reset() {
            // empty the board
            foreach (var row in _boardState) {
                for (var i = 0; i < _gameConfig.gameBoardSize; i++) {
                    row[i] = cEmptyTileValue;
                }
            }
            
            winTileAcquired = false;
            
            // add initial tiles
            PlaceRandomTiles(_gameConfig.numInitialRandomTiles);
        }

        /// <summary>
        /// Place random tiles with initial values (if possible).
        /// </summary>
        /// <param name="numTiles">Number of tiles that will be placed.</param>
        public void PlaceRandomTiles(int numTiles) {
            // find empty tiles
            var emptyTilesCoords = new List<Tuple<int, int>>();
            for (int i = 0; i < _gameConfig.gameBoardSize; i++) {
                for (int j = 0; j < _gameConfig.gameBoardSize; j++) {
                    if (boardState[i][j] == cEmptyTileValue)
                        emptyTilesCoords.Add(new Tuple<int, int>(i, j));
                }
            }

            // place tiles
            var shuffledEmptyTilesCoords = emptyTilesCoords.Shuffle();
            var numTilesToPlace = Math.Min(numTiles, emptyTilesCoords.Count);
            foreach (var emptyTileCoords in shuffledEmptyTilesCoords.Take(numTilesToPlace)) {
                _boardState[emptyTileCoords.Item1][emptyTileCoords.Item2] = _gameConfig.tileInitialValue;
            }
            
            UpdateAvailableMoves();
            
            BoardStateSet?.Invoke();
        }

        /// <summary>
        /// Makes a move (if that move is available).
        /// </summary>
        /// <param name="move">Move to make.</param>
        /// <param name="moveScore">Out parameter, will contain score player earned by that move.</param>
        /// <returns>True if move was performed successfully, false otherwise.</returns>
        public bool MakeMove(Move move, out int moveScore) {
            moveScore = 0;
            
            if (move == Move.IncorrectMove || !availableMoves.Contains(move))
                return false; // can't make a move
            
            // for each row/column tile movement will go from start to end index in the needed direction
            var startIndex = move == Move.Left || move == Move.Up ? 0 : _gameConfig.gameBoardSize - 1;
            var endIndex = startIndex == 0 ? _gameConfig.gameBoardSize - 1 : 0;
            var delta = startIndex == 0 ? 1 : -1;
            
            var isHorizontalMove = move == Move.Left || move == Move.Right;
            int GetCellValue(int outerIndex, int index) {
                return isHorizontalMove
                    ? boardState[outerIndex][index]
                    : boardState[index][outerIndex];
            }
            void SetCellValue(int outerIndex, int index, int value) {
                if (isHorizontalMove) {
                    _boardState[outerIndex][index] = value;
                }
                else {
                    _boardState[index][outerIndex] = value;
                }
            }
            Tuple<int, int> PackCellCoords(int outerIndex, int index) {
                return isHorizontalMove
                    ? new Tuple<int, int>(outerIndex, index)
                    : new Tuple<int, int>(index, outerIndex);
            }
            
            var boardTilesMoveData = new List<BoardTileMoveInfo>();
            
            // outer index corresponds to row index for horizontal moves and column index for vertical moves
            for (int outerIndex = 0; outerIndex < _gameConfig.gameBoardSize; outerIndex++) {
                int currentIndex = startIndex;
                do {
                    var currentCellValue = GetCellValue(outerIndex, currentIndex);
                    
                    // try to find non-empty value in some cell after the current cell
                    bool nonEmptyValueFound = false;
                    for (int checkIndex = currentIndex + delta; checkIndex != endIndex + delta; checkIndex += delta) {
                        var checkCellValue = GetCellValue(outerIndex, checkIndex);
                        if (checkCellValue != cEmptyTileValue) {
                            // non-empty value found
                            nonEmptyValueFound = true;

                            var moved = false;
                            var merged = false;
                            if (currentCellValue == cEmptyTileValue) {
                                // move value to the empty current cell
                                SetCellValue(outerIndex, currentIndex, checkCellValue);
                                
                                moved = true;
                            }
                            else if (currentCellValue == checkCellValue) {
                                // merge value with the non-empty current cell
                                var mergedValue = currentCellValue + 1;
                                SetCellValue(outerIndex, currentIndex, mergedValue);

                                var scoreToAdd = 1 << mergedValue;
                                if (scoreToAdd >= _gameConfig.numberOnWinTile)
                                    winTileAcquired = true;
                                moveScore += scoreToAdd;
                                
                                merged = true;
                            }

                            if (moved || merged) {
                                // empty check cell
                                SetCellValue(outerIndex, checkIndex, cEmptyTileValue);

                                // add info about tile movement
                                boardTilesMoveData.Add(
                                    new BoardTileMoveInfo {
                                        tileStartCoords = PackCellCoords(outerIndex, checkIndex),
                                        tileEndCoords = PackCellCoords(outerIndex, currentIndex)
                                    }
                                );
                            }

                            // prevent double merges, but allow merge after move
                            if (!moved)
                                currentIndex += delta;
                            break;
                        }
                    }

                    if (!nonEmptyValueFound) {
                        // all cells after the current one are empty, it's not necessary to continue
                        break;
                    }
                } while (currentIndex != endIndex);
            }
            
            UpdateAvailableMoves();
            
            if (boardTilesMoveData.Count > 0)
                TilesMoved?.Invoke(boardTilesMoveData);

            return true;
        }

        //-------------------------------------------------------------
        // Protected methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private methods
        //-------------------------------------------------------------
        
        /// <summary>
        /// Finds all available moves at current game board state and updates 'available moves' list.
        /// Move is available if there are two equal adjacent values (not zero) or if one of adjacent
        /// cells is empty.
        /// </summary>
        private void UpdateAvailableMoves() {
            availableMoves.Clear();
            
            // move is available either if adjacent cell is empty or if it contains the same number as checked cell
            bool CheckMoveAvailable(int destRow, int destColumn, int value) {
                var destValue = boardState[destRow][destColumn];
                return destValue == cEmptyTileValue || destValue == value;
            }

            for (var i = 0; i < _gameConfig.gameBoardSize; i++) {
                for (var j = 0; j < _gameConfig.gameBoardSize; j++) {
                    var currentCellValue = boardState[i][j];
                    
                    if (currentCellValue == cEmptyTileValue)
                        continue; // empty tile can't be moved

                    // left
                    if (j - 1 >= 0 && CheckMoveAvailable(i, j - 1, currentCellValue))
                        availableMoves.Add(Move.Left);
                    // right
                    if (j + 1 < _gameConfig.gameBoardSize && CheckMoveAvailable(i, j + 1, currentCellValue))
                        availableMoves.Add(Move.Right);
                    // up
                    if (i - 1 >= 0 && CheckMoveAvailable(i - 1, j, currentCellValue))
                        availableMoves.Add(Move.Up);
                    // down
                    if (i + 1 < _gameConfig.gameBoardSize && CheckMoveAvailable(i + 1, j, currentCellValue))
                        availableMoves.Add(Move.Down);

                    if (availableMoves.Count == 4) {
                        // all possible moves are available, it's not necessary to check further
                        return;
                    }
                }
            }
        }
        
        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Handlers
        //-------------------------------------------------------------
        
        protected override void OnDependenciesFulfilled() {
            // create game board
            _boardState = new List<List<int>>(_gameConfig.gameBoardSize);
            for (int i = 0; i < _gameConfig.gameBoardSize; i++) {
                _boardState.Add(new int[_gameConfig.gameBoardSize].ToList());
            }
        }
    }
} // namespace sample_game
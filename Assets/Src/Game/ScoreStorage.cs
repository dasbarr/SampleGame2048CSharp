using R3;
using Zenject;

namespace SampleGame2048 {
    
    /// <summary>
    /// Holds info about player score.
    /// </summary>
    public class ScoreStorage {
        
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

        public ScoreStorage(SignalBus signalBus) {
            // expose readonly properties
            CurrentScore = _currentScore.ToReadOnlyReactiveProperty();
            BestScore = _bestScore.ToReadOnlyReactiveProperty();
            
            // update score at the end of the move
            signalBus.GetStream<TileMovementFinishedSignal>()
                .Subscribe(x => {
                    _currentScore.Value += x.TotalMoveScore;
                    
                    if (_bestScore.Value < _currentScore.Value)
                        _bestScore.Value = _currentScore.Value;
                });
            
            // clear current score at the game start (best score should remain untouched)
            signalBus.GetStream<GameStateChangedSignal>()
                .Where(x => x.NewGameState == GameState.NewGamePreparation)
                .Subscribe(_ => _currentScore.Value = 0);
            
            // indicate when new best score was reached
            IsNewRecord = _currentScore
                .CombineLatest(_bestScore, (current, best) => current == best)
                .ToReadOnlyReactiveProperty();
        }
        
        //-------------------------------------------------------------
        // Variables
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------

        private readonly ReactiveProperty<int> _currentScore = new ReactiveProperty<int>();
        /// <summary>
        /// Score in the current game.
        /// </summary>
        public ReadOnlyReactiveProperty<int> CurrentScore { get; }
        
        private readonly ReactiveProperty<int> _bestScore = new ReactiveProperty<int>();
        /// <summary>
        /// Best score earned.
        /// </summary>
        public ReadOnlyReactiveProperty<int> BestScore { get; }

        /// <summary>
        /// If true, a new best score was reached in the current game.
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsNewRecord { get; }

        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
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
    }
}

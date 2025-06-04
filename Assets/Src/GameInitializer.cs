using DG.Tweening;
using R3;
using UnityEngine;
using Zenject;

namespace SampleGame2048 {
    
    /// <summary>
    /// Entry point for the game.
    /// </summary>
    public class GameInitializer : IInitializable {
        
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
        
        private GeneralGameSettings _generalSettings;
        private GameController _gameController;
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------
        
        [Inject]
        public void Construct(GeneralGameSettings generalSettings, GameController gameController) {
            _generalSettings = generalSettings;
            _gameController = gameController;
        }
        
        /// <summary>
        /// Initializes the game.
        /// </summary>
        public void Initialize() {
            Application.targetFrameRate = _generalSettings.TargetFrameRate;
            
            // init DOTween
            DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
            DOTween.defaultAutoPlay = AutoPlay.None;
            DOTween.defaultUpdateType = UpdateType.Normal;
            
            // we need to have an actual layout for later calculations, so force layout update
            Canvas.ForceUpdateCanvases();
            
            // make sure that everything was inited correctly by starting the game on the next frame
            Observable.NextFrame()
                .Subscribe(_ => _gameController.StartNewGame());
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

using sample_game.utils;
using DG.Tweening;
using UnityEngine;

namespace sample_game {
    
    /// <summary>
    /// Entry point, initializes the game.
    /// </summary>
    public static class GameInitializer {
        
        //-------------------------------------------------------------
        // Class constants
        //-------------------------------------------------------------
        
        private const int cTargetFrameRate = 60;
        
        //-------------------------------------------------------------
        // Class methods
        //-------------------------------------------------------------
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnApplicationLoaded() {
            Application.targetFrameRate = cTargetFrameRate;
            
            // init DOTween
            DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
            DOTween.defaultAutoPlay = AutoPlay.None;
            DOTween.defaultUpdateType = UpdateType.Normal;
            
            // make sure that all canvases will have proper size before game initialization
            Canvas.ForceUpdateCanvases();
            
            // game config
            ServiceLocator.AddService(Resources.Load<GameConfig>("GameConfig"));
            
            // init common services
            ServiceLocator.AddService(new LocalizationManager());
            
            // init game services
            ServiceLocator.AddService(new GameBoardProxy());
            ServiceLocator.AddService(new ScoreProxy());
            ServiceLocator.AddService(new GameController());
            
            // init simple bot
            ServiceLocator.AddService(BotController.Create<PushTheTempoBot>());
        }
    }
} // namespace sample_game
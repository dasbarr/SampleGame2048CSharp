using UnityEngine;
using Zenject;

namespace SampleGame2048 {
    
    /// <summary>
    /// Holds game settings and provides them for the dependency injection.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Sample 2048 game/Game config")]
    public class GameConfig : ScriptableObjectInstaller {

        [SerializeField]
        private GeneralGameSettings _generalGameSettings;
        
        [SerializeField]
        private GameController.Settings _gameControllerSettings;
        
        [SerializeField]
        private GameBoard.Settings _gameBoardSettings;
        
        [SerializeField]
        private TileValueGenerator.Settings _tileValueGeneratorSettings;
        
        [SerializeField]
        private GameBoardTileView.Settings _tileViewSettings;
        
        [SerializeField]
        private BotController.Settings _botSettings;
        
        [SerializeField]
        private WindowBase.Settings _overlayWindowSettings;

        public override void InstallBindings() {
            Container.Bind<GeneralGameSettings>()
                .FromInstance(_generalGameSettings);
            
            Container.Bind<GameController.Settings>()
                .FromInstance(_gameControllerSettings)
                .WhenInjectedInto<GameController>();
            
            Container.Bind<GameBoard.Settings>()
                .FromInstance(_gameBoardSettings)
                .WhenInjectedInto<GameBoard>();
            
            Container.Bind<TileValueGenerator.Settings>()
                .FromInstance(_tileValueGeneratorSettings)
                .WhenInjectedInto<TileValueGenerator>();
            
            Container.Bind<GameBoardTileView.Settings>()
                .FromInstance(_tileViewSettings)
                .WhenInjectedInto<GameBoardTileView>();
            
            Container.Bind<BotController.Settings>()
                .FromInstance(_botSettings)
                .WhenInjectedInto<BotController>();
            
            Container.Bind<WindowBase.Settings>()
                .FromInstance(_overlayWindowSettings)
                .WhenInjectedInto<WindowBase>();
        }
    }
}

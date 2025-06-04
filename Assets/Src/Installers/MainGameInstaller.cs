using System.Linq;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace SampleGame2048 {
    
    /// <summary>
    /// Main dependency injection installer.
    /// </summary>
    public class MainGameInstaller : MonoInstaller {

        [Header("GameView Prefabs")]
        [SerializeField]
        private GameObject _tileViewPrefab;

        [Header("Overlay")]
        [Tooltip("All overlay objects (for example, modal windows) will be created as children of that game object")]
        [SerializeField]
        private Transform _overlayObjectsContainer;
        [SerializeField]
        private GameObject _endGameWindowPrefab;
        [SerializeField]
        private GameObject _rulesControlsWindowPrefab;
        
        public override void InstallBindings() {
            InitSignals();
            InitFactories();
            InitOverlay();

            Container.Bind<PlayerInputManager>()
                .AsSingle();

            Container.Bind<TileValueGenerator>()
                .WhenInjectedInto<GameBoard>();
            
            // everyone can get access to the game board via the external interface
            Container.Bind<IGameBoard>()
                .To<GameBoard>()
                .AsSingle();
            // but only GameController have direct access to the GameBoard instance
            Container.Bind<GameBoard>()
                .FromResolveGetter<IGameBoard>(_ => (GameBoard)Container.Resolve<IGameBoard>())
                .WhenInjectedInto<GameController>();

            Container.Bind<ScoreStorage>()
                .AsSingle();
            
            Container.Bind<IBot>()
                .To<PushTheTempoBot>()
                .WhenInjectedInto<BotController>();
            Container.Bind<BotController>()
                .AsSingle();

            Container.Bind<GameController>()
                .AsSingle();
            
            // register entry point
            Container.BindInterfacesTo<GameInitializer>()
                .AsSingle()
                .NonLazy();
        }

        private void InitSignals() {
            SignalBusInstaller.Install(Container);
            
            // we don't care if signal was fired when there were no handlers
            Container.Settings.Signals.MissingHandlerDefaultResponse = SignalMissingHandlerResponses.Ignore;
            
            // register all signals via the reflection
            Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => typeof(ISignal).IsAssignableFrom(x))
                .ForEach(x => Container.DeclareSignal(x));
        }

        private void InitFactories() {
            Container.BindFactory<GameBoardTileView, GameBoardTileView.Factory>()
                .FromComponentInNewPrefab(_tileViewPrefab)
                .WithGameObjectName("GameBoardTileView");
        }
        
        private void InitOverlay() {
            Container.Bind<EndGameWindow>()
                .FromComponentInNewPrefab(_endGameWindowPrefab)
                .WithGameObjectName("EndGameWindow")
                .UnderTransform(_overlayObjectsContainer)
                .AsSingle();
            
            Container.Bind<RulesControlsWindow>()
                .FromComponentInNewPrefab(_rulesControlsWindowPrefab)
                .WithGameObjectName("RulesControlsWindow")
                .UnderTransform(_overlayObjectsContainer)
                .AsSingle();
        }
    }
}

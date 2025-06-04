using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SampleGame2048 {
    
    public class ButtonsPanel : MonoBehaviour {
        
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
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------
        
        /// <summary>
        /// Button that starts a new game.
        /// </summary>
        [SerializeField]
        private Button _newGameButton;
        /// <summary>
        /// Button that shows 'Rules & controls' window.
        /// </summary>
        [SerializeField]
        private Button _rulesControlsButton;
        /// <summary>
        /// Button that enables a bot.
        /// </summary>
        [SerializeField]
        private Button _enableBotButton;
        /// <summary>
        /// Button that disables a bot.
        /// </summary>
        [SerializeField]
        private Button _disableBotButton;
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------
        
        [Inject]
        public void Construct(GameController gameController, RulesControlsWindow rulesControlsWindow, BotController botController) {
            _newGameButton.OnClickAsObservable()
                .Subscribe(_ => gameController.StartNewGame());

            _rulesControlsButton.OnClickAsObservable()
                .Subscribe(_ => rulesControlsWindow.Show());

            // only one of disable/enable bot buttons should be shown at the same time
            botController.Enabled
                .Subscribe(x => {
                    _enableBotButton.gameObject.SetActive(!x);
                    _disableBotButton.gameObject.SetActive(x);
                });
            _enableBotButton.OnClickAsObservable().Select(_ => true)
                .Merge(_disableBotButton.OnClickAsObservable().Select(_ => false))
                .Subscribe(botController.Enabled);
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

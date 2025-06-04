using sample_game.utils;
using UnityEngine;

namespace sample_game {
    
    /// <summary>
    /// Bottom UI line.
    /// </summary>
    public class BottomUI : DependencyInjectableUnity {
        
        //-------------------------------------------------------------
        // Nested
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Dependencies
        //-------------------------------------------------------------
        
        [DependencyInjector.DependencyAttribute]
        private LocalizationManager _localizationManager = default;
        [DependencyInjector.DependencyAttribute]
        private BotController _botController = default;
        
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
        // Constructor/destructor
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Variables
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------

        /// <summary>
        /// AutoTranslator for the 'toggle bot' button.
        /// </summary>
        [SerializeField]
        private AutoTranslator _toggleBotButtonLabel = default;

        /// <summary>
        /// 'Bot enabled' label.
        /// </summary>
        [SerializeField]
        private GameObject _botEnabledLabel = default;
        
        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Protected methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private methods
        //-------------------------------------------------------------
        
        private void UpdateBotStateIndication() {
            _toggleBotButtonLabel.localizedStringId = _botController.botEnabled
                ? "StopBot"
                : "StartBot";
            
            _botEnabledLabel.SetActive(_botController.botEnabled);
        }
        
        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Handlers
        //-------------------------------------------------------------

        protected override void OnDependenciesFulfilled() {
            _botController.BotStateChanged += UpdateBotStateIndication;
            // set initial state of bot-related components
            UpdateBotStateIndication();
        }
        
        public void ToggleBotButtonHandler() {
            _botController.ToggleBot();
        }

        public void ToggleLanguageButtonHandler() {
            _localizationManager.ToggleLanguage();
        }
    }
} // namespace sample_game
using sample_game.utils;
using UnityEngine;
using UnityEngine.UI;

namespace sample_game {
    
    /// <summary>
    /// Provides automatic translation for text component when application language changes.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class AutoTranslator : DependencyInjectableUnity {
        
        //-------------------------------------------------------------
        // Nested
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Dependencies
        //-------------------------------------------------------------
        
        [DependencyInjector.DependencyAttribute]
        private LocalizationManager _localizationManager = default;
        
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
        
        /// <summary>
        /// Id of a localized string.
        /// </summary>
        public string localizedStringId {
            set {
                _localizedStringId = value;
                UpdateLocalization();
            }
        }
        
        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------

        /// <summary>
        /// String representation of LocaleStringId.
        /// </summary>
        [SerializeField]
        private string _localizedStringId = default;
        
        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Protected methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private methods
        //-------------------------------------------------------------

        private void UpdateLocalization() {
            GetComponent<Text>().text = string.IsNullOrEmpty(_localizedStringId)
                ? "" // show an empty string when there is not id specified
                : _localizationManager.GetLocalizedString(_localizedStringId);
        }
        
        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------

        //-------------------------------------------------------------
        // Handlers
        //-------------------------------------------------------------
        
        protected override void OnDependenciesFulfilled() {
            _localizationManager.CurrentLanguageChanged += UpdateLocalization;
            UpdateLocalization();
        }
    }
} // namespace sample_game
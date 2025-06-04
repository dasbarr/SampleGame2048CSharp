using System;
using System.Collections.Generic;
using System.Linq;
using sample_game.utils;
using UnityEngine;

namespace sample_game {
    
    /// <summary>
    /// Provides strings for game localization.
    /// </summary>
    public class LocalizationManager : DependencyInjectable {

        //-------------------------------------------------------------
        // Nested
        //-------------------------------------------------------------

        /// <summary>
        /// Contains data for all localizations.
        /// </summary>
        [Serializable]
        private class LocalizationsDataContainer {
            public LocalizationData[] localizations = default;
        }

        //-------------------------------------------------------------
        // Dependencies
        //-------------------------------------------------------------
        
        [DependencyInjector.DependencyAttribute]
        private GameConfig _gameConfig = default;
        
        //-------------------------------------------------------------
        // Class constants
        //-------------------------------------------------------------
        
        private const string cMissingLocale = "<missing>";
        
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
        
        /// <summary>
        /// Contains pairs 'language id -> localization for that language'.
        /// </summary>
        private readonly Dictionary<LanguageId, Localization> _localizations = new Dictionary<LanguageId, Localization>();

        /// <summary>
        /// Id of the current language.
        /// </summary>
        private LanguageId? _currentLanguageId = null;
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
        public event Action CurrentLanguageChanged;

        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------

        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------

        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------

        /// <summary>
        /// Gets a localized string.
        /// </summary>
        /// <param name="localizedStringId">Id for that string.</param>
        /// <returns>Localized string with that id or some default string if requested string is missing.</returns>
        public string GetLocalizedString(string localizedStringId) {
            if (_currentLanguageId.HasValue &&
                _localizations[_currentLanguageId.Value].TryGetLocalizedString(localizedStringId, out var requestedString)
            ) {
                return requestedString;
            }
            
            // requested string is missing
            Debug.LogWarning("Localized string '" + localizedStringId + "' is missing");
            return cMissingLocale;
        }
        
        /// <summary>
        /// Switches to another language (if possible).
        /// </summary>
        public void ToggleLanguage() {
            if (_localizations.Count < 2)
                return; // can't toggle, too few languages available
            
            var availableLanguages = _localizations.Keys.ToList();
            var nextLanguages = availableLanguages.SkipWhile(languageId => languageId != _currentLanguageId).Skip(1);

            var languageToSet = nextLanguages.Any()
                ? nextLanguages.First()
                : availableLanguages.First();
            SetLanguage(languageToSet);
        }
        
        //-------------------------------------------------------------
        // Protected methods
        //-------------------------------------------------------------

        //-------------------------------------------------------------
        // Private methods
        //-------------------------------------------------------------

        /// <summary>
        /// Sets a specific language.
        /// </summary>
        /// <param name="languageId">New language id.</param>
        private void SetLanguage(LanguageId languageId) {
            if (_currentLanguageId == languageId)
                return; // already set

            if (_localizations.ContainsKey(languageId)) {
                _currentLanguageId = languageId;
                CurrentLanguageChanged?.Invoke();
            }
            else {
                Debug.LogError("Can't set language '" + languageId + "' - localization data doesn't exist");
            }
        }
        
        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------

        //-------------------------------------------------------------
        // Handlers
        //-------------------------------------------------------------
        
        protected override void OnDependenciesFulfilled() {
            // load localizations
            var localizationsFile = Resources.Load<TextAsset>("Localizations");
            var localizationsDataContainer = JsonUtility.FromJson<LocalizationsDataContainer>(localizationsFile.text);
            foreach (var localizationData in localizationsDataContainer.localizations) {
                var localization = new Localization(localizationData);

                if (localization.languageId != LanguageId.NotSupported)
                    _localizations[localization.languageId] = localization;
            }
            
            // set initial language
            SetLanguage(_gameConfig.defaultLanguageId);
        }
    }
} // namespace sample_game
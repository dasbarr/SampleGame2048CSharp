using System.Linq;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace SampleGame2048 {
    
    /// <summary>
    /// Localization selection dropdown.
    /// </summary>
    public class LanguageSelector : MonoBehaviour {
        
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
        /// Actual dropdown.
        /// </summary>
        [SerializeField]
        private TMP_Dropdown _localeDropdown;

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

        private void SetLocale(string localeCode) {
            var locale = LocalizationSettings.AvailableLocales.Locales
                .FirstOrDefault(x => x.Identifier.Code == localeCode);

            if (locale != null) {
                LocalizationSettings.SelectedLocale = locale;
            }
            else {
                Debug.LogError($"Locale {localeCode} is not available");
            }
        }
        
        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------

        private void Awake() {
            // select default locale in dropdown
            var defaultLocaleCode = LocalizationSettings.SelectedLocale.Identifier.Code;
            var defaultLocaleOptionIndex = _localeDropdown.options
                .FindIndex(x => x.text == defaultLocaleCode);
            
            if (defaultLocaleOptionIndex >= 0) {
                _localeDropdown.value = defaultLocaleOptionIndex;
            }
            else {
                Debug.LogError($"Default locale {defaultLocaleCode} is not in the language selector's option list");
            }
            
            // switch locale when needed
            _localeDropdown.OnValueChangedAsObservable()
                .Select(x => _localeDropdown.options[x].text)
                .Subscribe(SetLocale);
        }
    }
}

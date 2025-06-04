using System;
using System.Collections.Generic;
using UnityEngine;

namespace sample_game {
    
    /// <summary>
    /// Contains unpacked localization data.
    /// </summary>
    public class Localization {
        
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
        // Constructor/destructor
        //-------------------------------------------------------------

        public Localization(LocalizationData localizationData) {
            // unpack localized strings
            foreach (var localizedStringEntry in localizationData.localizedStrings) {
                _localizedStrings[localizedStringEntry.id] = localizedStringEntry.localizedString;
            }
            
            // set language id
            if (Enum.TryParse(localizationData.languageId, out LanguageId parsedLanguageId)) {
                languageId = parsedLanguageId;
            }
            else {
                Debug.LogError("Language id '" + localizationData.languageId + "' can't be parsed");
                languageId = LanguageId.NotSupported;
            }
        }
        
        //-------------------------------------------------------------
        // Variables
        //-------------------------------------------------------------
        
        /// <summary>
        /// Contains pairs 'localized string id -> localized string'.
        /// </summary>
        private readonly Dictionary<string, string> _localizedStrings = new Dictionary<string, string>();
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------
        
        /// <summary>
        /// Language id for that localization.
        /// </summary>
        public LanguageId languageId { get; }

        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------
        
        /// <summary>
        /// Gets a localized string if it exists.
        /// </summary>
        /// <param name="localizedStringId">Id for that string.</param>
        /// <param name="requestedString">
        /// Output parameter, localized string with that id or null if requested string doesn't exist.
        /// </param>
        /// <returns>True if requested string exists, false otherwise.</returns>
        public bool TryGetLocalizedString(string localizedStringId, out string requestedString) {
            return _localizedStrings.TryGetValue(localizedStringId, out requestedString);
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
        
        //-------------------------------------------------------------
        // Handlers
        //-------------------------------------------------------------
    }
} // namespace sample_game
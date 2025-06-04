using System;

namespace sample_game {
    
    /// <summary>
    /// Serializable class that contains localized strings.
    /// </summary>
    [Serializable]
    public class LocalizationData {

        /// <summary>
        /// Contains data about one localized string.
        /// </summary>
        [Serializable]
        public class LocalizedStringEntry {
            // string id
            public string id;
            // string itself
            public string localizedString;
        }

        /// <summary>
        /// String representation of LanguageId enum item.
        /// </summary>
        public string languageId;
        
        public LocalizedStringEntry[] localizedStrings;
    }
} // namespace sample_game
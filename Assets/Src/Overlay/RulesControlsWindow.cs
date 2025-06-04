using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Zenject;

namespace SampleGame2048 {
    
    /// <summary>
    /// Info window that contains game rules and controls.
    /// </summary>
    public class RulesControlsWindow : WindowBase {
        
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
        /// Localize event for the description part that should be customized using the game settings.
        /// </summary>
        [SerializeField]
        private LocalizeStringEvent _desc3LocalizeEvent;
        
        [SerializeField]
        private Button _okButton;
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------

        [Inject]
        public void Construct(GeneralGameSettings generalGameSettings) {
            // set 'tile number to win the game' as description argument
            _desc3LocalizeEvent.StringReference.Arguments = new List<object> { generalGameSettings.NumberOnWinTile };
            
            // hide window when OK button pressed
            _okButton.OnClickAsObservable()
                .Subscribe(_ => Hide());
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

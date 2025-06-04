using System;
using sample_game.utils;
using UnityEngine;

namespace sample_game {
    
    /// <summary>
    /// Manages player input.
    /// </summary>
    public class InputManager : MonoBehaviour {
        
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

        //-------------------------------------------------------------
        // Variables
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
        /// <summary>
        /// Dispatched when player input was registered. Contains registered move.
        /// </summary>
        public event Action<Move> PlayedMoveInput;
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------

        /// <summary>
        /// Indicates if player input allowed or not. If not, game will not receive signals related to button press.
        /// </summary>
        public bool playerInputAllowed { get; set; } = true;

        //-------------------------------------------------------------
        // Private properties serialized for Unity
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

        private void DispatchMove(Move move) {
            if (!playerInputAllowed)
                return;
            
            PlayedMoveInput?.Invoke(move);
        }
        
        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------

        private void Awake() {
            // register itself as a service
            ServiceLocator.AddService(this);
        }

        private void OnUp() {
            DispatchMove(Move.Up);
        }
        private void OnDown() {
            DispatchMove(Move.Down);
        }
        private void OnLeft() {
            DispatchMove(Move.Left);
        }
        private void OnRight() {
            DispatchMove(Move.Right);
        }

        //-------------------------------------------------------------
        // Handlers
        //-------------------------------------------------------------
    }
} // namespace sample_game
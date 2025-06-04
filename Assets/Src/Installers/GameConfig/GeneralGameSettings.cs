using System;
using UnityEngine;

namespace SampleGame2048 {
    
    /// <summary>
    /// Game settings that are not directly related to single game part.
    /// </summary>
    [Serializable]
    public class GeneralGameSettings {
        
        [Tooltip("Game frame rate")]
        [field: SerializeField]
        public int TargetFrameRate { get; private set; } = 60;
        
        [Tooltip("Player should have that number on at least 1 tile to win the game")]
        [field: SerializeField]
        public int NumberOnWinTile { get; private set; } = 2048;
    }
}

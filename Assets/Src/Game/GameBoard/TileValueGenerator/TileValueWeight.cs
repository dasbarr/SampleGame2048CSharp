using System;
using UnityEngine;

namespace SampleGame2048 {
    
    /// <summary>
    /// Data entry for tile value weight settings.
    /// </summary>
    [Serializable]
    public struct TileValueWeight {
        
        [Tooltip("Tile value")]
        [field: SerializeField]
        public int PowerOf2Value { get; private set; }
        
        [Tooltip("Weight of the value above")]
        [field: SerializeField]
        public int Weight { get; private set; }
    }
}

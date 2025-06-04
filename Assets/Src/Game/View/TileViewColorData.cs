using System;
using UnityEngine;

namespace SampleGame2048 {
    
    /// <summary>
    /// Color data for game board tile views.
    /// </summary>
    [Serializable]
    public class TileViewColorData {

        [Tooltip("Color data will be applied for the tile with that 'power-of-2' value")]
        [field: SerializeField]
        public int PowerOf2Value { get; private set; }
        
        [Tooltip("Tile background color")]
        [field: SerializeField]
        public Color BackgroundColor { get; private set; }
        [Tooltip("Tile label color")]
        [field: SerializeField]
        public Color ValueLabelColor { get; private set; }
    }
}

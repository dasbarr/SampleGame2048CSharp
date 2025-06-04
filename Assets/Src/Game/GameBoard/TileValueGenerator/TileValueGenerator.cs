using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SampleGame2048 {
    
    /// <summary>
    /// Generates values for the new tiles during the game.
    /// </summary>
    public class TileValueGenerator {
        
        //-------------------------------------------------------------
        // Nested
        //-------------------------------------------------------------

        [Serializable]
        public class Settings {
            [Tooltip("Value weights for random new tile value generation")]
            [field: SerializeField]
            public TileValueWeight[] TileValueWeights { get; private set; }
        }
        
        //-------------------------------------------------------------
        // Dependencies
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Class constants
        //-------------------------------------------------------------

        /// <summary>
        /// If there are no value weight data in settings, that value will be used.
        /// </summary>
        private const int cFallbackPowerOf2Value = 0;
        
        //-------------------------------------------------------------
        // Class variables
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Class methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Constructor/finalizer
        //-------------------------------------------------------------

        public TileValueGenerator(Settings settings) {
            _settings = settings;

            _totalWeight = settings.TileValueWeights
                .Sum(x => x.Weight);
        }
        
        //-------------------------------------------------------------
        // Variables
        //-------------------------------------------------------------

        private readonly Settings _settings;
        
        /// <summary>
        /// Sum of the weights of all weighted tile values in settings.
        /// </summary>
        private readonly int _totalWeight;
        
        /// <summary>
        /// If false, fallback value-related error will be shown once.
        /// </summary>
        private bool _fallbackErrorShown = false;
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------

        /// <summary>
        /// Generates a new tile value based on the value weight data in settings.
        /// </summary>
        /// <returns>Generated value or fallback value if value can't be generated for some reason.</returns>
        public int Generate() {
            // calculate random weighted tile value
            var randomValue = Random.Range(1, _totalWeight + 1);
            var currentWeight = 0;
            foreach (var weightData in _settings.TileValueWeights) {
                currentWeight += weightData.Weight;
                if (randomValue <= currentWeight)
                    return weightData.PowerOf2Value;
            }
            
            // fallback
            if (!_fallbackErrorShown) {
                Debug.LogError($"For some reason tile value can't be generated, fallback value {cFallbackPowerOf2Value} " +
                               "will be used");
                _fallbackErrorShown = true;
            }
            
            return cFallbackPowerOf2Value;
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

using System;
using System.Drawing;

namespace SampleGame2048 {
    
    /// <summary>
    /// 2D array that provides easy access using the 2D tile index.
    /// </summary>
    public class Array2D<T> {
        
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
        
        public Array2D(Size size) {
            Size = size;
            
            _source = new T[size.Height, size.Width];
        }
        public Array2D(int width, int height) : this(new Size(width, height)) {
        }
        
        //-------------------------------------------------------------
        // Variables
        //-------------------------------------------------------------

        private readonly T[,] _source;
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------

        public Size Size { get; }

        public T this[int i, int j] {
            get => _source[i, j];
            set => _source[i, j] = value;
        }
        public T this[BoardTileIndex tileIndex] {
            get => _source[tileIndex.I, tileIndex.J];
            set => _source[tileIndex.I, tileIndex.J] = value;
        }
        
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
        /// Converts 2D array to a readonly version.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyArray2D<T> AsReadOnly() => new ReadOnlyArray2D<T>(this);

        /// <summary>
        /// Executes an action over each item in the 2D array.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="action" /> is <see langword="null" />
        /// </exception>
        public void ForEach(Action<T> action) {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            
            foreach (var sourceItem in _source) {
                action(sourceItem);
            }
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

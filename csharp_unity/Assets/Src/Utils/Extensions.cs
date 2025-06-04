using System;
using System.Collections.Generic;
using System.Linq;

namespace sample_game.utils {
    
    /// <summary>
    /// Various class extensions
    /// </summary>
    public static class Extensions {
        
        /// <summary>
        /// Returns a new container with shuffled elements.
        /// Source: https://stackoverflow.com/a/4262134
        /// </summary>
        /// <param name="source">Elements of this container will be shuffled.</param>
        /// <typeparam name="T">Element type.</typeparam>
        /// <returns>Container with shuffled elements.</returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            var rnd = new Random();
            return source.OrderBy(item => rnd.Next());
        }
    }
} // namespace sample_game.utils
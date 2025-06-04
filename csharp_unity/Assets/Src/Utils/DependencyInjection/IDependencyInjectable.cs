using System;

namespace sample_game.utils {
    
    /// <summary>
    /// Common interface for objects that support runtime dependency injection.
    /// </summary>
    public interface IDependencyInjectable {
        
        /// <summary>
        /// Dispatched when all dependencies were fulfilled and dependency injectable object is ready to use.
        /// </summary>
        event EventHandler AllDependenciesFulfilled;
        
        /// <summary>
        /// If true, all dependencies were fulfilled.
        /// </summary>
        bool dependenciesFulfilled { get; }
    }
} // namespace sample_game.utils
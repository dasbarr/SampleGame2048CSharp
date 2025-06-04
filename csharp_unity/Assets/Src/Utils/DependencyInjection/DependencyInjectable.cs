using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace sample_game.utils {
    
    /// <summary>
    /// Non-unity class that supports runtime dependency injection.
    /// </summary>
    public abstract class DependencyInjectable : IDependencyInjectable {
        
        //-------------------------------------------------------------
        // Nested
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
        // Constructor
        //-------------------------------------------------------------
        
        protected DependencyInjectable() {
            _notFulfilledDependencyFields = DependencyInjector.CollectInjectionFields(this);
            PerformInjection();
        }
        
        //-------------------------------------------------------------
        // Destructor
        //-------------------------------------------------------------
        
        ~DependencyInjectable() {
            // object no longer needs dependency fulfillment
            DependencyInjector.DependenciesListUpdated -= PerformInjection;
        }
        
        //-------------------------------------------------------------
        // Variables
        //-------------------------------------------------------------

        /// <summary>
        /// Contains all dependency injection fields that weren't fulfilled yet.
        /// </summary>
        private HashSet<FieldInfo> _notFulfilledDependencyFields = null;
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
        public event EventHandler AllDependenciesFulfilled;
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------
        
        public bool dependenciesFulfilled => _notFulfilledDependencyFields != null && !_notFulfilledDependencyFields.Any();

        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Protected methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private methods
        //-------------------------------------------------------------
        
        /// <summary>
        /// Injects dependencies to the current instance.
        /// </summary>
        private void PerformInjection() {
            // unsubscribe to avoid multiple subscription
            DependencyInjector.DependenciesListUpdated -= PerformInjection;
            
            DependencyInjector.Inject(this, ref _notFulfilledDependencyFields);

            if (dependenciesFulfilled) {
                // all dependencies were fulfilled
                OnDependenciesFulfilled();
                AllDependenciesFulfilled?.Invoke(this, EventArgs.Empty);
            }
            else {
                // maybe dependencies will be fulfilled later
                DependencyInjector.DependenciesListUpdated += PerformInjection;
            }
        }
        
        //-------------------------------------------------------------
        // Handlers
        //-------------------------------------------------------------

        /// <summary>
        /// Can contain specific actions for case when all dependencies are fulfilled.
        /// </summary>
        protected virtual void OnDependenciesFulfilled() {
            // override if necessary
        }
    }
} // namespace sample_game.utils
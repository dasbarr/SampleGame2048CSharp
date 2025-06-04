using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace sample_game.utils {
    
    /// <summary>
    /// Class that performs runtime dependency injection.
    /// </summary>
    public static class DependencyInjector {
        
        //-------------------------------------------------------------
        // Nested
        //-------------------------------------------------------------

        /// <summary>
        /// Attribute for dependency fields.
        /// </summary>
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class DependencyAttribute : Attribute {
            // no implementation needed
        }
        
        //-------------------------------------------------------------
        // Class constants
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Class variables
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Class events
        //-------------------------------------------------------------

        /// <summary>
        /// Dispatched when number of available dependencies updated, and now maybe it's possible to fulfill
        /// dependencies that couldn't be fulfilled before.
        /// </summary>
        public static event Action DependenciesListUpdated;
        
        //-------------------------------------------------------------
        // Public class methods
        //-------------------------------------------------------------

        /// <summary>
        /// Gets all dependency injection fields within a specific object.
        /// </summary>
        /// <param name="dependencyInjectable">Dependency injection fields of that object will be collected.</param>
        /// <returns>A collection that contains collected dependency injection fields.</returns>
        public static HashSet<FieldInfo> CollectInjectionFields(IDependencyInjectable dependencyInjectable) {
            var collectedFields = new HashSet<FieldInfo>();
            
            // If dependencyInjectable has some private dependency fields in parent classes, that fields
            // can't be obtained through DependencyInjectable type - we need to traverse over all parent types.
            // Also use BindingFlags.DeclaredOnly flag to get rid of duplicates.
            var currentType = dependencyInjectable.GetType();
            do {
                // get dependency fields in current type that should be fulfilled
                var dependencyFieldsToFulfillInCurrentType = currentType.GetFields(
                    BindingFlags.Public | BindingFlags.NonPublic
                    | BindingFlags.Instance | BindingFlags.DeclaredOnly
                ).Where(
                    // get only fields that has needed attribute and need to be fulfilled
                    fieldInfo => fieldInfo.GetCustomAttribute<DependencyAttribute>() != null
                                 && fieldInfo.GetValue(dependencyInjectable) == null
                );
                
                collectedFields.UnionWith(dependencyFieldsToFulfillInCurrentType);

                // then start search for dependency fields in parent type
                currentType = currentType.BaseType;
            } while (currentType != null && typeof(IDependencyInjectable).IsAssignableFrom(currentType));

            return collectedFields;
        }

        /// <summary>
        /// Injects dependencies if possible.
        /// </summary>
        /// <param name="dependencyInjectable">Injection destination.</param>
        /// <param name="fieldsToInject">
        /// Reference parameter, contains fields that should be fulfilled with dependencies. If dependency was
        /// injected successfully, that field will be removed from that collection.
        /// </param>
        public static void Inject(IDependencyInjectable dependencyInjectable, ref HashSet<FieldInfo> fieldsToInject) {
            // make a copy (we will change ref collection during injection)
            foreach (var field in fieldsToInject.ToArray()) {
                var dependency = field.GetValue(dependencyInjectable);
                
                // try to perform injection (if necessary and if possible)
                if (dependency == default && ServiceLocator.TryGetService(field.FieldType, out dependency))
                    field.SetValue(dependencyInjectable, dependency);

                if (dependency != default) {
                    // dependency fulfilled, remove field
                    fieldsToInject.Remove(field);
                }
            }
        }

        //-------------------------------------------------------------
        // Constructor
        //-------------------------------------------------------------
        
        static DependencyInjector() {
            ServiceLocator.NewServiceRegistered += OnNewServiceRegistered;
        }
        
        //-------------------------------------------------------------
        // Handlers
        //-------------------------------------------------------------
        
        private static void OnNewServiceRegistered() {
            DependenciesListUpdated?.Invoke();
        }
    }
} // namespace sample_game.utils
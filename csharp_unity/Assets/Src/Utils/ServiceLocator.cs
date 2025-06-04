using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace sample_game.utils {
    
    /// <summary>
    /// Stores and provides services for an application.
    /// </summary>
    public class ServiceLocator {
        
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

        private static readonly ServiceLocator sInstance = new ServiceLocator();
        
        //-------------------------------------------------------------
        // Class events
        //-------------------------------------------------------------
        
        /// <summary>
        /// Dispatched when a new service was registered in the locator.
        /// </summary>
        public static event Action NewServiceRegistered;
        
        //-------------------------------------------------------------
        // Class methods
        //-------------------------------------------------------------

        /// <summary>
        /// Adds a service in the locator. Type for addition may differ from actual service type (useful for interfaces).
        /// </summary>
        /// <param name="service">Service to add.</param>
        /// <typeparam name="T">Service will be added with that type.</typeparam>
        public static void AddService<T>(object service) {
            sInstance.InternalAddService(service, typeof(T));
        }
        public static void AddService(object service) {
            sInstance.InternalAddService(service, service.GetType());
        }

        /// <summary>
        /// Gets a registered service if it was registered.
        /// </summary>
        /// <param name="serviceType">Service type.</param>
        /// <param name="service">Output parameter, will contain requested service (if that service can be acquired).</param>
        /// <returns>True if requested service can be acquired, false otherwise.</returns>
        public static bool TryGetService(Type serviceType, out object service) {
            return sInstance._registeredServices.TryGetValue(serviceType, out service);
        }
        
        //-------------------------------------------------------------
        // Constructor/destructor
        //-------------------------------------------------------------

        private ServiceLocator() {
            // no implementation needed
        }
        
        //-------------------------------------------------------------
        // Variables
        //-------------------------------------------------------------

        /// <summary>
        /// Services that were registered and are available for external code.
        /// </summary>
        private readonly Dictionary<Type, object> _registeredServices = new Dictionary<Type, object>();
        /// <summary>
        /// Services that are waiting for registration.
        /// </summary>
        private readonly Dictionary<Type, object> _delayedRegistrationServices = new Dictionary<Type, object>();
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------

        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------
        
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

        /// <summary>
        /// Adds a service to the locator. If added service is ready to use right now, immediately registers it
        /// (after registration service will be available for the external code). If not, service will be added as
        /// 'service with delayed registration' and will be registered when it will be ready.
        /// </summary>
        /// <param name="service">Service to add.</param>
        /// <param name="type">Service will be added with that type.</param>
        private void InternalAddService(object service, Type type) {
            if (_registeredServices.ContainsKey(type) || _delayedRegistrationServices.ContainsKey(type)) {
                Debug.LogWarning("Service was already registered for the type '" + type.Name + "'");
                return;
            }

            if (service is IDependencyInjectable dependencyInjectableService
                && !dependencyInjectableService.dependenciesFulfilled
            ) {
                // delayed registration (we need to store service here because registration type can differ from
                // the actual service type)
                _delayedRegistrationServices[type] = dependencyInjectableService;
                dependencyInjectableService.AllDependenciesFulfilled += OnServiceDependenciesFulfilled;
            }
            else {
                // register service immediately
                InternalRegisterService(service, type);
            }
        }

        /// <summary>
        /// Registers the service.
        /// </summary>
        /// <param name="service">Service to register.</param>
        /// <param name="type"></param>
        private void InternalRegisterService(object service, Type type) {
            _registeredServices[type] = service;
            NewServiceRegistered?.Invoke();
        }
        
        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Handlers
        //-------------------------------------------------------------
        
        private void OnServiceDependenciesFulfilled(object delayedService, EventArgs _) {
            if (!(delayedService is IDependencyInjectable dependencyInjectableDelayedService))
                return;
            
            dependencyInjectableDelayedService.AllDependenciesFulfilled -= OnServiceDependenciesFulfilled;

            // get service type here because it can differ from the actual service type
            var delayedServiceType = _delayedRegistrationServices.First(serviceEntry => serviceEntry.Value == delayedService).Key;
            _delayedRegistrationServices.Remove(delayedServiceType);
            
            // register service
            InternalRegisterService(delayedService, delayedServiceType);
        }
    }
} // namespace sample_game.utils
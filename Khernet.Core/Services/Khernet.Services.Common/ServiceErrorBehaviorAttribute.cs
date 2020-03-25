using System;
using System.ServiceModel;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Khernet.Services.Common
{
    //Attribute to register and error handler to a WCF service
    public class ServiceErrorBehaviorAttribute : Attribute, IServiceBehavior
    {
        private readonly Type erroType;
        public ServiceErrorBehaviorAttribute(Type errorHandlerType)
        {
            this.erroType = errorHandlerType;
        }

        //Allow add custom binding elements to support contract implementation
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {

        }

        /// <summary>
        /// Allow change property values or insert custom extension objects at runtime like:
        /// managers, message and parameter interceptors, secutiry extensions.
        /// </summary>
        /// <param name="serviceDescription">The service description</param>
        /// <param name="serviceHostBase">The host of service</param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            IErrorHandler errorHandler = null;
            errorHandler = (IErrorHandler)Activator.CreateInstance(erroType);

            foreach (ChannelDispatcherBase channelBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = channelBase as ChannelDispatcher;

                if (channelDispatcher.Endpoints.Any((endpointDispatcher) => endpointDispatcher.IsSystemEndpoint))
                    continue;

                channelDispatcher.ErrorHandlers.Add(errorHandler);
            }
        }

        //Confirm if current service can be executed within given environment
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {

        }
    }
}
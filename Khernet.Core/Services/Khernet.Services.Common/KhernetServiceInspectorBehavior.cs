using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Khernet.Services.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class KhernetServiceInspectorBehavior : Attribute, IDispatchMessageInspector, IServiceBehavior
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {

        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher dispacher in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher endpoint in dispacher.Endpoints)
                {

                    endpoint.DispatchRuntime.MessageInspectors.Add(new KhernetServiceInspectorBehavior());
                }
            }
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {

        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            //Verify options of services host
        }
    }
}

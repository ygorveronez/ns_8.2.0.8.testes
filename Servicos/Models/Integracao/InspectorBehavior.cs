using System;
using System.Collections.Generic;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Servicos.Models.Integracao
{
    public class InspectorBehavior : IEndpointBehavior
    {
        private bool RemoveActionHeader { get; set; }
        public InspectorBehavior(bool removeActionHeader = false)
        {
            RemoveActionHeader = removeActionHeader;
        }

        public string LastRequestXML
        {
            get
            {
                return _messageInspector.LastRequestXML;
            }
        }

        public string LastResponseXML
        {
            get
            {
                return _messageInspector.LastResponseXML;
            }
        }

        private readonly CustomMessageInspector _messageInspector = new CustomMessageInspector();
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {

        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            _messageInspector.RemoveActionHeader = RemoveActionHeader;

            clientRuntime.ClientMessageInspectors.Add(_messageInspector);
        }
    }

    public class CustomMessageInspector : IClientMessageInspector
    {
        public bool RemoveActionHeader { get; set; }
        public string LastRequestXML { get; private set; }
        public string LastResponseXML { get; private set; }
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            LastResponseXML = reply.ToString();
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            if (RemoveActionHeader)
            {
                int headerIndexOfAction = request.Headers.FindHeader("Action", "http://schemas.microsoft.com/ws/2005/05/addressing/none");
                request.Headers.RemoveAt(headerIndexOfAction);
            }

            LastRequestXML = request.ToString();

            return request;
        }
    }
}

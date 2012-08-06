using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;

namespace SimpleCqrs.Rhino.ServiceBus
{
    public class RequestReplyMessageOwnersSelector : MessageOwnersSelector, IMessageOwnersSelector
    {
        private readonly MessageOwner[] messageOwners;
        private readonly IEndpointRouter endpointRouter;

        public RequestReplyMessageOwnersSelector(MessageOwner[] messageOwners, IEndpointRouter endpointRouter)
            : base(messageOwners, endpointRouter)
        {
            this.messageOwners = messageOwners;
            this.endpointRouter = endpointRouter;
        }

        Endpoint IMessageOwnersSelector.GetEndpointForMessageBatch(object[] messages)
        {
            if(messages == null)
                throw new ArgumentNullException("messages");

            if(messages.Length == 0)
                throw new MessagePublicationException("Cannot send empty message batch");

            var messageType = messages[0].GetType();
            if(messageType.IsGenericType && (messageType.GetGenericTypeDefinition() == typeof(Reply<>) || messageType.GetGenericTypeDefinition() == typeof(Request<>)))
            {
                var messageOwner = messageOwners.FirstOrDefault(x => x.IsOwner(messageType.GetGenericArguments()[0]));

                var endpoint = endpointRouter.GetRoutedEndpoint(messageOwner.Endpoint);
                endpoint.Transactional = messageOwner.Transactional;
                return endpoint;
            }

            return GetEndpointForMessageBatch(messages);
        }

        IEnumerable<MessageOwner> IMessageOwnersSelector.Of(Type type)
        {
            if(type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Request<>) || type.GetGenericTypeDefinition() == typeof(Reply<>)))
                return Of(type.GetGenericArguments()[0]);

            return Of(type);
        }
    }
}
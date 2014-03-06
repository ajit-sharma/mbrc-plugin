using System.Linq;

namespace MusicBeePlugin.AndroidRemote.Networking
{
    using System.Collections.Generic;
    using Entities;
    using ServiceStack.Text;
    using Events;
    using System;
    using System.Diagnostics;
    using Error;
    using Utilities;

    internal class ProtocolHandler
    {
        /// <summary>
        /// Processes the incoming message and answer's sending back the needed data.
        /// </summary>
        /// <param name="messages">The incoming message.</param>
        /// <param name="clientId"> </param>
        public void ProcessIncomingMessage(List<string> messages, string clientId)
        {
            try
            {
                var msgList = new List<SocketMessage>();
                
                try
                {
                    msgList.AddRange(messages.Select(msg => new SocketMessage(JsonObject.Parse(msg))));
                }
                catch (Exception ex)
                {
#if DEBUG
                    ErrorHandler.LogError(ex);
                    Debug.WriteLine("elements: {0}",messages.Count);
                    Debug.WriteLine("-----------------------------");
                    foreach (var message in messages)
                    {
                        Debug.WriteLine("message:" + message);
                    }
#endif                 
                }

                foreach (var msg in msgList)
                {
                    if (Authenticator.Client(clientId).PacketNumber == 0 && msg.context != Constants.Player)
                    {
                        EventBus.FireEvent(new MessageEvent(EventType.ActionForceClientDisconnect, string.Empty, clientId));
                        return;
                    }
                    if (Authenticator.Client(clientId).PacketNumber == 1 && msg.context != Constants.Protocol)
                    {
                        EventBus.FireEvent(new MessageEvent(EventType.ActionForceClientDisconnect, string.Empty, clientId));
                        return;
                    }

                    EventBus.FireEvent(new MessageEvent(msg.context, msg.data, clientId));
                }
                Authenticator.Client(clientId).IncreasePacketNumber();
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("Exception: " + ex);
                ErrorHandler.LogError(ex);
#endif
            }
        }
    }
}
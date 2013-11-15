using System.Collections.Generic;
using MusicBeePlugin.AndroidRemote.Entities;
using ServiceStack.Text;

namespace MusicBeePlugin.AndroidRemote.Networking
{
    using Events;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using Error;
    using Utilities;

    internal class ProtocolHandler
    {


        public ProtocolHandler()
        {
         
        }

        /// <summary>
        /// Processes the incoming message and answer's sending back the needed data.
        /// </summary>
        /// <param name="messages">The incoming message.</param>
        /// <param name="clientId"> </param>
        public void ProcessIncomingMessage(List<string> messages, string clientId)
        {
            try
            {
                List<SocketMessage> msgList = new List<SocketMessage>();
                
                try
                {                    
                    foreach (string msg in messages)
                    {
                        msgList.Add(new SocketMessage(JsonObject.Parse(msg)));
                    }
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

                foreach (SocketMessage msg in msgList)
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
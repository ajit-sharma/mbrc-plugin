namespace MusicBeePlugin.AndroidRemote.Networking
{
    using System.Linq;
    using NLog;
    using System.Collections.Generic;
    using Entities;
    using ServiceStack.Text;
    using Events;
    using System;
    using Utilities;

    internal class ProtocolHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
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
                    Logger.Debug(ex);
                    Logger.Info("elements: {0}", messages.Count);
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
                Logger.Debug(ex);
            }
        }
    }
}
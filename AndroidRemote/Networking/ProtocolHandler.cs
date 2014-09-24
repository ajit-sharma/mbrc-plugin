#region

using System;
using System.Collections.Generic;
using System.Linq;
using MusicBeePlugin.AndroidRemote.Entities;
using MusicBeePlugin.AndroidRemote.Events;
using MusicBeePlugin.AndroidRemote.Utilities;
using NLog;
using ServiceStack.Text;

#endregion

namespace MusicBeePlugin.AndroidRemote.Networking
{
    internal class ProtocolHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Processes the incoming message and answer's sending back the needed data.
        /// </summary>
        /// <param name="messages">The incoming message.</param>
        /// <param name="clientId"> </param>
        public void ProcessIncomingMessage(List<string> messages, string clientId)
        {
            try
            {
                var msgList = new List<NotificationMessage>();

                try
                {
                    msgList.AddRange(messages.Select(msg => new NotificationMessage(JsonObject.Parse(msg))));
                }
                catch (Exception ex)
                {
                    Logger.DebugException("Incoming:32", ex);
                    Logger.Info("elements: {0}", messages.Count);
                }

                foreach (var msg in msgList)
                {
                    if (Authenticator.Client(clientId).PacketNumber == 0 && msg.Message != Constants.Player)
                    {
                        EventBus.FireEvent(new MessageEvent(EventType.ActionForceClientDisconnect, string.Empty,
                            clientId));
                        return;
                    }
                    if (Authenticator.Client(clientId).PacketNumber == 1 && msg.Message != Constants.Protocol)
                    {
                        EventBus.FireEvent(new MessageEvent(EventType.ActionForceClientDisconnect, string.Empty,
                            clientId));
                        return;
                    }

                    EventBus.FireEvent(new MessageEvent(msg.Message));
                }
                Authenticator.Client(clientId).IncreasePacketNumber();
            }
            catch (Exception ex)
            {
                Logger.DebugException("Incoming:55", ex);
            }
        }
    }
}
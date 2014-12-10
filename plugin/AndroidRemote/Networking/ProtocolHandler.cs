#region

using MusicBeePlugin.AndroidRemote.Entities;
using MusicBeePlugin.AndroidRemote.Events;
using NLog;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    EventBus.FireEvent(new MessageEvent(msg.Message));
                }

            }
            catch (Exception ex)
            {
                Logger.DebugException("Incoming:55", ex);
            }
        }
    }
}
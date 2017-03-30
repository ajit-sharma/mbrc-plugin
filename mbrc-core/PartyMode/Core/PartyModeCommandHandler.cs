﻿using MusicBeeRemoteCore.PartyMode.Core.Model;
using MusicBeeRemoteCore.Remote.Commands;
using MusicBeeRemoteCore.Remote.Commands.Internal;
using MusicBeeRemoteCore.Remote.Interfaces;
using MusicBeeRemoteCore.Remote.Networking;
using TinyMessenger;

namespace MusicBeeRemoteCore.PartyMode.Core
{
    public class PartyModeCommandHandler
    {
        private readonly ITinyMessengerHub _hub;
        private readonly PartyModeModel _partyModeModel;
          
        public PartyModeCommandHandler(ITinyMessengerHub hub, PartyModeModel partyModeModel)
        {
            _hub = hub;
            _partyModeModel = partyModeModel;
            _hub.Subscribe<ConnectionReadyEvent>(msg => OnClientConnected(msg.Client));
        }

        public bool PartyModeActive => _partyModeModel.Settings.IsActive;

        private void OnClientConnected(SocketConnection connection)
        {
            // A connection where broadcast is disabled is from a secondary connection of an existing client.
            // Each client should only have one active broadcast enabled connection that is the main communication
            // channel.
            if (!connection.BroadcastsEnabled)
            {
                return;
            }

            _partyModeModel.AddClientIfNotExists(connection);
        }

        public void OnClientDisconnected(RemoteClient client)
        {
            _partyModeModel.RemoveConnection(client);
        }

        public void LogActivity(string client, string command, bool isCmdAllowed)
        {
            _partyModeModel.LogCommand(new ServerCommandEventArgs(client, command, isCmdAllowed));
        }

        public bool HasPermissions(ICommand command, IEvent @event)
        {
            var limitedCommand = command as LimitedCommand;
            if (limitedCommand == null)
            {
                return true;
            }

            var client = _partyModeModel.GetClient(@event.ClientId);
            var hasPermissions = client.HasPermission(limitedCommand.GetPermissions());
            LogActivity(@event.ClientId, @event.Type, hasPermissions);
            return hasPermissions;
        }
    }
}
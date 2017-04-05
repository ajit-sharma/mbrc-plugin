﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MusicBeeRemote.Core.Network;
using MusicBeeRemote.Core.Windows.Mvvm;
using MusicBeeRemote.PartyMode.Core.Helper;
using MusicBeeRemote.PartyMode.Core.Tools;
using MusicBeeRemote.PartyMode.Core.ViewModel;
using NLog;

namespace MusicBeeRemote.PartyMode.Core.Model
{
    public class PartyModeModel : ViewModelBase
    {
        #region vars

        private readonly SettingsHandler _handler;

        private readonly CycledList<PartyModeLogs> _logs;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion vars

        public PartyModeModel(SettingsHandler handler)
        {
            _handler = handler;
            Settings = _handler.GetSettings();

            // Load stored
            KnownClients = Settings.KnownClients;

            _logs = new CycledList<PartyModeLogs>(10000);
            ServerMessagesQueue = new ConcurrentQueue<PartyModeLogs>();
        }

        public void AddClientIfNotExists(SocketConnection connection)
        {
            var ipadress = connection.IpAddress;
            var clientId = connection.ClientId;

            if (!KnownClients.Any())
            {
                KnownClients.Add(CreateClient(ipadress, clientId));
            }
            else
            {
                var client = KnownClients.SingleOrDefault(x => x.ClientId == clientId);
                if (client != null)
                {
                    client.AddConnection();
                }
                else
                {
                    KnownClients.Add(CreateClient(ipadress, clientId));
                }

            }

            LogCommand(new ServerCommandEventArgs(clientId, "New connection", true));

            OnPropertyChanged(nameof(KnownClients));
        }

        private static RemoteClient CreateClient(IPAddress ipadress, string clientId)
        {
            var client = new RemoteClient(PartyModeNetworkTools.GetMacAddress(ipadress), ipadress)
            {
                ClientId = clientId
            };
            client.AddConnection();
            return client;
        }

        public Settings Settings { get; }

        public List<RemoteClient> KnownClients { get; }

        public void LogCommand(ServerCommandEventArgs e)
        {
            if (e.Command == Constants.Ping || e.Command == Constants.Pong)
            {
                return;
            }

            var logMessages = new PartyModeLogs(e.Client, e.Command, !e.IsCommandAllowed);
            _logs.Add(logMessages);
            ServerMessagesQueue.Enqueue(logMessages);
            OnPropertyChanged(nameof(ServerMessagesQueue));
        }

        public ConcurrentQueue<PartyModeLogs> ServerMessagesQueue;

        public void SaveSettings()
        {
            Settings.KnownClients = KnownClients;
            _handler.SaveSettings(Settings);
        }

        public void RequestAllServerMessages()
        {
            ServerMessagesQueue = new ConcurrentQueue<PartyModeLogs>(_logs);
            OnPropertyChanged(nameof(ServerMessagesQueue));
        }

        public void RemoveConnection(RemoteClient client)
        {
            var savedClient = KnownClients.SingleOrDefault(x => x.ClientId == client.ClientId);
            savedClient?.RemoveConnection();
            LogCommand(new ServerCommandEventArgs(client.ClientId, "connection closed", true));
        }

        public RemoteClient GetClient(string clientId)
        {
            return KnownClients.SingleOrDefault(x => x.ClientId == clientId);
        }
    }
}
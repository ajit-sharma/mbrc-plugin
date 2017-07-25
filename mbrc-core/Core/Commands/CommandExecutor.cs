﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using MusicBeeRemote.Core.Events;
using MusicBeeRemote.Core.Model.Entities;
using MusicBeeRemote.Core.Network;
using TinyMessenger;

namespace MusicBeeRemote.Core.Commands
{
    public class CommandExecutor
    {
        private readonly Dictionary<string, ICommand> _commandMap = new Dictionary<string, ICommand>();
        private readonly ITinyMessengerHub _hub;
        private readonly ExecutorHelper _executorHelper;


        public CommandExecutor(ITinyMessengerHub hub, ExecutorHelper executorHelper)
        {
            _hub = hub;
            _executorHelper = executorHelper;
            _hub.Subscribe<MessageEvent>(CommandExecute);
        }

        public void AddCommand(string eventType, ICommand command)
        {
            if (_commandMap.ContainsKey(eventType)) return;
            _commandMap.Add(eventType, command);
        }

        public void RemoveCommand(string eventType)
        {
            if (_commandMap.ContainsKey(eventType))
                _commandMap.Remove(eventType);
        }

        private void CommandExecute(IEvent e)
        {
            if (e?.Type == null)
            {
#if DEBUG
                Debug.WriteLine("failed to execute command due to missing type/or event");
#endif
                return;
            }

            if (!_commandMap.ContainsKey(e.Type)) return;
            var command = _commandMap[e.Type];
            try
            {
                Execute(e, command);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
#endif
                // Oh noes something went wrong... let's ignore the exception?
            }
        }

        private void Execute(IEvent e, ICommand command)
        {
            if (_executorHelper.PermissionMode)
            {
                PermissionBasedExecution(e, command);
            }
            else
            {
                Debug.WriteLine($"Running {command.GetType()}");
                command.Execute(e);
            }
        }

        private void PermissionBasedExecution(IEvent e, ICommand command)
        {
            var limited = command as LimitedCommand;
            if (limited != null)
            {
                var status = limited.Execute(e, _executorHelper.ClientPermissions(e.ClientId));
                if (status != ExecutionStatus.Denied)
                {
                    return;
                }
                var message = new SocketMessage(Constants.CommandUnavailable);
                _hub.Publish(new PluginResponseAvailableEvent(message));
            }
            else
            {
                command.Execute(e);
            }
        }
    }
}
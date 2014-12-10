#region

using System;
using System.Collections.Generic;
using MusicBeePlugin.AndroidRemote.Interfaces;
using Ninject;
using NLog;

#endregion

namespace MusicBeePlugin.AndroidRemote.Controller
{
    public class Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<string, Type> _commandMap;
        private IKernel _kernel;

        public Controller()
        {
            _commandMap = new Dictionary<string, Type>();
        }

        public void AddCommand(string eventType, Type command)
        {
            if (_commandMap.ContainsKey(eventType)) return;
            _commandMap.Add(eventType, command);
        }

        public void RemoveCommand(string eventType)
        {
            if (_commandMap.ContainsKey(eventType))
                _commandMap.Remove(eventType);
        }

        public void CommandExecute(IEvent e)
        {
            if (!_commandMap.ContainsKey(e.Type)) return;
            var commandType = _commandMap[e.Type];
            var command = (ICommand) _kernel.Get(commandType);

            try
            {
                command.Execute(e);
            }
            catch (Exception ex)
            {
                Logger.TraceException("Controller", ex);
            }
        }

        public void InjectKernel(IKernel kernel)
        {
            _kernel = kernel;
        }
    }
}
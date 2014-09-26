using System.Data;
using Ninject;

namespace MusicBeePlugin.AndroidRemote.Controller
{
    using NLog;
    using System;
    using System.Collections.Generic;
    using Interfaces;

    public class Controller
    {
        private readonly IKernel _kernel;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<string, Type> _commandMap; 

        public void AddCommand(string eventType,Type command)
        {
            if (_commandMap.ContainsKey(eventType)) return;
            _commandMap.Add(eventType,command);
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
            using (var command = (ICommand)_kernel.Get(commandType))
            {
                try
                {
                    command.Execute(e);
                }
                catch (Exception ex)
                {
                    Logger.TraceException("Controller",ex);
                }
                
            }
        }

        public Controller(IKernel kernel)
        {
            _kernel = kernel;
            _commandMap = new Dictionary<string, Type>();
        }
    }
}
namespace MusicBeePlugin.AndroidRemote.Controller
{
    using NLog;
    using System;
    using System.Collections.Generic;
    using Interfaces;
 
    internal class Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<string, Type> _commandMap; 

        private static readonly Controller ClassInstance = new Controller();

        public static Controller Instance
        {
            get { return ClassInstance; }
        }

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
            using (var command = (ICommand)Activator.CreateInstance(commandType))
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

        private Controller()
        {
            _commandMap = new Dictionary<string, Type>();
        }
    }
}
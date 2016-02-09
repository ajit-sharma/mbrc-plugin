namespace MusicBeePlugin.AndroidRemote.Controller
{
    using System;
    using System.Collections.Generic;

    using MusicBeePlugin.AndroidRemote.Interfaces;

    using Ninject;

    using NLog;

    public class Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, Type> _commandMap;

        private IKernel _kernel;

        public Controller()
        {
            this._commandMap = new Dictionary<string, Type>();
        }

        public void AddCommand(string eventType, Type command)
        {
            if (this._commandMap.ContainsKey(eventType))
            {
                return;
            }

            this._commandMap.Add(eventType, command);
        }

        public void CommandExecute(IEvent e)
        {
            if (!this._commandMap.ContainsKey(e.Type))
            {
                return;
            }

            var commandType = this._commandMap[e.Type];
            var command = (ICommand)this._kernel.Get(commandType);

            try
            {
                command.Execute(e);
            }
            catch (Exception ex)
            {
                Logger.Trace(ex, "Controller");
            }
        }

        public void InjectKernel(IKernel kernel)
        {
            this._kernel = kernel;
        }

        public void RemoveCommand(string eventType)
        {
            if (this._commandMap.ContainsKey(eventType))
            {
                this._commandMap.Remove(eventType);
            }
        }
    }
}
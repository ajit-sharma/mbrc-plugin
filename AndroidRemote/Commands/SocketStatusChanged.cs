#region

using MusicBeePlugin.AndroidRemote.Interfaces;
using System;

#endregion

namespace MusicBeePlugin.AndroidRemote.Commands
{
    internal class SocketStatusChanged : ICommand
    {
        public void Execute(IEvent eEvent)
        {
            Plugin.Instance.UpdateWindowStatus((Boolean)eEvent.Data);
        }
    }
}
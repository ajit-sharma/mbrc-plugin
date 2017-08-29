using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MusicBeeRemote.Core.Settings
{
    [DataContract]
    public class Configuration
    {
        [DataMember(Name = "addresses")]
        public List<string> Addresses { get; set; }

        [DataMember(Name = "http")]
        public uint HttpPort { get; set; }

        [DataMember(Name = "websocket")]
        public uint WebSocket { get; set; }
    }
}
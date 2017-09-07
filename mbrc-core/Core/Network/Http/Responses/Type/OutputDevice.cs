using System.Runtime.Serialization;

namespace MusicBeeRemote.Core.Network.Http.Responses.Type
{
    [DataContract(Name = "output")]
    public class OutputDevice
    {
        public OutputDevice(string[] deviceNames, string activeDeviceName)
        {
            DeviceNames = deviceNames;
            ActiveDeviceName = activeDeviceName;
        }

        [DataMember(Name = "active")]
        public string ActiveDeviceName { get; set; }

        [DataMember(Name = "devices")]
        public string[] DeviceNames { get; set; }
    }
}
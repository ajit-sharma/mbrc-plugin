using System.Runtime.Serialization;

namespace MusicBeeRemote.Core.Rest.ServiceModel.Type
{
    [DataContract]
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
using System.Runtime.Serialization;

namespace MusicBeeRemote.Core.Feature.Library
{
  [DataContract]
  public class Playlist
  {
    [DataMember(Name = "url")]
    public string Url { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }
  }
}
using System.Runtime.Serialization;

namespace MusicBeeRemote.Core.Network.Http.Responses.Type
{
    /// <summary>
    /// The position request body.
    /// </summary>
    [DataContract]
    public class PositionRequestBody
    {
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        [DataMember(Name = "position")]
        public int Position { get; set; }
    }
}
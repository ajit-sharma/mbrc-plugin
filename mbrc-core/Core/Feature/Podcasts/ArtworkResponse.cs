﻿using System.Runtime.Serialization;
using MusicBeeRemote.Core.Network.Http.Responses.Type;

namespace MusicBeeRemote.Core.Feature.Podcasts
{
    [DataContract]
    public class ArtworkResponse : ApiResponse
    {
        [DataMember(Name = "artwork")]
        public string Artwork { get; set; }
    }
}
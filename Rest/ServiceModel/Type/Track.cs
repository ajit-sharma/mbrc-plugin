using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    public class Track
    {
        public string artist { get; set; }
        public string title { get; set; }
        public string album { get; set; }
        public string year { get; set; }
    }
}

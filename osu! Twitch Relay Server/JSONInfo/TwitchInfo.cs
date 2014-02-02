using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace osu_Twitch_Relay_Server
{
    [DataContract]
    public class TwitchInfo
    {
        [DataMember(Name = "viewers_count")]
        public int viewers_count { get; set; }
    }
}

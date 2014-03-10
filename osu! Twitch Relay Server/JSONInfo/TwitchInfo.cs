using System.Runtime.Serialization;

namespace osu_Twitch_Relay_Server
{
    [DataContract]
    public class TwitchInfo
    {
        [DataMember(Name = "viewers_count")]
        public int viewers_count { get; set; }
    }
}

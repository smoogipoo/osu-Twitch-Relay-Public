using System.Runtime.Serialization;

namespace osu_Twitch_Relay_Server
{
    [DataContract]
    public class BeatmapInfo
    {
        [DataMember(Name = "artist")]
        public string artist { get; set; }

        [DataMember(Name = "title")]
        public string title { get; set; }
    }
}

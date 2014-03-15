using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace osu_Twitch_Relay
{
    class GlobalVars
    {
        public static string privKey = ""; //This must be set but has been removed for privacy reasons
#if DEBUG
        public static string server_IP = ""; //This must be set but has been removed for privacy reasons
#else
        public static string server_IP = ""; //This must be set but has been removed for privacy reasons
#endif
        
        public static int server_Port = 0; //This must be set but has been removed for privacy reasons
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using smgiFuncs;

namespace osu_Twitch_Relay_Server
{

    class GlobalVars
    {
        public static string privKey = ""; //This must be set but has been removed for privacy reasons
        public static string apiKey = ""; //This must be set to the osu! API key for your applications but has been removed for privacy reasons
        public static string osuIRC_Password = ""; //This must be set, but has been removed for privacy reasons. Can be found here -> https://osu.ppy.sh/p/ircauth (log onto website IRC if it tells you to connect to IRC)
        public static string osuIRC_Username = ""; //This must be set and is the bot's osu! username
        public static Socket oSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        volatile public static ConcurrentDictionary<sString, bool> oUsers = new ConcurrentDictionary<sString, bool>();  //oName,tName,TwitchToken, | False OR True
        volatile public static ConcurrentDictionary<sString, Socket> tUsers = new ConcurrentDictionary<sString, Socket>();  //oName,tName,TwitchToken, | TwitchState
        volatile public static List<Int64> oSendTimes = new List<Int64>(); //CurrentTime
        public static Settings settings = null;
        public static string email_Email = "";
        public static string email_Pass = "";
        public static StreamWriter logWriter = new StreamWriter(Environment.CurrentDirectory + "\\log.txt");

        public class ClientState
        {
            public Socket client = null;
            public byte[] buffer = new byte[65536];
        }
        public class tState
        {
            public Socket originalClient = null;
            public Socket client = null;
            public byte[] buffer = new byte[65536];
            public sString receivedstr = "";
        }
    }
}

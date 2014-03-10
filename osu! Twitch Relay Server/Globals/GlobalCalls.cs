using System;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace osu_Twitch_Relay_Server
{
    class GlobalCalls
    {
        public static T ParseOsuData<T>(string url) where T : class
        {
            T result;
            byte[] jsonBytes;
            using (WebClient wc = new WebClient())
            {
                jsonBytes = wc.DownloadData(url);
            }
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(jsonBytes))
            {
                result = (T)serializer.ReadObject(ms);
            }
            return result;
        }

        public static TwitchInfo ParseTwitchData(string tName)
        {
            TwitchInfo result;
            byte[] jsonBytes;
            using (WebClient wc = new WebClient())
            {
                jsonBytes = wc.DownloadData("http://api.justin.tv/api/stream/summary.json?channel=" + tName);
            }
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TwitchInfo));
            using (MemoryStream ms = new MemoryStream(jsonBytes))
            {
                result = (TwitchInfo)serializer.ReadObject(ms);
            }
            return result;
        }

        public static void AddToQueue(string msg)
        {
            WriteToConsole(Enum.GetName(typeof(Signals),Signals.TTO_MESSAGE_QUEUED));
            Int64 WaitTime = 0;
            if (GlobalVars.oSendTimes.Count != 0)
            {
                WaitTime = Math.Abs(1200 - ((Int64)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds - GlobalVars.oSendTimes[GlobalVars.oSendTimes.Count - 1]));
            }
            GlobalVars.oSendTimes.Add((Int64)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds + WaitTime);
            new Timer(oQueueSend, msg, WaitTime, Timeout.Infinite);
        }
        public static void oQueueSend(object Data)
        {
            WriteToConsole(Enum.GetName(typeof(Signals), Signals.TTO_MESSAGE_SENT));
            WriteToSocket(GlobalVars.oSock, Encoding.ASCII.GetBytes((string)Data));
            Thread.Sleep(1200);
            GlobalVars.oSendTimes.RemoveAt(0);
        }

        public static bool WriteToSocket(Socket sck, byte[] data)
        {
            try
            {
                sck.Send(data);
                return true;
            }
            catch { return false; }
        }

        public static void WriteToConsole(string message, int type = 0)
        {
            switch (type)
            {
                case 0:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case 1:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            Console.WriteLine(DateTime.Now + " - " + message);
            GlobalVars.logWriter.WriteLine(DateTime.Now + " - " + message);
            GlobalVars.logWriter.Flush();
        }
    }
}

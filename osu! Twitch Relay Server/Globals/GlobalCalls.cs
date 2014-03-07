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
    class GlobalCalls
    {
        public static T ParseOsuData<T>(string url) where T : class
        {
            T result = null;
            byte[] jsonBytes = null;
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
            TwitchInfo result = null;
            byte[] jsonBytes = null;
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
            Timer qSendTmr = new Timer(new TimerCallback(oQueueSend), msg, WaitTime, Timeout.Infinite);
        }
        public static void oQueueSend(object Data)
        {
            WriteToConsole(Enum.GetName(typeof(Signals), Signals.TTO_MESSAGE_SENT));
            WriteToSocket(GlobalVars.oSock, Encoding.ASCII.GetBytes((string)Data));
            System.Threading.Thread.Sleep(1200);
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
            Console.WriteLine(System.DateTime.Now.ToString() + " - " + message);
            GlobalVars.logWriter.WriteLine(System.DateTime.Now.ToString() + " - " + message);
            GlobalVars.logWriter.Flush();
        }
    }
}

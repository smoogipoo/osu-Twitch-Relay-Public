using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Threading.Tasks;
using smgiFuncs;

namespace osu_Twitch_Relay_Server
{
    class tServ
    {
        static bool trashb;
        static Socket trashs;

        public static void tConn(GlobalVars.tState state, bool retry = false, bool Authed = false)
        {
            Socket tempSck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                tempSck.Connect("irc.twitch.tv", 6667);
                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.TWITCH_CONNECT_SUCCESS),1);
                GlobalCalls.WriteToSocket(state.originalClient, Encoding.ASCII.GetBytes(Signals.TWITCH_CONNECT_SUCCESS.ToString()));
            }
            catch
            {
                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.TWITCH_CONNECT_FAIL),3);
                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.TWITCH_RECONNECTING_ONE), 2);
                GlobalCalls.WriteToSocket(state.originalClient, Encoding.ASCII.GetBytes(Signals.TWITCH_CONNECT_FAIL.ToString()));
                GlobalCalls.WriteToSocket(state.originalClient, Encoding.ASCII.GetBytes(Signals.TWITCH_RECONNECTING_ONE.ToString()));
                System.Threading.Thread.Sleep(1000);
                tConn(state, true);
            }
            if (tempSck.Connected == true)
            {
                bool alreadyAuthenticated = false;
                state.receivedstr = state.receivedstr.ToString().Replace(" ", "_");
                foreach (var user in GlobalVars.oUsers)
                {
                    if ((user.Key.SubString(0, user.Key.nthDexOf(",", 0)) == state.receivedstr.SubString(0, state.receivedstr.nthDexOf(",", 0))) || (user.Key.SubString(user.Key.nthDexOf(",", 0) + 1, user.Key.nthDexOf(",", 1)) == state.receivedstr.SubString(state.receivedstr.nthDexOf(",", 0) + 1, state.receivedstr.nthDexOf(",", 1))))
                    {
                        if (user.Value == true)
                        {
                            alreadyAuthenticated = true;
                            if (retry == true)
                            {
                                while (GlobalVars.tUsers.TryRemove(user.Key, out trashs) == false) ;
                                while (GlobalVars.tUsers.TryAdd(state.receivedstr, tempSck) == false) ;
                                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.TWITCH_RECONNECTED),1);
                                GlobalCalls.WriteToSocket(state.originalClient, Encoding.ASCII.GetBytes(Signals.TWITCH_RECONNECTED.ToString()));
                            }
                            else
                            {
                                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.USER_ALREADY_AUTHENTICATED),1);
                                GlobalCalls.WriteToSocket(state.originalClient, Encoding.ASCII.GetBytes(Signals.USER_ALREADY_AUTHENTICATED.ToString()));
                            }
                            break;
                        }
                        else
                        {
                            while (GlobalVars.oUsers.TryRemove(user.Key, out trashb) == false) ;
                            while (GlobalVars.tUsers.TryRemove(user.Key, out trashs) == false) ;
                        }

                    }
                }
                if (alreadyAuthenticated == false)
                {
                    if (Authed == true)
                    {
                        while (GlobalVars.oUsers.TryAdd(state.receivedstr, true) == false) ;
                        while (GlobalVars.tUsers.TryAdd(state.receivedstr, tempSck) == false) ;
                    }
                    else
                    {
                        while (GlobalVars.oUsers.TryAdd(state.receivedstr, false) == false) ;
                        while (GlobalVars.tUsers.TryAdd(state.receivedstr, tempSck) == false) ;
                    }

                }
                if ((alreadyAuthenticated == false) || (alreadyAuthenticated == true && retry == true))
                {
                    state.client = tempSck;
                    tempSck.Send(System.Text.Encoding.ASCII.GetBytes("PASS " + state.receivedstr.SubString(state.receivedstr.nthDexOf(",", 1) + 1, state.receivedstr.nthDexOf(",", 2)) + "\nNICK " + state.receivedstr.SubString(state.receivedstr.nthDexOf(",", 0) + 1, state.receivedstr.nthDexOf(",", 1)) + "\nJOIN #" + state.receivedstr.SubString(state.receivedstr.nthDexOf(",", 0) + 1, state.receivedstr.nthDexOf(",", 1)).ToLower() + "\n"));
                    tempSck.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, new AsyncCallback(tRead), state);
                }

            }
        }

        public static void tRead(IAsyncResult result)
        {
            GlobalVars.tState state = (GlobalVars.tState)result.AsyncState;
            int readlength = 0;
            try
            {
                readlength = state.client.EndReceive(result);
            }
            catch
            {
                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.TWITCH_DISCONNECTED),3);
                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.TWITCH_RECONNECTING_ONE), 2);
                GlobalCalls.WriteToSocket(state.originalClient, Encoding.ASCII.GetBytes(Signals.TWITCH_DISCONNECTED.ToString()));
                GlobalCalls.WriteToSocket(state.originalClient, Encoding.ASCII.GetBytes(Signals.TWITCH_RECONNECTING_ONE.ToString()));
                System.Threading.Thread.Sleep(1000);
                tConn(state, true);
            }
            if (readlength > 0)
            {
                string[] splitstr = Regex.Split(Encoding.ASCII.GetString(state.buffer, 0, readlength), "\r\n");
                for (int i = 0; i <= splitstr.Count() - 2; i++)
                {
                    if (splitstr[i].Length < 4)
                        continue;
                    sString line = splitstr[i];
                    if (line.SubString(0, 4) == "PING")
                    {
                        GlobalCalls.WriteToSocket(state.client, Encoding.ASCII.GetBytes(line.ToString().Replace("PING", "PONG") + "\r\n"));
                        GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.TWITCH_PONG));
                    }
                    else
                    {
                        string command = line.SubString(line.nthDexOf(" ", 0) + 1, line.nthDexOf(" ", 1));
                        string msg = "";
                        switch (command)
                        {
                            case "NOTICE":
                                msg = line.SubString(line.nthDexOf(":", 1) + 1);
                                if (msg == "Login unsuccessful")
                                {
                                    state.client.Close();
                                    GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.TWITCH_AUTH_FAIL),3);
                                    GlobalCalls.WriteToSocket(state.originalClient, Encoding.ASCII.GetBytes(Signals.TWITCH_AUTH_FAIL.ToString()));
                                    return;
                                }
                                break;
                            case "001":
                                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.TWITCH_AUTH_SUCCESS),1);
                                GlobalCalls.WriteToSocket(state.originalClient, Encoding.ASCII.GetBytes(Signals.TWITCH_AUTH_SUCCESS.ToString()));
                                break;
                            case "PRIVMSG":
                                string user = line.SubString(1, line.nthDexOf("!", 0));
                                string channel = line.SubString(line.nthDexOf("#", 0) + 1, line.ToString().IndexOf(" ", line.nthDexOf("#", 0) + 1));
                                msg = line.SubString(line.nthDexOf(":", 1) + 1);
                                if (user.ToLower() != channel.ToLower())
                                {
                                    foreach (sString k in GlobalVars.oUsers.Keys.ToArray())
                                    {
                                        if ((GlobalVars.oUsers[k] == true) && (k.SubString(k.nthDexOf(",", 0) + 1, k.nthDexOf(",", 1)).ToString().ToLower() == channel.ToLower()))
                                        {
                                            string processedMsg = "";
                                            string[] splitmsg = Regex.Split(msg, " ");
                                            foreach (string s in splitmsg.ToArray())
                                            {
                                                var regstr = Regex.Match(s, "http://osu.ppy.sh/b/([0-9]{1,})+");
                                                if (regstr.Success)
                                                {
                                                    BeatmapInfo[] btmp = GlobalCalls.ParseOsuData<BeatmapInfo[]>("http://osu.ppy.sh/api/get_beatmaps?k=" + GlobalVars.apiKey + "&b=" + regstr.Groups[1].Value);
                                                    if (btmp.Length != 0)
                                                    {
                                                        processedMsg += "(" + btmp[0].artist + " - " + btmp[0].title + ")[" + s + "] ";
                                                    }
                                                    else { processedMsg += s + " "; }
                                                    continue;
                                                }
                                                regstr = Regex.Match(s, "http://osu.ppy.sh/s/([0-9]{1,})+");
                                                if (regstr.Success)
                                                {
                                                    BeatmapInfo[] btmp = GlobalCalls.ParseOsuData<BeatmapInfo[]>("http://osu.ppy.sh/api/get_beatmaps?k=" + GlobalVars.apiKey + "&s=" + regstr.Groups[1].Value);
                                                    if (btmp.Length != 0)
                                                    {
                                                        processedMsg += "(" + btmp[0].artist + " - " + btmp[0].title + ")[" + s + "] ";
                                                    }
                                                    else { processedMsg += s + " "; }
                                                    continue;
                                                }
                                                processedMsg += s + " ";
                                            }
                                            msg = "PRIVMSG " + k.SubString(0, k.nthDexOf(",", 0)) + " :" + user + ": " + processedMsg.Substring(0, processedMsg.LastIndexOf(" ")) + "\n";
                                            GlobalCalls.AddToQueue(msg);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
                state.client.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, new AsyncCallback(tRead), state);
            }
        }
    }
}

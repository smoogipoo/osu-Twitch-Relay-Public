using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Collections.Generic;
using smgiFuncs;

namespace osu_Twitch_Relay_Server
{
    class tServ
    {
        static bool trashb;
        static Socket trashs;

        public static void tConn(GlobalVars.tState state, bool retry = false, bool prevAuthed = false)
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
            if (tempSck.Connected)
            {
                bool alreadyAuthenticated = false;
                state.receivedstr = state.receivedstr.ToString().Replace(" ", "_");
                foreach (var user in GlobalVars.oUsers.Where(user => user.Key.SubString(0, user.Key.nthDexOf(",", 0)) == state.receivedstr.SubString(0, state.receivedstr.nthDexOf(",", 0)) || user.Key.SubString(user.Key.nthDexOf(",", 0) + 1, user.Key.nthDexOf(",", 1)) == state.receivedstr.SubString(state.receivedstr.nthDexOf(",", 0) + 1, state.receivedstr.nthDexOf(",", 1))))
                {
                    if (user.Value)
                    {
                        alreadyAuthenticated = true;
                        if (retry)
                        {
                            if (GlobalVars.tUsers[user.Key].Connected)
                                GlobalVars.tUsers[user.Key].Shutdown(SocketShutdown.Both);
                            while (GlobalVars.tUsers.TryRemove(user.Key, out trashs) == false) ;
                            while (GlobalVars.tUsers.TryAdd(state.receivedstr, tempSck) == false) ;
                            GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.TWITCH_RECONNECTED),1);
                            GlobalCalls.WriteToSocket(state.originalClient, Encoding.ASCII.GetBytes(Signals.TWITCH_RECONNECTED.ToString()));
                        }
                        else
                        {
                            GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.USER_ALREADY_AUTHENTICATED),1);
                            GlobalCalls.WriteToSocket(state.originalClient, Encoding.ASCII.GetBytes(Signals.USER_ALREADY_AUTHENTICATED.ToString()));
                            return;
                        }
                        break;
                    }
                    while (GlobalVars.oUsers.TryRemove(user.Key, out trashb) == false) ;
                    if (GlobalVars.tUsers[user.Key].Connected)
                        GlobalVars.tUsers[user.Key].Shutdown(SocketShutdown.Both);
                    while (GlobalVars.tUsers.TryRemove(user.Key, out trashs) == false) ;
                }
                if (!alreadyAuthenticated && !retry)
                {
                    while (GlobalVars.oUsers.TryAdd(state.receivedstr, prevAuthed) == false) ;
                    while (GlobalVars.tUsers.TryAdd(state.receivedstr, tempSck) == false) ;
                }
                state.client = tempSck;
                tempSck.Send(Encoding.ASCII.GetBytes("PASS " + state.receivedstr.SubString(state.receivedstr.nthDexOf(",", 1) + 1, state.receivedstr.nthDexOf(",", 2)) + "\r\nNICK " + state.receivedstr.SubString(state.receivedstr.nthDexOf(",", 0) + 1, state.receivedstr.nthDexOf(",", 1)) + "\r\nJOIN #" + state.receivedstr.SubString(state.receivedstr.nthDexOf(",", 0) + 1, state.receivedstr.nthDexOf(",", 1)).ToLower() + "\r\n"));
                tempSck.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, tRead, state);
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
                        if (state.tFirstPing)
                        {
                            state.tFirstPing = false;
                            System.Threading.Thread pingThread = new System.Threading.Thread(tPing);
                            pingThread.IsBackground = true;
                            pingThread.Start(state);
                        }
                        GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.TWITCH_PONGS));
                    }
                    else if (line.SubString(0, 4) == "PONG")
                    {
                        GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.TWITCH_PONGR));
                        state.tPongTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                    }
                    else
                    {
                        string command = line.SubString(line.nthDexOf(" ", 0) + 1, line.nthDexOf(" ", 1));
                        string msg;
                        switch (command)
                        {
                            case "NOTICE":
                                msg = line.SubString(line.nthDexOf(":", 1) + 1);
                                if (msg == "Login unsuccessful")
                                {
                                    state.client.Close();
                                    GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.TWITCH_AUTH_FAIL), 3);
                                    GlobalCalls.WriteToSocket(state.originalClient, Encoding.ASCII.GetBytes(Signals.TWITCH_AUTH_FAIL.ToString()));
                                    foreach (string k in GlobalVars.settings.GetKeys().Where(k => k.Contains("AuthUser")))
                                    {
                                        sString k1 = GlobalVars.settings.GetSetting(k);
                                        k1 = k1.SubString(0, k1.LastIndexOf(",") + 1);
                                        if (Equals(k1, state.receivedstr))
                                        {
                                            GlobalVars.settings.DeleteSetting(k);
                                            //Reset setting count
                                            List<string> newSettings = GlobalVars.settings.GetKeys().Where(setting => setting.Contains("AuthUser")).Select(setting => GlobalVars.settings.GetSetting(setting)).ToList();
                                            foreach (string setting in GlobalVars.settings.GetKeys())
                                            {
                                                GlobalVars.settings.DeleteSetting(setting);
                                            }
                                            for (int settingCounter = 0; settingCounter < newSettings.Count; settingCounter++)
                                            {
                                                GlobalVars.settings.AddSetting("AuthUser" + settingCounter, newSettings[settingCounter]);
                                            }
                                            GlobalVars.settings.Save();
                                            while (GlobalVars.oUsers.TryRemove(k1, out trashb) == false) ;
                                            while (GlobalVars.tUsers.TryRemove(k1, out trashs) == false) ;
                                        }
                                    }
                                    return;
                                }
                                break;
                            case "001":
                                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.TWITCH_AUTH_SUCCESS), 1);
                                GlobalCalls.WriteToSocket(state.originalClient, Encoding.ASCII.GetBytes(Signals.TWITCH_AUTH_SUCCESS.ToString()));
                                break;
                            case "PRIVMSG":
                                string user = line.SubString(1, line.nthDexOf("!", 0));
                                string channel = line.SubString(line.nthDexOf("#", 0) + 1, line.ToString().IndexOf(" ", line.nthDexOf("#", 0) + 1, StringComparison.Ordinal));
                                msg = line.SubString(line.nthDexOf(":", 1) + 1);
                                if (!String.Equals(user, channel, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    foreach (var k in GlobalVars.oUsers.Where(k => k.Value && String.Equals(k.Key.SubString(k.Key.nthDexOf(",", 0) + 1, k.Key.nthDexOf(",", 1)), channel, StringComparison.CurrentCultureIgnoreCase)))
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
                                        msg = "PRIVMSG " + k.Key.SubString(0, k.Key.nthDexOf(",", 0)) + " :" + user + ": " + processedMsg.Substring(0, processedMsg.LastIndexOf(" ", StringComparison.Ordinal)) + "\n";
                                        GlobalCalls.AddToQueue(msg);
                                    }
                                }
                                break;
                            default:
                                if (GlobalVars.logAll)
                                    GlobalCalls.WriteToConsole(line.ToString());
                                break;
                        }
                    }
                }
                state.client.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, tRead, state);
            }
        }

        public static void tPing(object state)
        {
            while (true)
            {
                System.Threading.Thread.Sleep(40000);
                if (GlobalCalls.WriteToSocket(((GlobalVars.tState)state).client, Encoding.ASCII.GetBytes("PING\r\n")) == false)
                    return;
                System.Threading.Thread.Sleep(10000);
                if ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds - ((GlobalVars.tState)state).tPongTime > 20)
                {
                    ((GlobalVars.tState)state).client.Shutdown(SocketShutdown.Both);
                    GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.TWITCH_DISCONNECTED), 3);
                    GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.TWITCH_RECONNECTING_ONE), 2);
                    System.Threading.Thread.Sleep(1000);
                    break;
                }
            }
            tConn((GlobalVars.tState)state, true, true);
        }
    }
}

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using smgiFuncs;

namespace osu_Twitch_Relay_Server
{
    class oServ
    {
        public static void Create()
        {
            new oServ();
        }
        public oServ()
        {
            oConn();
        }
        public void oConn(bool retry = false)
        {
            try
            {
                GlobalVars.oFirstPing = true;
                GlobalVars.oSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                GlobalVars.oSock.Connect("irc.ppy.sh", 6667);
                GlobalCalls.WriteToConsole(retry? Enum.GetName(typeof (Signals), Signals.OSU_RECONNECTED) : Enum.GetName(typeof (Signals), Signals.OSU_CONNECT_SUCCESS), 1);
                byte[] buffer = new byte[65536];
                GlobalCalls.WriteToSocket(GlobalVars.oSock, Encoding.ASCII.GetBytes("PASS " + GlobalVars.osuIRC_Password + "\nNICK " + GlobalVars.osuIRC_Username + "\n"));
                GlobalVars.oSock.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, oRead, buffer);
            }
            catch
            {
                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals),Signals.OSU_CONNECT_FAIL),3);
                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.OSU_RECONNECTING_ONE),2);
                System.Threading.Thread.Sleep(1000);
                oConn(true);
            }
        }

        public void oRead(IAsyncResult result)
        {
            byte[] buffer = (byte[])result.AsyncState;
            int readlength = 0;
            try
            {
                readlength = GlobalVars.oSock.EndReceive(result);
            }
            catch
            {
                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.OSU_DISCONNECTED),3);
                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.OSU_RECONNECTING_ONE),2);
                System.Threading.Thread.Sleep(1000);
                oConn(true);
            }
            if (readlength > 0)
            {
                string[] splitstr = Regex.Split(Encoding.ASCII.GetString(buffer, 0, readlength), "\n");
                for (int i = 0; i <= splitstr.Count() - 2; i++)
                {
                    if (splitstr[i].Length < 4)
                        continue;
                    sString line = splitstr[i];
                    if (line.SubString(0, 4) == "PING")
                    {
                        GlobalCalls.WriteToSocket(GlobalVars.oSock, Encoding.ASCII.GetBytes(line.ToString().Replace("PING", "PONG") + "\n"));
                        if (GlobalVars.oFirstPing)
                        {
                            GlobalVars.oFirstPing = false;
                            System.Threading.Thread pingThread = new System.Threading.Thread(oPing);
                            pingThread.IsBackground = true;
                            pingThread.Start();
                        }

                        GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.OSU_PONGS));
                    }
                    else
                    {
                        string command = line.SubString(line.nthDexOf(" ", 0) + 1, line.nthDexOf(" ", 1));
                        switch (command)
                        {
                            case "464":
                                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.OSU_AUTH_FAIL),3);
                                GlobalVars.oSock.Close();
                                oConn(true);
                                break;
                            case "001":
                                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.OSU_AUTH_SUCCESS),1);
                                break;
                            case "PONG":
                                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.OSU_PONGR));
                                GlobalVars.oPongTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                                break;
                            case "PRIVMSG":
                                string user = line.SubString(1, line.nthDexOf("!", 0));
                                string msg = line.SubString(line.nthDexOf(":", 1) + 1);
                                foreach (sString k in GlobalVars.oUsers.Keys.ToArray().Where(k => String.Equals(k.SubString(0, k.nthDexOf(",", 0)).ToString(CultureInfo.InvariantCulture), user, StringComparison.CurrentCultureIgnoreCase)))
                                {
                                    if (msg == "!auth")
                                    {
                                        GlobalVars.oUsers[k] = !GlobalVars.oUsers[k];
                                        if (GlobalVars.oUsers[k])
                                        {
                                            bool modified = false;
                                            foreach (string s in GlobalVars.settings.GetKeys().Where(s => String.Equals(GlobalVars.settings.GetSetting(s).Substring(0, GlobalVars.settings.GetSetting(s).LastIndexOf(",", StringComparison.InvariantCulture) + 1), k.ToString())))
                                            {
                                                GlobalVars.settings.AddSetting(s, k + "1");
                                                modified = true;
                                            }
                                            if (!modified)
                                                GlobalVars.settings.AddSetting("AuthUser" + GlobalVars.settings.GetKeys().Count, k + "1");
                                            GlobalVars.settings.Save();
                                            GlobalCalls.WriteToSocket(GlobalVars.oSock,Encoding.ASCII.GetBytes("PRIVMSG " + user + " :Authorized for in-game messaging.\n"));
                                            GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.PLAYER_AUTHED), 1);
                                            GlobalCalls.WriteToConsole(user);
                                        }
                                        else
                                        {
                                            bool modified = false;
                                            foreach (string s in GlobalVars.settings.GetKeys().Where(s => String.Equals(GlobalVars.settings.GetSetting(s).Substring(0, GlobalVars.settings.GetSetting(s).LastIndexOf(",", StringComparison.InvariantCulture) + 1), k.ToString())))
                                            {
                                                GlobalVars.settings.AddSetting(s, k + "0");
                                                modified = true;
                                            }
                                            if (!modified)
                                                GlobalVars.settings.AddSetting("AuthUser" + GlobalVars.settings.GetKeys().Count, k + "0");
                                            GlobalVars.settings.Save();
                                            GlobalCalls.WriteToSocket(GlobalVars.oSock, Encoding.ASCII.GetBytes("PRIVMSG " + user + " :Deauthorized from in-game messaging\n"));
                                            GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.PLAYER_DEAUTHED), 1);
                                            GlobalCalls.WriteToConsole(user);
                                        }
                                    }
                                    else
                                    {
                                        if (GlobalVars.oUsers[k])
                                        {
                                            switch (msg)
                                            {
                                                case "!viewers":
                                                    TwitchInfo tSerialized = GlobalCalls.ParseTwitchData(k.SubString(k.nthDexOf(",", 0) + 1, k.nthDexOf(",", 1)).ToLower());
                                                    GlobalCalls.AddToQueue("PRIVMSG " + user + " :" + tSerialized.viewers_count + " Viewers.\n");
                                                    break;
                                                default:
                                                    GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.OTT_MESSAGE_SENT));
                                                    GlobalCalls.WriteToSocket(GlobalVars.tUsers[k], Encoding.ASCII.GetBytes("PRIVMSG #" + k.SubString(k.nthDexOf(",", 0) + 1, k.nthDexOf(",", 1)).ToLower() + " :" + msg + "\n"));
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                                }
                                break;
                            default:
                                if (GlobalVars.logAll)
                                    GlobalCalls.WriteToConsole(line.ToString());
                                break;
                            }
                        }
                    }
                buffer = new byte[65536];
                GlobalVars.oSock.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, oRead, buffer);
            }
        }

        public void oPing()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(40000);
                if (GlobalCalls.WriteToSocket(GlobalVars.oSock, Encoding.ASCII.GetBytes("PING\n")) == false)
                    return;
                System.Threading.Thread.Sleep(10000);
                if ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds - GlobalVars.oPongTime > 20)
                {
                    GlobalVars.oSock.Shutdown(SocketShutdown.Both);
                    GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.OSU_DISCONNECTED), 3);
                    GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.OSU_RECONNECTING_ONE), 2);
                    System.Threading.Thread.Sleep(1000);
                    break;
                }
            }
            oConn(true);
        }
    }
}

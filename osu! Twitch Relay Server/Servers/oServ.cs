using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
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
                GlobalVars.oSock.Connect("irc.ppy.sh", 6667);
                if (retry == true)
                {
                    GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.OSU_RECONNECTED),1);
                }
                else
                {
                    GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.OSU_CONNECT_SUCCESS),1);
                }
                byte[] buffer = new byte[65536];
                GlobalCalls.WriteToSocket(GlobalVars.oSock, Encoding.ASCII.GetBytes("PASS " + GlobalVars.osuIRC_Password + "\nNICK " + GlobalVars.osuIRC_Username + "\n"));
                GlobalVars.oSock.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(oRead), buffer);
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
                        GlobalCalls.WriteToSocket(GlobalVars.oSock, Encoding.ASCII.GetBytes("PONG\n"));
                        GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.OSU_PONG));
                    }
                    else
                    {
                        string command = line.SubString(line.nthDexOf(" ", 0) + 1, line.nthDexOf(" ", 1));
                        string msg = "";
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
                            case "PRIVMSG":
                                string user = line.SubString(1, line.nthDexOf("!", 0));
                                msg = line.SubString(line.nthDexOf(":", 1) + 1);
                                foreach (sString k in GlobalVars.oUsers.Keys.ToArray())
                                {
                                    if (k.SubString(0, k.nthDexOf(",", 0)).ToString().ToLower() == user.ToLower())
                                    {
                                        if (msg == "!auth")
                                        {
                                            GlobalVars.oUsers[k] = !GlobalVars.oUsers[k];
                                            if (GlobalVars.oUsers[k] == true)
                                            {
                                                GlobalVars.settings.AddSetting("AuthUser" + GlobalVars.settings.GetKeys().Count, k.ToString(), true);
                                                GlobalVars.settings.Save();
                                                GlobalCalls.WriteToSocket(GlobalVars.oSock,Encoding.ASCII.GetBytes("PRIVMSG " + user + " :Authorized for in-game messaging.\n"));
                                                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.PLAYER_AUTHED), 1);
                                                GlobalCalls.WriteToConsole(user);
                                            }
                                            else
                                            {
                                                foreach (string setting_key in GlobalVars.settings.GetKeys())
                                                {
                                                    if (GlobalVars.settings.GetSetting(setting_key) == k.ToString())
                                                    {
                                                        GlobalVars.settings.DeleteSetting(setting_key);
                                                        break;
                                                    }
                                                }
                                                GlobalVars.settings.Save();
                                                GlobalCalls.WriteToSocket(GlobalVars.oSock, Encoding.ASCII.GetBytes("PRIVMSG " + user + " :Deauthorized from in-game messaging\n"));
                                                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.PLAYER_DEAUTHED), 1);
                                                GlobalCalls.WriteToConsole(user);
                                            }
                                        }
                                        else
                                        {
                                            if (GlobalVars.oUsers[k] == true)
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
                                }
                                break;
                            }
                        }
                    }
                buffer = new byte[65536];
                GlobalVars.oSock.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(oRead), buffer);
            }
        }
    }
}

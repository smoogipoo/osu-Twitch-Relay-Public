using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net.Mail;
using smgiFuncs;

namespace osu_Twitch_Relay_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //If an error occurs in the server, smoogi will be notified and server will be
            //automatically restarted
            AppDomain.CurrentDomain.UnhandledException += Program_UnhandledExceptionTrap;

            GlobalVars.settings = new Settings();
            System.Threading.Thread.Sleep(1000);

            //Client listener start thread
            System.Threading.Thread cs = new System.Threading.Thread(cServ.Create);
            cs.IsBackground = true;
            cs.Start();

            //osu! IRC listener start thread
            System.Threading.Thread os = new System.Threading.Thread(oServ.Create);
            os.IsBackground = true;
            os.Start();

            //Re-authenticate previous authenticated users (crash previously occurred)
            foreach (string k in GlobalVars.settings.GetKeys())
            {
                if (k.Substring(0, 8) == "AuthUser")
                {
                    GlobalVars.tState twitchState = new GlobalVars.tState();
                    twitchState.receivedstr = GlobalVars.settings.GetSetting(k);
                    tServ.tConn(twitchState, false, true);
                }
            }

            while (true) {
                string s = Console.ReadLine();
                if (s == "log-all")
                {
                    if (GlobalVars.logAll == true)
                        GlobalVars.logAll = false;
                    else
                        GlobalVars.logAll = true;
                }
            }
        }
        static void Program_UnhandledExceptionTrap(object sender, UnhandledExceptionEventArgs e)
        {
            //Send email
            try
            {
                Exception exception = (Exception)e.ExceptionObject;
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential(GlobalVars.email_Email, GlobalVars.email_Pass);
                client.Send(GlobalVars.email_Email, "...@gmail.com", "Crash on " + System.DateTime.Now.ToString(), e.ExceptionObject.ToString());
            }
            catch { }

            //Restart
            System.Diagnostics.Process.Start(Environment.CurrentDirectory + "\\osu! Twitch Relay Server.exe");
            Environment.Exit(0);
        }
    }
}

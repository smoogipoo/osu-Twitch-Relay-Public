using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Net.Mail;
using smgiFuncs;

namespace osu_Twitch_Relay_Server
{
    class Program
    {
        static void Main()
        {
            //If an error occurs in the server, smoogi will be notified and server will be
            //automatically restarted
            AppDomain.CurrentDomain.UnhandledException += Program_UnhandledExceptionTrap;

            GlobalVars.settings = new Settings();
            Thread.Sleep(1000);

            //Client listener start thread
            Thread cs = new Thread(cServ.Create);
            cs.IsBackground = true;
            cs.Start();

            //osu! IRC listener start thread
            Thread os = new Thread(oServ.Create);
            os.IsBackground = true;
            os.Start();

            //Re-authenticate previous authenticated users (crash previously occurred)
            foreach (string k in GlobalVars.settings.GetKeys().Where(k => k.Substring(0, 8) == "AuthUser"))
            {
                GlobalVars.tState twitchState = new GlobalVars.tState();
                twitchState.receivedstr = GlobalVars.settings.GetSetting(k);
                tServ.tConn(twitchState, false, true);
            }

            while (true) {
                string s = Console.ReadLine();
                if (s == "log-all")
                {
                    GlobalVars.logAll = !GlobalVars.logAll;
                }
            }
        }
        static void Program_UnhandledExceptionTrap(object sender, UnhandledExceptionEventArgs e)
        {
            //Send email
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(GlobalVars.email_Email, GlobalVars.email_Pass);
                client.Send(GlobalVars.email_Email, "@gmail.com", "Crash on " + DateTime.Now, e.ExceptionObject.ToString());
            }
            catch { }

            //Restart
            System.Diagnostics.Process.Start(Environment.CurrentDirectory + "\\osu! Twitch Relay Server.exe");
            Environment.Exit(0);
        }
    }
}

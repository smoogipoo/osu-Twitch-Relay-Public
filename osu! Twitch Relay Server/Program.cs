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
using smgiFuncs;

namespace osu_Twitch_Relay_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //Client listener start thread
            System.Threading.Thread cs = new System.Threading.Thread(cServ.Create);
            cs.IsBackground = true;
            cs.Start();

            //osu! IRC listener start thread
            System.Threading.Thread os = new System.Threading.Thread(oServ.Create);
            os.IsBackground = true;
            os.Start();

            while (true) { Console.ReadKey(); }
        }
    }
}

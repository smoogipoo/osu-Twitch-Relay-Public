using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using smgiFuncs;

namespace osu_Twitch_Relay_Server
{
    class cServ
    {
        static readonly AutoResetEvent clientWaitHandle = new AutoResetEvent(false);
        public static void Create()
        {
            new cServ();
        }
        public cServ()
        {
            TcpListener clientListener = new TcpListener(IPAddress.Any, 6192);
            clientListener.Start();
            GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.LISTENER_STARTED),1);
            while (true)
            {
                clientListener.BeginAcceptSocket(clientAccCB, clientListener);
                clientWaitHandle.WaitOne();
                clientWaitHandle.Reset();
            }
        }
        public void clientAccCB(IAsyncResult result)
        {
            GlobalVars.ClientState state = new GlobalVars.ClientState();
            state.client = ((TcpListener)result.AsyncState).EndAcceptSocket(result);
            clientWaitHandle.Set();
            GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.CLIENT_CONNECT_SUCCESS),1);
            state.client.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, clientReadCB, state);
        }
        public void clientReadCB(IAsyncResult result)
        {
            GlobalVars.ClientState state = (GlobalVars.ClientState)result.AsyncState;
            int readlength;
            try
            {
                readlength = state.client.EndReceive(result);
            }
            catch
            {
                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.CLIENT_DISCONNECTED),3);
                return;
            }
            if (readlength > 0)
            {
                sString receivedstr = Encoding.ASCII.GetString(state.buffer, 0, readlength); //Contains: oName,tName,TwitchToken,privKey,
                GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.CLIENT_RECEIVED_CONNECTION),1);
                GlobalCalls.WriteToConsole("o:" + receivedstr.SubString(0, receivedstr.nthDexOf(",", 0)) + "\tt:" + receivedstr.SubString(receivedstr.nthDexOf(",", 0) + 1, receivedstr.nthDexOf(",", 1)));
                if (receivedstr.CountOf(",") == 4)
                {
                    if (receivedstr.SubString(receivedstr.nthDexOf(",", 2) + 1, receivedstr.nthDexOf(",", 3)) == GlobalVars.privKey)
                    {
                        GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.CLIENT_PRIVKEY_SUCCESS),1);
                        GlobalVars.tState twitchState = new GlobalVars.tState();
                        twitchState.originalClient = state.client;
                        twitchState.receivedstr = receivedstr;
                        tServ.tConn(twitchState);
                    }
                    else
                    {
                        GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.CLIENT_PRIVKEY_FAIL),3);
                        state.client.Close();
                    }
                }
                else
                {
                    GlobalCalls.WriteToConsole(Enum.GetName(typeof(Signals), Signals.CLIENT_MALFORMED_DATA),3);
                    state.client.Close();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_Twitch_Relay_Server
{
    enum Signals
    {
        //Client signals
        CLIENT_RECEIVED_CONNECTION = 0,
        CLIENT_CONNECT_FAIL = 1,
        CLIENT_CONNECT_SUCCESS = 2,
        CLIENT_DISCONNECTED = 3,
        CLIENT_MALFORMED_DATA = 4,
        CLIENT_PRIVKEY_SUCCESS = 5,
        CLIENT_PRIVKEY_FAIL = 6,

        USER_ALREADY_AUTHENTICATED = 7,

        //Twitch signals
        TWITCH_CONNECT_SUCCESS = 8,
        TWITCH_CONNECT_FAIL = 9,
        TWITCH_DISCONNECTED = 10,
        TWITCH_RECONNECTED = 11,
        TWITCH_RECONNECTING_ONE = 12,
        TWITCH_AUTH_SUCCESS = 13,
        TWITCH_AUTH_FAIL = 14,
        TWITCH_PONGS = 15, //PONG sent
        TWITCH_PONGR = 16, //PONG received

        //osu! signals
        OSU_CONNECT_FAIL = 17,
        OSU_CONNECT_SUCCESS = 18,
        OSU_DISCONNECTED = 19,
        OSU_RECONNECTED = 20,
        OSU_RECONNECTING_ONE = 21,
        OSU_AUTH_SUCCESS = 22,
        OSU_AUTH_FAIL = 23,
        OSU_PONGS = 24, //PONG sent
        OSU_PONGR = 25, //PONG received

        //Message signals
        OTT_MESSAGE_SENT = 26, //osu! to twitch
        TTO_MESSAGE_QUEUED = 27, //Twitch to osu!
        TTO_MESSAGE_SENT = 28, //Twitch to osu!

        //Client signals
        LISTENER_STARTED = 29,

        //Player signals
        PLAYER_AUTHED = 30,
        PLAYER_DEAUTHED = 31,


    }
}

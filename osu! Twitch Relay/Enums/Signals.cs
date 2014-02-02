using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_Twitch_Relay
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
        TWITCH_PONG = 15,

        //osu! signals
        OSU_CONNECT_FAIL = 16,
        OSU_CONNECT_SUCCESS = 17,
        OSU_DISCONNECTED = 18,
        OSU_RECONNECTED = 19,
        OSU_RECONNECTING_ONE = 20,
        OSU_AUTH_SUCCESS = 21,
        OSU_AUTH_FAIL = 22,
        OSU_PONG = 23,

        //Message signals
        OTT_MESSAGE_SENT = 24, //osu! to twitch
        TTO_MESSAGE_QUEUED = 25, //Twitch to osu!
        TTO_MESSAGE_SENT = 26, //Twitch to osu!

        //Client signals
        LISTENER_STARTED = 27,

        //Player signals
        PLAYER_AUTHED = 28,
        PLAYER_DEAUTHER = 29,
    }
}

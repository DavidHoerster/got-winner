
using System;
using StackExchange.Redis;

namespace got_winner_voting.Globals
{
    public static class GlobalItems
    {
        public static Lazy<ConnectionMultiplexer> RedisConnection { get; set; }
    }
}
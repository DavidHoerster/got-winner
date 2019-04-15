
using System;
using System.Data;
using StackExchange.Redis;

namespace got_winner_voting.Globals
{
    public static class GlobalItems
    {
        public static Lazy<ConnectionMultiplexer> RedisConnection { get; set; }
        public static string SqlConnectionStr { get; set; }
    }
}
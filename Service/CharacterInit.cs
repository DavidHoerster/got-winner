

using System.Collections.Generic;
using System.Data.SqlClient;
using got_winner_voting.Model;
using Dapper;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System;
using StackExchange.Redis;
using Microsoft.ApplicationInsights;

namespace got_winner_voting.Service
{
    public class CharacterInit : BackgroundService
    {

        private readonly IConfiguration _config;
        private Lazy<ConnectionMultiplexer> _cache;
        public CharacterInit(IConfiguration config, Lazy<ConnectionMultiplexer> cache)
        {
            _config = config;
            _cache = cache;
        }
        // public async Task<IEnumerable<Character>> GetCharactersAsync(IConfiguration config)
        // {
        //     IEnumerable<Character> chars = null;
        //     using (var conn = new SqlConnection(_config["AzureSQL:ConnectionString"]))
        //     {
        //         chars = await conn.QueryAsync<Character>("SELECT Id, FullName FROM GoTCharacters ORDER BY Id");
        //     }
        //     return chars;
        // }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = new TelemetryClient();

            var date = DateTimeOffset.UtcNow;
            var cacheDb = _cache.Value.GetDatabase();
            //var result = await cacheDb.KeyDeleteAsync("got");

            IEnumerable<Character> chars = null;
            using (var conn = new SqlConnection(_config["Azure:SQL:ConnectionString"]))
            {
                chars = await conn.QueryAsync<Character>("SELECT Id, FullName FROM GoTCharacters ORDER BY Id");
            }

            // foreach (var character in chars)
            // {
            //     cacheDb.HashSet("got", character.Id, 0);
            // }

            client.TrackDependency("Redis Cache", "Reset All Characters for Startup", "startup", date, new TimeSpan(DateTimeOffset.UtcNow.Ticks - date.Ticks), true);
        }
    }
}
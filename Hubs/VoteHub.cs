using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using got_winner_voting.Model;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace got_winner_voting.Hubs
{
    public class VoteHub : Hub<IVoteHub>
    {
        public async Task RecordVote(string character)
        {
            IDatabase cache = Globals.GlobalItems.RedisConnection.Value.GetDatabase();

            if (character.Equals("RESET", StringComparison.OrdinalIgnoreCase))
            {
                var chars = await cache.HashGetAllAsync("got");
                var updChars = chars.Select(c => new HashEntry(c.Name, 0)).ToArray();
                await cache.HashSetAsync("got", updChars);
            }
            else
            {
                var newValue = await cache.HashIncrementAsync("got", character);
            }
            var votes = await GetVotesAsync(cache);
            await Clients.All.BroadcastVotes(votes);
        }

        private async Task<IEnumerable<Character>> GetVotesAsync(IDatabase db)
        {
            var chars = await db.HashGetAllAsync("got");
            var charList = chars.Select(c =>
            {
                if (!c.Value.TryParse(out long votes))
                {
                    votes = 0;
                }
                return new Character { Name = c.Name, Votes = votes };
            });
            return charList;
        }
    }
}
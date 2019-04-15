
using System.Collections.Generic;
using System.Threading.Tasks;
using got_winner_voting.Model;

namespace got_winner_voting.Hubs
{
    public interface IVoteHub
    {
        Task RecordVote(string character);
        Task BroadcastVotes(IEnumerable<Character> chars);
    }
}
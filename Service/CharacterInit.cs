

using System.Collections.Generic;
using System.Data.SqlClient;
using got_winner_voting.Model;
using Dapper;
using System.Threading.Tasks;

namespace got_winner_voting.Service
{
    public static class CharacterInit
    {

        public async static Task<IEnumerable<Character>> GetCharactersAsync()
        {
            IEnumerable<Character> chars = null;
            using (var conn = new SqlConnection(Globals.GlobalItems.SqlConnectionStr))
            {
                chars = await conn.QueryAsync<Character>("SELECT Id, FullName FROM GoTCharacters ORDER BY Id");
            }
            return chars;
        }
    }
}
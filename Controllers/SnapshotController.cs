using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using got_winner_voting.Model;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Dapper;

namespace got_winner_voting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SnapshotController : ControllerBase
    {
        // POST api/values
        [HttpPost("{name}")]
        public async Task<ActionResult> Post(string name)
        {
            var client = new TelemetryClient();
            var date = DateTimeOffset.UtcNow;

            var db = Globals.GlobalItems.RedisConnection.Value.GetDatabase();
            var chars = await db.HashGetAllAsync("got");


            client.TrackDependency("Redis Cache", "Get All Characters", "data", date, new TimeSpan(DateTimeOffset.UtcNow.Ticks - date.Ticks), true);

            var charList = chars.Select(c =>
            {
                if (!c.Value.TryParse(out long votes))
                {
                    votes = 0;
                }
                return new Snapshot { SnapName = name, Name = c.Name, Votes = votes };
            });

            using (var conn = new SqlConnection(Globals.GlobalItems.SqlConnectionStr))
            {
                var count = conn.Execute(@"INSERT INTO GoTSnapshot2(SnapName, Name, Votes) VALUES (@SnapName, @Name, @Votes)",
                    charList.ToArray());
            }

            return Redirect("~/");
        }
    }
}
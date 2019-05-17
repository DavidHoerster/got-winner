using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using got_winner_voting.Model;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace got_winner_voting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SnapshotController : ControllerBase
    {
        private Lazy<ConnectionMultiplexer> _cache;
        private readonly IConfiguration _config;

        public SnapshotController(IConfiguration config, Lazy<ConnectionMultiplexer> cache)
        {
            _config = config;
            _cache = cache;
        }
        // POST api/values
        [HttpPost("{name}")]
        public async Task<ActionResult> Post(string name)
        {
            var client = new TelemetryClient();
            var date = DateTimeOffset.UtcNow;

            var db = _cache.Value.GetDatabase();
            var chars = await db.HashGetAllAsync("got");

            var charList = chars.Select(c =>
            {
                if (!c.Value.TryParse(out long votes))
                {
                    votes = 0;
                }
                return new Snapshot { SnapName = name, Name = c.Name, Votes = votes };
            });

            var data = String.Join(";", charList.Select(c => $"{c.Name} = {c.Votes} votes").ToArray());

            client.TrackDependency("Redis Cache", "Get All Characters", data, date, new TimeSpan(DateTimeOffset.UtcNow.Ticks - date.Ticks), true);


            using (var conn = new SqlConnection(_config["Azure:SQL:ConnectionString"]))
            {
                var count = conn.Execute(@"INSERT INTO GoTSnapshot2(SnapName, Name, Votes) VALUES (@SnapName, @Name, @Votes)",
                    charList.ToArray());
            }

            return Redirect("~/");
        }
    }
}
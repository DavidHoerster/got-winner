using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using got_winner_voting.Model;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace got_winner_voting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        // GET api/values
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<Character>>> GetAll()
        {
            var db = Globals.GlobalItems.RedisConnection.Value.GetDatabase();
            var chars = await db.HashGetAllAsync("got");
            var charList = chars.Select(c =>
            {
                if (!c.Value.TryParse(out long votes))
                {
                    votes = 0;
                }
                return new Character { Name = c.Name, Votes = votes };
            });

            return Ok(charList);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<long>> Get(string id)
        {
            var db = Globals.GlobalItems.RedisConnection.Value.GetDatabase();
            var character = await db.HashGetAsync("got", id);
            if (!character.TryParse(out long votes))
            {
                votes = 0;
            }
            return Ok(votes);
        }

        // POST api/values
        [HttpPost("{id}")]
        public async Task<ActionResult<long>> Post(string id)
        {
            var db = Globals.GlobalItems.RedisConnection.Value.GetDatabase();
            var character = await db.HashGetAsync("got", id);
            var val = await db.HashIncrementAsync("got", id);
            return Ok(val);
        }

        // DELETE api/values/5
        [HttpDelete("")]
        public async Task<ActionResult> Delete()
        {
            var db = Globals.GlobalItems.RedisConnection.Value.GetDatabase();
            var chars = await db.HashGetAllAsync("got");
            var updChars = chars.Select(c => new HashEntry(c.Name, 0)).ToArray();
            await db.HashSetAsync("got", updChars);

            var charList = chars.Select(c => new Character { Name = c.Name, Votes = 0 });

            return Ok();
        }
    }
}

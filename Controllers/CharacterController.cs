﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using got_winner_voting.Model;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Microsoft.ApplicationInsights;

namespace got_winner_voting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private Lazy<ConnectionMultiplexer> _cache;
        public CharacterController(Lazy<ConnectionMultiplexer> cache)
        {
            _cache = cache;
        }
        // GET api/values
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<Character>>> GetAll()
        {
            var date = DateTimeOffset.UtcNow;
            var client = new TelemetryClient();

            var db = _cache.Value.GetDatabase();
            var chars = await db.HashGetAllAsync("got");


            client.TrackDependency("Redis Cache", "Get All Characters", "data", date, new TimeSpan(DateTimeOffset.UtcNow.Ticks - date.Ticks), true);


            var charList = chars.Select(c =>
            {
                if (!c.Value.TryParse(out long votes))
                {
                    votes = 0;
                }
                return new Character { Id = c.Name, Votes = votes };
            });

            return Ok(charList);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<long>> Get(string id)
        {
            var db = _cache.Value.GetDatabase();
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
            var db = _cache.Value.GetDatabase();
            var character = await db.HashGetAsync("got", id);
            var val = await db.HashIncrementAsync("got", id);
            return Ok(val);
        }

        // DELETE api/values/5
        [HttpDelete("")]
        public async Task<ActionResult> Delete()
        {
            var db = _cache.Value.GetDatabase();
            var chars = await db.HashGetAllAsync("got");
            var updChars = chars.Select(c => new HashEntry(c.Name, 0)).ToArray();
            await db.HashSetAsync("got", updChars);

            return Ok();
        }
    }
}


using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace got_winner_voting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BadController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> Get()
        {

            // throw new InvalidOperationException("You tried to do something bad");

            // return BadRequest();
            return Ok("all good now!");
        }

        [HttpGet("slow")]
        public async Task<ActionResult> GetSlow()
        {
            await Task.Delay(3000);

            return Ok("everything is a-ok");
        }
    }
}
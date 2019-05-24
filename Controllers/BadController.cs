
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
            // make it slow, too, and then fail
            await Task.Delay(1500);

            var x = 1;
            if (x == 1)
            {
                throw new InvalidOperationException("You tried to do something bad");
            }

            return BadRequest();
            // return Ok("all good now!");
        }

        [HttpGet("slow")]
        public async Task<ViewResult> Slow()
        {
            await Task.Delay(3000);

            return new ViewResult();
        }
    }
}
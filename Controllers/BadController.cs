
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

            throw new InvalidOperationException("You tried to do something bad");

            return BadRequest();
        }
    }
}
using KonkursBot.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KonkursBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(GetUserDataServices services) : ControllerBase
    {
        private readonly GetUserDataServices _services = services;

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _services.GetAllUsers());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] long Id)
        {
            try
            {
                return Ok(await _services.GetUserById(Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("all-refferals/{Id}")]
        public async Task<IActionResult> GetAllRefferals([FromRoute] long Id)
        {
            try
            {
                return Ok(await _services.GetAllUserRefferals(Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

using Core.Application.Interfaces;
using MedicalAppointmentSystem.API.Controllers.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace MedicalAppointmentSystem.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            var success = await _userRepository.RegisterUserAsync(model);
            if (!success)
                return StatusCode(500, "Registration failed.");

            return Ok(new { message = "User registered successfully!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserDto model)
        {
            var user = await _userRepository.LoginUserAsync(model);

            if (user == null)
                return Unauthorized("Invalid mobile number or password.");

            return Ok(new
            {
                message = "Login successful",
                user
            });
        }
    }
}
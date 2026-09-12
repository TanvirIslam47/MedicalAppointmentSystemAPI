using Core.Application.Services;
using Core.Domain.Entities;
using MedicalAppointmentSystem.API.Controllers.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MedicalAppointmentSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : BaseApiController
    {

        private readonly IConfiguration _configuration;

        public EmailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmailAsync([FromBody] EmailRequestModel model)
        {
     
            EmailService otpService = new EmailService(_configuration);

         
            try
            {

                await otpService.SendEmailAsync(model.RecipientEmail, model.Subject, model.Body, model.Body);
                return Ok("Email sent successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to send email: {ex.Message}");
            }
        }
    }
}

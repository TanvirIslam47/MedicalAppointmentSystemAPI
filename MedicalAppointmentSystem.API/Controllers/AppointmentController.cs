using Core.Application.Commands;
using Core.Application.Interfaces;
using Core.Application.Queries;
using MedicalAppointmentSystem.API.Controllers.Common;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace MedicalAppointmentSystem.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class AppointmentController : BaseApiController
    {
        private readonly IAppointmentRepository _repository;
        private readonly IEmailService _emailService;
        public AppointmentController(IAppointmentRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        [HttpPost("save-appointment")]
        public async Task<IActionResult> SaveAppointment([FromBody] AppointmentDto dto)
        {
            var result = await _mediatr.Send(new CreateAppointmentCommand { Model = dto });

            if (!result.IsSuccess)
                return BadRequest("Failed to save appointment");

            var appointment = await _repository.GetAppointmentByIdAsync(result.AppointmentId);

            if (appointment == null)
                return NotFound("Appointment details not found.");

            var emailSent = false;
            if (!string.IsNullOrWhiteSpace(appointment.PatientEmail))
            {
                await _emailService.SendAppointmentEmailAsync(appointment);
                emailSent = true;
            }

            return Ok(new
            {
                message = emailSent
                    ? "Appointment saved & email sent"
                    : "Appointment saved; no patient email is available",
                appointmentId = result.AppointmentId,
                appointment = appointment
            });
        }

        [HttpPost("appointments")]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentSaveDto dto)
        {
            var result = await _mediatr.Send(new SaveAppointmentCommand { Model = dto });

            if (result > 0)
            {
                var appointment = await _repository.GetAppointmentByIdAsync(result);
                if (appointment == null)
                    return NotFound("Appointment details not found.");

                await _emailService.SendAppointmentEmailAsync(appointment);
                return Ok(new { message = "Appointment saved and email sent successfully!", appointmentId = result });
            }

            return BadRequest(new { message = "Failed to save appointment" });
        }


        [HttpPut("appointments/{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDto dto)
        {
            if (dto == null)
                return BadRequest("Appointment data is required.");

            var result = await _mediatr.Send(new UpdateAppointmentCommand
            {
                AppointmentId = id,
                Model = dto
            });
            var appointment = await _repository.GetAppointmentByIdAsync(id);

            if (appointment == null)
                return NotFound("Appointment details not found.");

            var emailSent = false;
            if (!string.IsNullOrWhiteSpace(appointment.PatientEmail))
            {
                await _emailService.SendAppointmentEmailAsync(appointment);
                emailSent = true;
            }

            return Ok(new
            {
                result,
                message = emailSent
                    ? "Appointment updated and email sent"
                    : "Appointment updated; no patient email is available"
            });
        }

        [HttpDelete("appointments/{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Appointment Id.");

            var result = await _mediatr.Send(new DeleteAppointmentCommand { AppointmentId = id });

            if (!result)
                return NotFound($"Appointment with Id {id} not found.");

            return Ok(new { Message = $"Appointment {id} deleted successfully." });
        }

        //[HttpGet("appointments")]
        //public async Task<IActionResult> GetAppointments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        //{
        //    return Ok(await _mediatr.Send(new GetAppointmentsQuery(pageNumber, pageSize)));
        //}

        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointments(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string role = "Admin",
        [FromQuery] string? contactNumber = null)
        {
            var result = await _mediatr.Send(new GetAppointmentsQuery(pageNumber, pageSize, role, contactNumber));
            return Ok(result);
        }


        [HttpGet("appointments/{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var result = await _mediatr.Send(new GetAppointmentByIdQuery(id));
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("get-all-patients")]
        public async Task<IActionResult> GetPatients()
        {

            return Ok(await _mediatr.Send(new GetPatientsQuery()));
        }

        [HttpGet("get-all-doctors")]
        public async Task<IActionResult> GetDoctors()
        {

            return Ok(await _mediatr.Send(new GetDoctorsQuery()));
        }

        [HttpGet("get-all-medicines")]
        public async Task<IActionResult> GetMedicines()
        {

            return Ok(await _mediatr.Send(new GetMedicinesQuery()));
        }
        [HttpGet("get-all-department")]
        public async Task<IActionResult> GetDepartment()
        {

            return Ok(await _mediatr.Send(new GetDepartmentQuery()));
        }



    }
}


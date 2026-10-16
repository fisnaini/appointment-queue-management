using Microsoft.AspNetCore.Mvc;
using Qwiik.DTOs;
using Qwiik.Services;

namespace Qwiik.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _service;
    public AppointmentsController(IAppointmentService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest request)
    {
        try
        {
            var appointment = await _service.CreateAppointmentAsync(request);
            return CreatedAtAction(nameof(GetAppointmentsByDate),
                new { date = appointment.AppointmentDate.ToString("yyyy-MM-dd") }, appointment);
        }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpGet]
    public async Task<IActionResult> GetAppointmentsByDate([FromQuery] DateTime date) =>
        Ok(await _service.GetAppointmentsByDateAsync(date));
}

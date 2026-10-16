using Microsoft.AspNetCore.Mvc;
using Qwiik.DTOs;
using Qwiik.Services;

namespace Qwiik.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DaysOffController : ControllerBase
{
    private readonly IAppointmentService _service;
    public DaysOffController(IAppointmentService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetDaysOff() => Ok(await _service.GetDaysOffAsync());

    [HttpPost]
    public async Task<IActionResult> AddDayOff([FromBody] CreateDayOffRequest request)
    {
        try
        {
            var dayOff = await _service.AddDayOffAsync(request);
            return CreatedAtAction(nameof(GetDaysOff), new { id = dayOff.Id }, dayOff);
        }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> RemoveDayOff(int id) =>
        await _service.RemoveDayOffAsync(id) ? NoContent() : NotFound();
}

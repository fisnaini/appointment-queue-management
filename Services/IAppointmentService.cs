using Qwiik.DTOs;
using Qwiik.Models;

namespace Qwiik.Services;

public interface IAppointmentService
{
    Task<Appointment> CreateAppointmentAsync(CreateAppointmentRequest request);
    Task<List<Appointment>> GetAppointmentsByDateAsync(DateTime date);
    Task<List<DayOff>> GetDaysOffAsync();
    Task<DayOff> AddDayOffAsync(CreateDayOffRequest request);
    Task<bool> RemoveDayOffAsync(int id);
}

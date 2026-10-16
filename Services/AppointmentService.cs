using Microsoft.EntityFrameworkCore;
using Qwiik.Data;
using Qwiik.DTOs;
using Qwiik.Models;

namespace Qwiik.Services;

public class AppointmentService : IAppointmentService
{
    private const int MaxAppointmentsPerDay = 5;
    private readonly AppointmentDbContext _db;

    public AppointmentService(AppointmentDbContext db) => _db = db;

    public async Task<Appointment> CreateAppointmentAsync(CreateAppointmentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName))
            throw new ArgumentException("Customer name is required.");

        var requestedDate = request.AppointmentDate.Date;
        if (requestedDate < DateTime.Today)
            throw new ArgumentException("Appointment date cannot be in the past.");

        // Find the first date on or after the requested date that is open and has capacity.
        var date = requestedDate;
        for (var daysChecked = 0; daysChecked < 3660; daysChecked++, date = date.AddDays(1))
        {
            if (await _db.DaysOff.AnyAsync(d => d.Date == date))
                continue;

            var nextDay = date.AddDays(1);
            var count = await _db.Appointments.CountAsync(a =>
                a.AppointmentDate >= date && a.AppointmentDate < nextDay);

            if (count >= MaxAppointmentsPerDay)
                continue;

            var appointment = new Appointment
            {
                CustomerName = request.CustomerName.Trim(),
                AppointmentDate = date,
                TokenNumber = $"A{count + 1:000}",
                CreatedAt = DateTime.UtcNow
            };

            _db.Appointments.Add(appointment);
            await _db.SaveChangesAsync();
            return appointment;
        }

        throw new InvalidOperationException("No available appointment date was found.");
    }

    public async Task<List<Appointment>> GetAppointmentsByDateAsync(DateTime date)
    {
        var startOfDay = date.Date;
        var nextDay = startOfDay.AddDays(1);
        return await _db.Appointments
            .Where(a => a.AppointmentDate >= startOfDay && a.AppointmentDate < nextDay)
            .OrderBy(a => a.TokenNumber)
            .ToListAsync();
    }

    public async Task<List<DayOff>> GetDaysOffAsync() =>
        await _db.DaysOff.OrderBy(d => d.Date).ToListAsync();

    public async Task<DayOff> AddDayOffAsync(CreateDayOffRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("A description is required for a day off.");

        var date = request.Date.Date;
        if (date < DateTime.Today)
            throw new ArgumentException("A day off cannot be set in the past.");

        if (await _db.DaysOff.AnyAsync(d => d.Date == date))
            throw new InvalidOperationException("This date is already marked as a day off.");

        var dayOff = new DayOff { Date = date, Description = request.Description.Trim() };
        _db.DaysOff.Add(dayOff);
        await _db.SaveChangesAsync();
        return dayOff;
    }

    public async Task<bool> RemoveDayOffAsync(int id)
    {
        var dayOff = await _db.DaysOff.FindAsync(id);
        if (dayOff is null) return false;
        _db.DaysOff.Remove(dayOff);
        await _db.SaveChangesAsync();
        return true;
    }
}

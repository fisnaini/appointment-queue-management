using Microsoft.EntityFrameworkCore;
using Qwiik.Models;

namespace Qwiik.Data;

public class AppointmentDbContext : DbContext
{
    public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options)
        : base(options) { }

    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<DayOff> DaysOff => Set<DayOff>();
}

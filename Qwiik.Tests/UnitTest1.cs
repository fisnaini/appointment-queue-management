using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Qwiik.Data;
using Qwiik.DTOs;
using Qwiik.Services;
using Xunit;

namespace Qwiik.Tests;

public class UnitTest1
{
    private static async Task<(
        SqliteConnection Connection,
        AppointmentDbContext Db,
        AppointmentService Service)> CreateServiceAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppointmentDbContext>()
            .UseSqlite(connection)
            .Options;

        var db = new AppointmentDbContext(options);
        await db.Database.EnsureCreatedAsync();

        return (connection, db, new AppointmentService(db));
    }

    [Fact]
    public async Task CreateAppointment_ValidRequest_ReturnsTokenA001()
    {
        var (connection, db, service) = await CreateServiceAsync();
        using (connection)
        await using (db)
        {
            var date = DateTime.Today.AddDays(1);

            var result = await service.CreateAppointmentAsync(
                new CreateAppointmentRequest
                {
                    CustomerName = "Budi",
                    AppointmentDate = date
                });

            Assert.Equal("Budi", result.CustomerName);
            Assert.Equal("A001", result.TokenNumber);
            Assert.Equal(date, result.AppointmentDate);
        }
    }

    [Fact]
    public async Task CreateAppointment_EmptyName_ThrowsArgumentException()
    {
        var (connection, db, service) = await CreateServiceAsync();
        using (connection)
        await using (db)
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => service.CreateAppointmentAsync(
                    new CreateAppointmentRequest
                    {
                        CustomerName = " ",
                        AppointmentDate = DateTime.Today.AddDays(1)
                    }));
        }
    }

    [Fact]
    public async Task CreateAppointment_SixthAppointment_MovesToNextAvailableDay()
    {
        var (connection, db, service) = await CreateServiceAsync();
        using (connection)
        await using (db)
        {
            var requestedDate = DateTime.Today.AddDays(1);

            for (var i = 1; i <= 5; i++)
            {
                await service.CreateAppointmentAsync(
                    new CreateAppointmentRequest
                    {
                        CustomerName = $"Customer {i}",
                        AppointmentDate = requestedDate
                    });
            }

            var sixth = await service.CreateAppointmentAsync(
                new CreateAppointmentRequest
                {
                    CustomerName = "Customer 6",
                    AppointmentDate = requestedDate
                });

            Assert.Equal(requestedDate.AddDays(1), sixth.AppointmentDate);
            Assert.Equal("A001", sixth.TokenNumber);
            Assert.Equal("Customer 6", sixth.CustomerName);

            Assert.Equal(
                5,
                (await service.GetAppointmentsByDateAsync(requestedDate)).Count);
        }
    }

    [Fact]
    public async Task CreateAppointment_SkipsDayOff()
    {
        var (connection, db, service) = await CreateServiceAsync();
        using (connection)
        await using (db)
        {
            var holiday = DateTime.Today.AddDays(1);

            await service.AddDayOffAsync(
                new CreateDayOffRequest
                {
                    Date = holiday,
                    Description = "Public holiday"
                });

            var appointment = await service.CreateAppointmentAsync(
                new CreateAppointmentRequest
                {
                    CustomerName = "Dian",
                    AppointmentDate = holiday
                });

            Assert.Equal(holiday.AddDays(1), appointment.AppointmentDate);
            Assert.Equal("A001", appointment.TokenNumber);
        }
    }

    [Fact]
    public async Task CreateAppointment_SkipsFullDayAndHoliday()
    {
        var (connection, db, service) = await CreateServiceAsync();
        using (connection)
        await using (db)
        {
            var requestedDate = DateTime.Today.AddDays(1);
            var nextDate = requestedDate.AddDays(1);

            // Fill the requested date to its daily limit.
            for (var i = 1; i <= 5; i++)
            {
                await service.CreateAppointmentAsync(
                    new CreateAppointmentRequest
                    {
                        CustomerName = $"Customer {i}",
                        AppointmentDate = requestedDate
                    });
            }

            // Mark the next date as a holiday.
            await service.AddDayOffAsync(
                new CreateDayOffRequest
                {
                    Date = nextDate,
                    Description = "Public holiday"
                });

            var appointment = await service.CreateAppointmentAsync(
                new CreateAppointmentRequest
                {
                    CustomerName = "Customer 6",
                    AppointmentDate = requestedDate
                });

            Assert.Equal(nextDate.AddDays(1), appointment.AppointmentDate);
            Assert.Equal("A001", appointment.TokenNumber);
        }
    }

    [Fact]
    public async Task AddDayOff_DuplicateDate_ThrowsInvalidOperationException()
    {
        var (connection, db, service) = await CreateServiceAsync();
        using (connection)
        await using (db)
        {
            var date = DateTime.Today.AddDays(1);

            await service.AddDayOffAsync(
                new CreateDayOffRequest
                {
                    Date = date,
                    Description = "Holiday"
                });

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.AddDayOffAsync(
                    new CreateDayOffRequest
                    {
                        Date = date,
                        Description = "Duplicate holiday"
                    }));
        }
    }

    [Fact]
    public async Task AddDayOff_PastDate_ThrowsArgumentException()
    {
        var (connection, db, service) = await CreateServiceAsync();
        using (connection)
        await using (db)
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => service.AddDayOffAsync(
                    new CreateDayOffRequest
                    {
                        Date = DateTime.Today.AddDays(-1),
                        Description = "Past holiday"
                    }));
        }
    }
}

namespace Qwiik.Models;

public class Appointment
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public DateTime AppointmentDate { get; set; }

    public string TokenNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

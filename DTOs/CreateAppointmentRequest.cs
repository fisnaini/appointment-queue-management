namespace Qwiik.DTOs;

public class CreateAppointmentRequest
{
    public string CustomerName { get; set; } = string.Empty;

    public DateTime AppointmentDate { get; set; }
}

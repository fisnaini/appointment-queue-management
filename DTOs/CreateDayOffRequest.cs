namespace Qwiik.DTOs;

public class CreateDayOffRequest
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
}

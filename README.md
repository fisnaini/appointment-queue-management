# Qwiik Appointment Queue

Qwiik is a simple appointment booking app built with ASP.NET Core and SQLite. It allows an agency to manage customer appointments, assign queue tokens, and keep track of daily bookings through a web interface.

## Features

- Book appointments and assign queue tokens (`A001` to `A005`).
- View appointments by date.
- Set a daily limit of five appointments.
- Automatically schedule bookings for the next available date if the requested date is full or marked as a day off.
- Add and remove days off, such as public holidays.
- View and test API endpoints through Swagger.

## Requirements

- .NET 10 SDK

## Running the App

1. Open the folder containing `Qwiik.csproj` in VS Code.
2. Open the integrated terminal.
3. Restore the required packages:

   ```bash
   dotnet restore
   ```

4. Start the application:

   ```bash
   dotnet run
   ```

5. Open the local URL shown in the terminal to access the Qwiik interface.

To access the API documentation, open `/swagger` on the same local address.

## API Endpoints

### Appointments

| Method | Endpoint                            | Description                           |
| ------ | ----------------------------------- | ------------------------------------- |
| POST   | `/api/appointments`                 | Create an appointment                 |
| GET    | `/api/appointments?date=YYYY-MM-DD` | View appointments for a specific date |

Example request body:

```json
{
  "customerName": "Budi",
  "appointmentDate": "2026-10-16"
}
```

When an appointment is created, the response includes its queue token and scheduled date. If the requested date is full or marked as a day off, the app looks for the next available date.

### Days Off

| Method | Endpoint            | Description       |
| ------ | ------------------- | ----------------- |
| GET    | `/api/daysoff`      | View all days off |
| POST   | `/api/daysoff`      | Add a day off     |
| DELETE | `/api/daysoff/{id}` | Remove a day off  |

Example request body:

```json
{
  "date": "2026-12-25",
  "description": "Public holiday"
}
```

A day off cannot be added for a past date, and duplicate dates are not allowed. Days off can also be managed directly through the web interface.

## Booking Rules

- Customer names are required.
- Appointments cannot be booked for dates in the past.
- Each date can have up to five appointments.
- Queue tokens start at `A001` for each date.
- If a date is full or marked as a day off, the appointment is moved to the next available date.
- The actual appointment date and assigned token are returned in the API response.

## Running the Tests

Run the following command from the project directory:

```bash
dotnet test Qwiik.Tests/Qwiik.Tests.csproj
```

The unit tests cover successful bookings, empty customer names, bookings when the daily limit is reached, and day-off handling.

## Project Structure

```text
Qwiik/
├── Controllers/   # API endpoints
├── Data/          # Database context
├── DTOs/          # Request objects
├── Models/        # Data models
├── Services/      # Booking logic
├── wwwroot/       # Web interface
└── Qwiik.Tests/   # Unit tests
```

## Technologies Used

- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger / OpenAPI
- HTML, CSS, and JavaScript
- xUnit

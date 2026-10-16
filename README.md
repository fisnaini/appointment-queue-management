# Appointment Queue Management

This project is a simple appointment booking application built with ASP.NET Core and SQLite.

Users can book appointments, get a queue number, and check the appointment list for a specific date. The application also handles dates that are already full or marked as days off by finding the next available date.

## Features

- Book an appointment and get a queue number.
- View appointments by date.
- Limit bookings to five appointments per day.
- Automatically find the next available date when a date is full or marked as a day off.
- Add and remove days off.
- Test the main booking and validation scenarios.

## Technologies Used

- C# and ASP.NET Core
- Entity Framework Core
- SQLite
- HTML, CSS, and JavaScript
- Swagger for API testing
- xUnit for unit testing

## How to Run

Make sure the .NET 10 SDK is installed.

Open a terminal in the project folder and run:

```bash
dotnet restore
dotnet run
```

Once the application starts, open the local URL shown in the terminal.

To test the API, add `/swagger` to the local URL.

## Run the Tests

From the project folder, run:

```bash
dotnet test Qwiik.Tests/Qwiik.Tests.csproj
```

## API Endpoints

| Method | Endpoint                            | Purpose                      |
| ------ | ----------------------------------- | ---------------------------- |
| POST   | `/api/appointments`                 | Create a new appointment     |
| GET    | `/api/appointments?date=YYYY-MM-DD` | View appointments for a date |
| POST   | `/api/daysoff`                      | Add a day off                |
| DELETE | `/api/daysoff/{date}`               | Remove a day off             |

## Project Structure

- `Controllers` – Handles API requests.
- `Data` – Contains the database context.
- `DTOs` – Defines request data.
- `Models` – Contains the application data models.
- `Services` – Handles the booking rules and other application logic.
- `wwwroot` – Contains the web interface.
- `Qwiik.Tests` – Contains unit tests.

## Notes

The application uses a local SQLite database. Each date can have up to five appointments, and bookings are moved to the next available date when necessary.

using Microsoft.EntityFrameworkCore;
using Qwiik.Data;
using Qwiik.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<AppointmentDbContext>(options => options.UseSqlite("Data Source=qwiik.db"));
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppointmentDbContext>();
    db.Database.EnsureCreated();
    // EnsureCreated won't add tables to an existing database, so create this table explicitly too.
    db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS \"DaysOff\" (\"Id\" INTEGER NOT NULL CONSTRAINT \"PK_DaysOff\" PRIMARY KEY AUTOINCREMENT, \"Date\" TEXT NOT NULL, \"Description\" TEXT NOT NULL);");
    db.Database.ExecuteSqlRaw("CREATE UNIQUE INDEX IF NOT EXISTS \"IX_DaysOff_Date\" ON \"DaysOff\" (\"Date\");");
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

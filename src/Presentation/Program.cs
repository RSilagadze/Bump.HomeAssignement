using System.Text.Json.Serialization;
using Application.Appointments;
using Application.Clients;
using Application.Patients;
using Domain.Entities;
using Domain.Ports;
using Infrastructure.DTOs;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Routing.Constraints;

//Very simple example
var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddRoutingCore().Configure<RouteOptions>(options => {
    options.SetParameterPolicy<RegexInlineRouteConstraint>("regex");
});

var clientsDataHolder = new List<ClientDbDTO>();
var patientsDataHolder = new List<PatientDbDTO>();
var appointmentsDataHolder = new List<AppointmentDbDTO>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddSingleton<IPatientRepository>(_ =>
{
    return new PatientRepository(() => patientsDataHolder);
});
builder.Services.AddSingleton<IClientRepository>(_ =>
{
    return new ClientRepository(() => clientsDataHolder ,() => patientsDataHolder);
});
builder.Services.AddSingleton<IAppointmentRepository>(_ =>
{
    return new AppointmentRepository(() => patientsDataHolder, () => appointmentsDataHolder);
});

builder.Services.AddSingleton<AppointmentHandler>(p =>
{
    var appointmentRepository = p.GetRequiredService<IAppointmentRepository>();
    var patientRepository = p.GetRequiredService<IPatientRepository>();
    return new AppointmentHandler(appointmentRepository, patientRepository);
});

builder.Services.AddSingleton<ClientHandler>(p =>
{
    var clientRepository = p.GetRequiredService<IClientRepository>();
    return new ClientHandler(clientRepository);
});

builder.Services.AddSingleton<PatientHandler>(p =>
{
    var patientRepository = p.GetRequiredService<IPatientRepository>();
    var clientRepository = p.GetRequiredService<IClientRepository>();
    return new PatientHandler(patientRepository, clientRepository);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/index", () => Results.Redirect("swagger/index.html")).ExcludeFromDescription();
app.MapGet("/index.html", () => Results.Redirect("swagger/index.html")).ExcludeFromDescription();
app.MapGet("", () => Results.Redirect("swagger/index.html")).ExcludeFromDescription();

var clientsApi = app.MapGroup("/clients").WithTags("clients").WithOpenApi();
clientsApi.MapPost("/", async ([FromServices] ClientHandler handler, [FromBody] CreateClientCommand cmd) =>
{
    var result = await handler.CreateClientAsync(cmd);
    return Results.Created($"/clients/{result.Id}", result);
});

var patientsApi = app.MapGroup("/clients/patients").WithTags("patients").WithOpenApi();
patientsApi.MapPost("/", async ([FromServices] PatientHandler handler, [FromBody] CreatePatientCommand cmd) =>
{
    var result = await handler.CreatePatientAsync(cmd);
    return Results.Created($"/patients/{result.Id}", result);
});

var appointmentsApi = app.MapGroup("/appointments").WithTags("appointments").WithOpenApi();
appointmentsApi.MapPost("/",
    async ([FromServices] AppointmentHandler handler, [FromBody] ScheduleAppointmentCommand cmd) =>
    {
        var result = await handler.ScheduleAppointmentAsync(cmd);
        return Results.Created($"/appointments/{result.Id}", result);
    });
appointmentsApi.MapPatch("/",
    async ([FromServices] AppointmentHandler handler, [FromBody] ReScheduleAppointmentCommand cmd) =>
    {
        var result = await handler.ReScheduleAppointmentAsync(cmd);
        return Results.Ok(result);
    });
appointmentsApi.MapPost("/cancel",
    async ([FromServices] AppointmentHandler handler, [FromBody] CancelAppointmentCommand cmd) =>
    {
        var result = await handler.CancelAppointmentAsync(cmd);
        return Results.Ok(result);
    });

app.Run();


[JsonSerializable(typeof(Appointment))]
[JsonSerializable(typeof(Patient))]
[JsonSerializable(typeof(Client))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(DateTime?))]
[JsonSerializable(typeof(CreateClientCommand))]
[JsonSerializable(typeof(CreatePatientCommand))]
[JsonSerializable(typeof(ScheduleAppointmentCommand))]
[JsonSerializable(typeof(ReScheduleAppointmentCommand))]
[JsonSerializable(typeof(CancelAppointmentCommand))]
[JsonSourceGenerationOptions(AllowTrailingCommas = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, WriteIndented = true, DefaultBufferSize = 4096, PropertyNameCaseInsensitive = true)]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}

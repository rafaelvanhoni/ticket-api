using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TicketDbContext>(options =>
    options.UseSqlite("Data Source = Data/tickets.db"));
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<ITicketRepository, EfTicketRepository>();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true))
);
builder.Services.AddSwaggerGen(options =>
    options.SchemaFilter<EnumSchemaFilter>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/tickets/{id}", async (int id, TicketService service) =>
{
    var result = await service.GetTicketByIdAsync(id);
    return result.ToHttpResult();
})
.WithName("GetTicketById")
.WithOpenApi();

app.MapGet("/tickets", async (string? status, string? priority, TicketService service) =>
{

    TicketStatus? parsedStatus = null;
    TicketPriority? parsedPriority = null;

    if (!string.IsNullOrWhiteSpace(status))
    {
        if (!Enum.TryParse<TicketStatus>(status, true, out var statusValue) ||
            !Enum.IsDefined(typeof(TicketStatus), statusValue))
        {
            return new OperationResult<IEnumerable<Ticket>>()
            {
                Status = ResultStatus.ValidationError,
                Message = "Invalid ticket status.",
            }.ToHttpResult();
        }

        parsedStatus = statusValue;
    }

    if (!string.IsNullOrWhiteSpace(priority))
    {
        if (!Enum.TryParse<TicketPriority>(priority, true, out var priorityValue) ||
            !Enum.IsDefined(typeof(TicketPriority), priorityValue))
        {
            return new OperationResult<IEnumerable<Ticket>>()
            {
                Status = ResultStatus.ValidationError,
                Message = "Invalid ticket priority.",
            }.ToHttpResult();
        }

        parsedPriority = priorityValue;
    }

    var result = await service.GetTicketsAsync(parsedStatus, parsedPriority);
    return result.ToHttpResult();
})
.WithName("GetTickets")
.WithOpenApi();

app.MapPost("/tickets", async (CreateTicketDto dto, TicketService service) =>
{
    var result = await service.AddTicketAsync(dto);
    return result.ToHttpResult();
})
.WithName("CreateTicket")
.WithOpenApi();

app.MapPut("/tickets/{id}", async (int id, UpdateTicketDto dto, TicketService service) =>
{
    var result = await service.UpdateTicketAsync(id, dto);
    return result.ToHttpResult();
})
.WithName("UpdateTicket")
.WithOpenApi();

app.MapDelete("/tickets/{id}", async (int id, TicketService service) =>
{
    var result = await service.DeleteTicketAsync(id);
    return result.ToHttpResult();
})
.WithName("DeleteTicket")
.WithOpenApi();

app.Run();

public partial class Program { }
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<TicketService>();
builder.Services.AddSingleton<ITicketRepository, TicketRepository>();

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/tickets/{id}", (int id, TicketService service) =>
{
    var ticket = service.GetTicketById(id);
    return ticket is null ? Results.NotFound() : Results.Ok(ticket);

})
.WithName("GetTicketById")
.WithOpenApi();

app.MapGet("/tickets", (TicketStatus? status, TicketPriority? priority, TicketService service) =>
{
    return Results.Ok(service.GetTickets(status, priority));
})
.WithName("GetTickets")
.WithOpenApi();

app.MapPost("/tickets", (CreateTicketDto dto, TicketService service) =>
{

    var result = service.AddTicket(dto);

    if (!result.IsSuccess)
        return Results.BadRequest(result.Message);

    return Results.Created($"/tickets/{result.Data!.Id}", result.Data);

})
.WithName("CreateTicket")
.WithOpenApi();

app.MapPut("/tickets/{id}", (int id, UpdateTicketDto dto, TicketService service) =>
{
    var result = service.UpdateTicket(id, dto);
    if (!result.IsSuccess)
    {
        if (result.Data is null)
            return Results.NotFound(result.Message);

        return Results.BadRequest(result.Message);
    }

    return Results.Ok(result.Data);

})
.WithName("UpdateTicket")
.WithOpenApi();

app.MapDelete("/tickets/{id}", (int id, TicketService service) =>
{
    var result = service.DeleteTicket(id);
    if (!result.IsSuccess)
    {
        if (result.Data is null)
            return Results.NotFound(result.Message);

        return Results.BadRequest(result.Message);
    }

    return Results.Ok(result.Data);
})
.WithName("DeleteTicket")
.WithOpenApi();

app.Run();
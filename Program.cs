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

// ObeterTicket por Id
app.MapGet("/tickets/{id}", (int id, TicketService service) =>
{
    var ticket = service.ObterTicketPorId(id);
    return ticket is null ? Results.NotFound() : Results.Ok(ticket);

})
.WithName("GetTicketId")
.WithOpenApi();

// ObterTicket
app.MapGet("/tickets", (TicketStatus? status, TicketPriority? prioridade, TicketService service) =>
{
    var listaTickets = service.ObterTickets(status, prioridade);
    return Results.Ok(listaTickets);
})
.WithName("GetTickets")
.WithOpenApi();

app.MapPost("/tickets", (CreateTicketDto dto, TicketService service) =>
{
    var ticket = service.AdicionarTicket(dto);
    return Results.Created($"/tickets/{ticket.Id}", ticket);
})
.WithName("PostTickets")
.WithOpenApi();

app.Run();
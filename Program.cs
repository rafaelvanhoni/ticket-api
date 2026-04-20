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

    var resultado = service.AdicionarTicket(dto);

    if (!resultado.IsSuccess)
        return Results.BadRequest(resultado.Message);

    return Results.Created($"/tickets/{resultado.Data!.Id}", resultado.Data);

})
.WithName("PostTickets")
.WithOpenApi();

app.MapPut("/tickets/{id}", (int id, UpdateTicketDto dto, TicketService service) =>
{
    var resultado = service.AtualizarTicket(id, dto);
    if (!resultado.IsSuccess)
    {
        if (resultado.Data is null)
            return Results.NotFound(resultado.Message);

        return Results.BadRequest(resultado.Message);
    }

    return Results.Ok(resultado.Data);

})
.WithName("PutTicket")
.WithOpenApi();

app.Run();
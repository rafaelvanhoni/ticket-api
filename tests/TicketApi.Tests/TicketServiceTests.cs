public class TicketServiceTests
{

    [Fact]
    public void AddTicket_ShouldReturnSuccess_WhenValidData()
    {
        // Given
        var repository = new TicketRepository();
        var service = new TicketService(repository);
        var dto = new CreateTicketDto()
        {
            Title = "Teste de Inclusao",
            Description = "Primeiro teste de inclusao de ticket",
            Status = TicketStatus.Open,
            Priority = TicketPriority.High,
            AssignedTo = "Teste"
        };

        // When
        var result = service.AddTicket(dto);

        // Then
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Teste de Inclusao", result.Data.Title);
    }

    [Fact]
    public void AddTicket_ShouldReturnFailure_WhenTitleIsEmpty()
    {
        // Given
        var repository = new TicketRepository();
        var service = new TicketService(repository);
        var dto = new CreateTicketDto()
        {
            Title = "",
            Description = "Teste sem o titulo no ticket",
            Status = TicketStatus.Open,
            Priority = TicketPriority.Low
        };

        // When
        var result = service.AddTicket(dto);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal("Title is required.", result.Message);
    }

    [Fact]
    public void AddTicket_ShouldReturnFailure_WhenDescriptionIsEmpty()
    {
        // Given
        var repository = new TicketRepository();
        var service = new TicketService(repository);
        var dto = new CreateTicketDto()
        {
            Title = "Titulo sem descricao",
            Description = "",
            Status = TicketStatus.Open,
            Priority = TicketPriority.High
        };

        // When
        var result = service.AddTicket(dto);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal("Description is required.", result.Message);
    }

    [Fact]
    public void DeleteTicket_ShouldReturnSuccess_WhenTicketExistsAndNotCompleted()
    {
        // Given
        var repository = new TicketRepository();
        var service = new TicketService(repository);
        var created = service.AddTicket(
            new CreateTicketDto()
            {
                Title = "Simulando ticket",
                Description = "Descricao da simulacao de ticket",
                Status = TicketStatus.Closed,
                Priority = TicketPriority.Medium,
                AssignedTo = "Teste"
            });
        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);

        // When
        var result = service.DeleteTicket(created.Data!.Id);

        // Then
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(string.Empty, result.Message);
    }

    [Fact]
    public void DeleteTicket_ShouldReturnFailure_WhenTicketDoesNotExist()
    {
        // Given
        var repository = new TicketRepository();
        var service = new TicketService(repository);

        // When
        var result = service.DeleteTicket(999);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal("Ticket not found.", result.Message);
    }

    [Fact]
    public void DeleteTicket_ShouldReturnFailure_WhenTicketIsCompleted()
    {
        // Given
        var repository = new TicketRepository();
        var service = new TicketService(repository);

        var created = service.AddTicket(
            new CreateTicketDto()
            {
                Title = "Ticket concluido",
                Description = "Ticket concluido nao pode ser deletado",
                Status = TicketStatus.Completed,
                Priority = TicketPriority.High,
                AssignedTo = "Teste"
            });
        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);


        // When
        var result = service.DeleteTicket(created.Data!.Id);

        // Then
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Completed tickets cannot be deleted.", result.Message);
    }

    [Fact]
    public void GetTicketById_ShouldReturnTicket_WhenIdExists()
    {
        // Given
        var repository = new TicketRepository();
        var service = new TicketService(repository);
        var created = service.AddTicket(
            new CreateTicketDto()
            {
                Title = "Ticket valido",
                Description = "Encontrando ticket por id",
                Status = TicketStatus.Completed,
                Priority = TicketPriority.High,
                AssignedTo = "Teste"
            });
        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);

        // When
        var ticket = service.GetTicketById(created.Data!.Id);

        // Then
        Assert.NotNull(ticket);
        Assert.Equal("Ticket valido", ticket.Title);
    }

    [Fact]
    public void GetTicketById_ShouldReturnNull_WhenIdDoesNotExist()
    {
        // Given
        var repository = new TicketRepository();
        var service = new TicketService(repository);

        // When
        var ticket = service.GetTicketById(999);

        // Then
        Assert.Null(ticket);
    }

    [Fact]
    public void GetTickets_ShouldReturnOnlyOpenTickets_WhenStatusIsOpen()
    {
        // Given
        var repository = new TicketRepository();
        var service = new TicketService(repository);

        var createdOpen1 = service.AddTicket(
            new CreateTicketDto()
            {
                Title = "Ticket 1",
                Description = "Ticket 1 Aberto",
                Status = TicketStatus.Open,
                Priority = TicketPriority.High,
                AssignedTo = "Teste"
            });
        Assert.True(createdOpen1.IsSuccess);
        Assert.NotNull(createdOpen1.Data);

        var createdOpen2 = service.AddTicket(
            new CreateTicketDto()
            {
                Title = "Ticket 2",
                Description = "Ticket 2 Aberto",
                Status = TicketStatus.Open,
                Priority = TicketPriority.High,
                AssignedTo = "Teste"
            });
        Assert.True(createdOpen2.IsSuccess);
        Assert.NotNull(createdOpen2.Data);

        var createdClosed = service.AddTicket(
            new CreateTicketDto()
            {
                Title = "Ticket 3",
                Description = "Ticket 3 Fechado",
                Status = TicketStatus.Closed,
                Priority = TicketPriority.High,
                AssignedTo = "Teste"
            });
        Assert.True(createdClosed.IsSuccess);
        Assert.NotNull(createdClosed.Data);

        // When
        var tickets = service.GetTickets(status: TicketStatus.Open);

        // Then
        Assert.All(tickets, ticket => Assert.Equal(TicketStatus.Open, ticket.Status));

    }

}
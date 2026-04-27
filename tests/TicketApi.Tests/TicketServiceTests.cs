
public class TicketServiceTests
{
    private TicketService CreateService()
    {
        var repository = new FakeTicketRepository();
        return new TicketService(repository);
    }

    [Fact]
    public void AddTicket_ShouldReturnSuccess_WhenValidData()
    {
        // Given
        var service = CreateService();
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
        var service = CreateService();
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
        var service = CreateService();
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
        var service = CreateService();
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
        var service = CreateService();

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
        var service = CreateService();

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
        var service = CreateService();
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
        var service = CreateService();

        // When
        var ticket = service.GetTicketById(999);

        // Then
        Assert.Null(ticket);
    }

    [Fact]
    public void GetTickets_ShouldReturnOnlyOpenTickets_WhenStatusIsOpen()
    {
        // Given
        var service = CreateService();

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
        Assert.NotEmpty(tickets);
        Assert.All(tickets, ticket => Assert.Equal(TicketStatus.Open, ticket.Status));

    }

    [Fact]
    public void UpdateTicket_ShouldReturnSuccess_WhenIdExistsAndDataIsValid()
    {
        // Given
        var service = CreateService();
        var created = service.AddTicket(
            new CreateTicketDto()
            {
                Title = "Registro criado",
                Description = "Novo registro criado",
                Status = TicketStatus.Open,
                Priority = TicketPriority.Low,
                AssignedTo = "Teste"
            });
        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);

        var dto = new UpdateTicketDto()
        {
            Title = "Registro alterado",
            Description = "Registro alterado via update",
            Status = TicketStatus.Completed,
            Priority = TicketPriority.Medium,
            AssignedTo = "Rafael"
        };

        // When
        var result = service.UpdateTicket(created.Data.Id, dto);
        var ticket = result.Data;

        // Then
        Assert.True(result.IsSuccess);
        Assert.NotNull(ticket);
        Assert.Equal(created.Data.Id, ticket.Id);
        Assert.Equal("Registro alterado", ticket.Title);
        Assert.Equal("Registro alterado via update", ticket.Description);
        Assert.Equal(TicketStatus.Completed, ticket.Status);
        Assert.Equal(TicketPriority.Medium, ticket.Priority);
        Assert.Equal("Rafael", ticket.AssignedTo);
        Assert.NotNull(ticket.UpdatedAt);
        Assert.NotNull(ticket.CompletedAt);

    }

    [Fact]
    public void UpdateTicket_ShouldReturnFailure_WhenTicketDoesNotExist()
    {
        // Given
        var service = CreateService();
        var dto = new UpdateTicketDto()
        {
            Title = "Registro alterado",
            Description = "Registro alterado via update",
            Status = TicketStatus.Completed,
            Priority = TicketPriority.Medium,
            AssignedTo = "Rafael"
        };

        // When
        var result = service.UpdateTicket(999, dto);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal("Ticket not found.", result.Message);
    }

    [Fact]
    public void UpdateTicket_ShouldReturnFailure_WhenTitleIsEmpty()
    {
        // Given
        var service = CreateService();

        var dtoAdd = new CreateTicketDto()
        {
            Title = "Inclusao",
            Description = "Inclusao de ticket",
            Status = TicketStatus.Open,
            Priority = TicketPriority.High,
            AssignedTo = "Teste"
        };

        var created = service.AddTicket(dtoAdd);

        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);

        // When
        var result = service.UpdateTicket(created.Data.Id, new UpdateTicketDto()
        {
            Title = "",
            Description = "Alteracao de ticket",
            Status = TicketStatus.Open,
            Priority = TicketPriority.High,
            AssignedTo = "Teste"
        });

        // Then
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal("Title is required.", result.Message);
    }

    [Fact]
    public void UpdateTicket_ShouldReturnFailure_WhenDescriptionIsEmpty()
    {
        // Given
        var service = CreateService();

        var dtoAdd = new CreateTicketDto()
        {
            Title = "Inclusao",
            Description = "Inclusao de ticket",
            Status = TicketStatus.Open,
            Priority = TicketPriority.High,
            AssignedTo = "Teste"
        };

        var created = service.AddTicket(dtoAdd);

        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);

        // When
        var result = service.UpdateTicket(created.Data.Id, new UpdateTicketDto()
        {
            Title = "Alteracao",
            Description = "",
            Status = TicketStatus.Closed,
            Priority = TicketPriority.Low,
            AssignedTo = "Rafael"
        });

        // Then
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal("Description is required.", result.Message);
    }

}
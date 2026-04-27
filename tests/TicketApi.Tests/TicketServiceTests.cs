public class TicketServiceTests
{
    private TicketService CreateService()
    {
        var repository = new FakeTicketRepository();
        return new TicketService(repository);
    }

    private CreateTicketDto CreateValidCreateTicketDto()
    {
        return new CreateTicketDto()
        {
            Title = "Default title",
            Description = "Default description",
            Status = TicketStatus.Open,
            Priority = TicketPriority.Medium,
            AssignedTo = "Test"
        };
    }

    private UpdateTicketDto CreateValidUpdateTicketDto()
    {
        return new UpdateTicketDto()
        {
            Title = "Updated title",
            Description = "Updated description",
            Status = TicketStatus.Open,
            Priority = TicketPriority.Medium,
            AssignedTo = "Test"
        };
    }

    [Fact]
    public void AddTicket_ShouldReturnSuccess_WhenValidData()
    {
        // Given
        var service = CreateService();
        var dto = CreateValidCreateTicketDto();

        // When
        var result = service.AddTicket(dto);

        // Then
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(dto.Title, result.Data.Title);
    }

    [Fact]
    public void AddTicket_ShouldReturnFailure_WhenTitleIsEmpty()
    {
        // Given
        var service = CreateService();
        var dto = CreateValidCreateTicketDto();
        dto.Title = "";

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
        var dto = CreateValidCreateTicketDto();
        dto.Description = "";

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
        var created = service.AddTicket(CreateValidCreateTicketDto());
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

        var dto = CreateValidCreateTicketDto();
        dto.Status = TicketStatus.Completed;

        var created = service.AddTicket(dto);
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
        var dto = CreateValidCreateTicketDto();

        var created = service.AddTicket(dto);
        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);

        // When
        var ticket = service.GetTicketById(created.Data!.Id);

        // Then
        Assert.NotNull(ticket);
        Assert.Equal(dto.Title, ticket.Title);
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

        var dto1 = CreateValidCreateTicketDto();
        dto1.Title = "Ticket 1";
        dto1.Status = TicketStatus.Open;
        var createdOpen1 = service.AddTicket(dto1);
        Assert.True(createdOpen1.IsSuccess);
        Assert.NotNull(createdOpen1.Data);

        var dto2 = CreateValidCreateTicketDto();
        dto2.Title = "Ticket 2";
        dto2.Status = TicketStatus.Open;
        var createdOpen2 = service.AddTicket(dto2);
        Assert.True(createdOpen2.IsSuccess);
        Assert.NotNull(createdOpen2.Data);

        var dto3 = CreateValidCreateTicketDto();
        dto3.Title = "Ticket 3";
        dto3.Status = TicketStatus.Closed;
        var createdClosed = service.AddTicket(dto3);
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
        var created = service.AddTicket(CreateValidCreateTicketDto());
        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);

        var dto = CreateValidUpdateTicketDto();

        // When
        var result = service.UpdateTicket(created.Data.Id, dto);
        var ticket = result.Data;

        // Then
        Assert.True(result.IsSuccess);
        Assert.NotNull(ticket);
        Assert.Equal(created.Data.Id, ticket.Id);
        Assert.Equal(dto.Title, ticket.Title);
        Assert.Equal(dto.Description, ticket.Description);
        Assert.Equal(dto.Status, ticket.Status);
        Assert.Equal(dto.Priority, ticket.Priority);
        Assert.Equal(dto.AssignedTo, ticket.AssignedTo);
        Assert.NotNull(ticket.UpdatedAt);

    }

    [Fact]
    public void UpdateTicket_ShouldSetCompletedAt_WhenStatusIsCompleted()
    {
        // Given
        var service = CreateService();
        var created = service.AddTicket(CreateValidCreateTicketDto());
        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);

        var dto = CreateValidUpdateTicketDto();
        dto.Status = TicketStatus.Completed;

        // When
        var result = service.UpdateTicket(created.Data.Id, dto);
        var ticket = result.Data;

        // Then
        Assert.True(result.IsSuccess);
        Assert.NotNull(ticket);
        Assert.Equal(created.Data.Id, ticket.Id);
        Assert.Equal(dto.Title, ticket.Title);
        Assert.Equal(dto.Description, ticket.Description);
        Assert.Equal(dto.Status, ticket.Status);
        Assert.Equal(dto.Priority, ticket.Priority);
        Assert.Equal(dto.AssignedTo, ticket.AssignedTo);
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
        var created = service.AddTicket(CreateValidCreateTicketDto());
        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);

        var dto = CreateValidUpdateTicketDto();
        dto.Title = "";

        // When
        var result = service.UpdateTicket(created.Data.Id, dto);

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
        var created = service.AddTicket(CreateValidCreateTicketDto());
        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);

        var dto = CreateValidUpdateTicketDto();
        dto.Description = "";

        // When
        var result = service.UpdateTicket(created.Data.Id, dto);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal("Description is required.", result.Message);
    }

}
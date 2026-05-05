using System.Threading.Tasks;
using Moq;

public class TicketServiceTests
{
    private TicketService CreateService()
    {
        var repository = new FakeTicketRepository();
        return new TicketService(repository);
    }

    private TicketService CreateService(ITicketRepository repository)
    {
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
    public async Task AddTicket_ShouldReturnSuccess_WhenValidData()
    {
        // Given
        var service = CreateService();
        var dto = CreateValidCreateTicketDto();

        // When
        var result = await service.AddTicketAsync(dto);

        // Then
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(dto.Title, result.Data.Title);
    }

    [Fact]
    public async Task AddTicket_ShouldReturnFailure_WhenTitleIsEmpty()
    {
        // Given
        var service = CreateService();
        var dto = CreateValidCreateTicketDto();
        dto.Title = "";

        // When
        var result = await service.AddTicketAsync(dto);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal("Title is required.", result.Message);
    }

    [Fact]
    public async Task AddTicket_ShouldReturnFailure_WhenDescriptionIsEmpty()
    {
        // Given
        var service = CreateService();
        var dto = CreateValidCreateTicketDto();
        dto.Description = "";

        // When
        var result = await service.AddTicketAsync(dto);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal("Description is required.", result.Message);
    }

    [Fact]
    public async Task DeleteTicket_ShouldReturnSuccess_WhenTicketExistsAndNotCompleted()
    {
        // Given
        var service = CreateService();
        var created = await service.AddTicketAsync(CreateValidCreateTicketDto());
        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);

        // When
        var result = await service.DeleteTicketAsync(created.Data!.Id);

        // Then
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Null(result.Message);
    }

    [Fact]
    public async Task DeleteTicket_ShouldReturnFailure_WhenTicketDoesNotExist()
    {
        // Given
        var service = CreateService();

        // When
        var result = await service.DeleteTicketAsync(999);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal("Ticket not found.", result.Message);
    }

    [Fact]
    public async Task DeleteTicket_ShouldReturnFailure_WhenTicketIsCompleted()
    {
        // Given
        var service = CreateService();

        var dto = CreateValidCreateTicketDto();
        dto.Status = TicketStatus.Completed;

        var created = await service.AddTicketAsync(dto);
        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);

        // When
        var result = await service.DeleteTicketAsync(created.Data!.Id);

        // Then
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Completed tickets cannot be deleted.", result.Message);
    }

    [Fact]
    public async Task GetTicketById_ShouldReturnTicket_WhenIdExists()
    {
        // Given
        var service = CreateService();
        var dto = CreateValidCreateTicketDto();

        var created = await service.AddTicketAsync(dto);
        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);

        // When
        var ticket = await service.GetTicketByIdAsync(created.Data!.Id);

        // Then
        Assert.NotNull(ticket);
        Assert.Equal(dto.Title, ticket.Title);
    }

    [Fact]
    public async Task GetTicketById_ShouldReturnNull_WhenIdDoesNotExist()
    {
        // Given
        var service = CreateService();

        // When
        var ticket = await service.GetTicketByIdAsync(999);

        // Then
        Assert.Null(ticket);
    }

    [Fact]
    public async Task GetTickets_ShouldReturnOnlyOpenTickets_WhenStatusIsOpen()
    {
        // Given
        var service = CreateService();

        var dto1 = CreateValidCreateTicketDto();
        dto1.Title = "Ticket 1";
        dto1.Status = TicketStatus.Open;
        var createdOpen1 = await service.AddTicketAsync(dto1);
        Assert.True(createdOpen1.IsSuccess);
        Assert.NotNull(createdOpen1.Data);

        var dto2 = CreateValidCreateTicketDto();
        dto2.Title = "Ticket 2";
        dto2.Status = TicketStatus.Open;
        var createdOpen2 = await service.AddTicketAsync(dto2);
        Assert.True(createdOpen2.IsSuccess);
        Assert.NotNull(createdOpen2.Data);

        var dto3 = CreateValidCreateTicketDto();
        dto3.Title = "Ticket 3";
        dto3.Status = TicketStatus.Closed;
        var createdClosed = await service.AddTicketAsync(dto3);
        Assert.True(createdClosed.IsSuccess);
        Assert.NotNull(createdClosed.Data);

        // When
        var tickets = await service.GetTicketsAsync(status: TicketStatus.Open);

        // Then
        Assert.NotEmpty(tickets);
        Assert.All(tickets, ticket => Assert.Equal(TicketStatus.Open, ticket.Status));
    }

    [Fact]
    public async Task UpdateTicket_ShouldReturnSuccess_WhenIdExistsAndDataIsValid()
    {
        // Given
        var service = CreateService();
        var created = await service.AddTicketAsync(CreateValidCreateTicketDto());
        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);

        var dto = CreateValidUpdateTicketDto();

        // When
        var result = await service.UpdateTicketAsync(created.Data.Id, dto);
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
    public async Task UpdateTicket_ShouldSetCompletedAt_WhenStatusIsCompleted()
    {
        // Given
        var service = CreateService();
        var created = await service.AddTicketAsync(CreateValidCreateTicketDto());
        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);

        var dto = CreateValidUpdateTicketDto();
        dto.Status = TicketStatus.Completed;

        // When
        var result = await service.UpdateTicketAsync(created.Data.Id, dto);
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
    public async Task UpdateTicket_ShouldReturnFailure_WhenTicketDoesNotExist()
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
        var result = await service.UpdateTicketAsync(999, dto);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal("Ticket not found.", result.Message);
    }

    [Fact]
    public async Task UpdateTicket_ShouldReturnFailure_WhenTitleIsEmpty()
    {
        // Given
        var service = CreateService();
        var created = await service.AddTicketAsync(CreateValidCreateTicketDto());
        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);

        var dto = CreateValidUpdateTicketDto();
        dto.Title = "";

        // When
        var result = await service.UpdateTicketAsync(created.Data.Id, dto);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal("Title is required.", result.Message);
    }

    [Fact]
    public async Task UpdateTicket_ShouldReturnFailure_WhenDescriptionIsEmpty()
    {
        // Given
        var service = CreateService();
        var created = await service.AddTicketAsync(CreateValidCreateTicketDto());
        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Data);

        var dto = CreateValidUpdateTicketDto();
        dto.Description = "";

        // When
        var result = await service.UpdateTicketAsync(created.Data.Id, dto);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal("Description is required.", result.Message);
    }

    [Fact]
    public async Task GetTicketById_ShouldReturnCorrectTicket_WhenTicketExists()
    {
        // Given
        var expectedTicket = new Ticket { Id = 2, Title = "test 2" };
        var repositoryMock = new Mock<ITicketRepository>();
        repositoryMock
            .Setup(repository => repository.GetByIdAsync(2))
            .ReturnsAsync(expectedTicket);

        var service = new TicketService(repositoryMock.Object);

        // When
        var ticket = await service.GetTicketByIdAsync(2);

        // Then
        Assert.NotNull(ticket);
        Assert.Equal(2, ticket.Id);
        Assert.Equal("test 2", ticket.Title);
    }

    [Fact]
    public async Task DeleteTicket_ShouldCallRepositoryDelete_WhenTicketExists()
    {
        // Given
        var expectedTicket = new Ticket { Id = 1, Title = "Test 1" };
        var repositoryMock = new Mock<ITicketRepository>();

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(expectedTicket);
        var service = new TicketService(repositoryMock.Object);

        // When
        await service.DeleteTicketAsync(1);

        // Then
        repositoryMock.Verify(r => r.DeleteAsync(It.Is<Ticket>(t => t.Id == 1)), Times.Once);
    }

    [Fact]
    public async Task AddTicket_ShouldReturnFailure_WhenStatusIsInvalid()
    {
        // Given
        var repositoryMock = new Mock<ITicketRepository>();
        var service = CreateService(repositoryMock.Object);
        var dto = CreateValidCreateTicketDto();
        dto.Status = (TicketStatus)999;

        // When
        var result = await service.AddTicketAsync(dto);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Equal("Invalid ticket status.", result.Message);
        Assert.Null(result.Data);

        repositoryMock.Verify(r => r.AddAsync(It.IsAny<Ticket>()), Times.Never);
    }

    [Fact]
    public async Task AddTicket_ShouldReturnFailure_WhenPriorityIsInvalid()
    {
        // Given
        var repositoryMock = new Mock<ITicketRepository>();
        var service = CreateService(repositoryMock.Object);
        var dto = CreateValidCreateTicketDto();
        dto.Priority = (TicketPriority)555;

        // When
        var result = await service.AddTicketAsync(dto);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Equal("Invalid ticket priority.", result.Message);
        Assert.Null(result.Data);

        repositoryMock.Verify(r => r.AddAsync(It.IsAny<Ticket>()), Times.Never);
    }
}
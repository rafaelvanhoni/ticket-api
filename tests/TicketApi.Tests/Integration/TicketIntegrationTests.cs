using System.Data.Common;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

public class TicketIntegrationTests : IClassFixture<TicketApiFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true) }
    };

    public TicketIntegrationTests(TicketApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTickets_ShouldReturnOk()
    {
        // Given
        var response = await _client.GetAsync("/tickets");

        // When

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateTicket_ShouldReturnCreated()
    {
        // Given
        var request = new CreateTicketDto
        {
            Title = "Integration test ticket",
            Description = "Ticket created by integration test",
            Status = TicketStatus.Open,
            Priority = TicketPriority.High,
            AssignedTo = "Rafael"
        };

        // When
        var response = await _client.PostAsJsonAsync("/tickets", request);

        // Then
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateTicket_ShouldPersistTicket()
    {
        // Given
        var request = new CreateTicketDto
        {
            Title = "Persisted integration ticket",
            Description = "Ticket created and retrieved by integration test",
            Status = TicketStatus.Open,
            Priority = TicketPriority.High,
            AssignedTo = "Rafael"
        };

        // When
        var postResponse = await _client.PostAsJsonAsync("/tickets", request);
        var responseContent = await postResponse.Content.ReadFromJsonAsync<ApiResponse<Ticket>>(JsonOptions);

        Assert.NotNull(responseContent);
        Assert.NotNull(responseContent.Data);

        var ticketId = responseContent.Data.Id;

        var getByIdResponse = await _client.GetAsync($"/tickets/{ticketId}");
        var getByIdContent = await getByIdResponse.Content.ReadFromJsonAsync<ApiResponse<Ticket>>(JsonOptions);

        // Then
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, getByIdResponse.StatusCode);

        Assert.NotNull(getByIdContent);
        Assert.NotNull(getByIdContent.Data);
        Assert.Equal(ticketId, getByIdContent.Data.Id);
        Assert.Equal(request.Title, getByIdContent.Data.Title);

    }

    [Fact]
    public async Task CreateTicket_ShouldReturnBadRequest_WhenTitleIsEmpty()
    {
        // Given
        var request = new CreateTicketDto
        {
            Title = "",
            Description = "Title is empty"
        };

        // When
        var response = await _client.PostAsJsonAsync("/tickets", request);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<Ticket>>(JsonOptions);

        // Then
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
        Assert.Equal("Title is required.", content.Message);
    }

    [Fact]
    public async Task GetTicketById_ShouldReturnNotFound_WhenTicketDoesNotExist()
    {
        // Given
        var response = await _client.GetAsync("/tickets/999");
        // When

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<Ticket>>(JsonOptions);

        // Then
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal("Ticket not found.", content.Message);

    }

    [Fact]
    public async Task UpdateTicket_ShouldUpdateTicketSuccessfully()
    {
        // Given
        var createRequest = new CreateTicketDto
        {
            Title = "Integration test ticket",
            Description = "Ticket created by integration test",
            Status = TicketStatus.Open,
            Priority = TicketPriority.High,
            AssignedTo = "Rafael"
        };

        var updateRequest = new UpdateTicketDto
        {
            Title = "Title updated",
            Description = "Ticket updated by integration test",
            Status = TicketStatus.Open,
            Priority = TicketPriority.Low,
            AssignedTo = "Rafael"
        };

        // When
        var createResponse = await _client.PostAsJsonAsync("/tickets", createRequest);
        var createdContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<Ticket>>(JsonOptions);
        Assert.NotNull(createdContent);
        Assert.NotNull(createdContent.Data);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var updateResponse = await _client.PutAsJsonAsync($"/tickets/{createdContent.Data.Id}", updateRequest);
        var updatedContent = await updateResponse.Content.ReadFromJsonAsync<ApiResponse<Ticket>>(JsonOptions);

        // Then
        Assert.NotNull(updatedContent);
        Assert.NotNull(updatedContent.Data);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        Assert.Equal(updateRequest.Title, updatedContent.Data.Title);
        Assert.Equal(updateRequest.Description, updatedContent.Data.Description);
        Assert.Equal(updateRequest.Status, updatedContent.Data.Status);
        Assert.Equal(updateRequest.Priority, updatedContent.Data.Priority);
        Assert.Equal(updateRequest.AssignedTo, updatedContent.Data.AssignedTo);
        Assert.NotNull(updatedContent.Data.UpdatedAt);

    }

    [Fact]
    public async Task DeleteTicket_ShouldDeleteTicketSuccessfully()
    {
        // Given
        var createRequest = new CreateTicketDto
        {
            Title = "Ticket to be deleted",
            Description = "Ticket created for delete integration test"
        };

        // When
        var createResponse = await _client.PostAsJsonAsync("/tickets", createRequest);
        var createdContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<Ticket>>(JsonOptions);
        Assert.NotNull(createdContent);
        Assert.NotNull(createdContent.Data);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var ticketId = createdContent.Data.Id;

        var deleteResponse = await _client.DeleteAsync($"/tickets/{ticketId}");
        var deletedcontent = await deleteResponse.Content.ReadFromJsonAsync<ApiResponse<Ticket>>(JsonOptions);

        // Then
        Assert.NotNull(deletedcontent);
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var response = await _client.GetAsync($"/tickets/{ticketId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
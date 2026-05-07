public class TicketService
{

    private readonly ITicketRepository _repository;

    public TicketService(ITicketRepository repository)
    {
        _repository = repository;
    }

    private Task<IEnumerable<Ticket>> GetBaseTicketsAsync()
    {
        return _repository.GetAllTicketsAsync();
    }

    public async Task<OperationResult<Ticket>> GetTicketByIdAsync(int id)
    {
        var ticket = await _repository.GetByIdAsync(id);

        return new OperationResult<Ticket>
        {
            Status = ticket is null ? ResultStatus.NotFound : ResultStatus.Success,
            Message = ticket is null ? "Ticket not found." : null,
            Data = ticket
        };
    }

    public async Task<OperationResult<IEnumerable<Ticket>>> GetTicketsAsync(TicketStatus? status = null, TicketPriority? priority = null)
    {
        var allTickets = await GetBaseTicketsAsync();
        var tickets = Enumerable.Empty<Ticket>();

        if (status is null && priority is null)
        {
            tickets = allTickets.OrderBy(ticket => ticket.Id);
        }
        else
        {
            tickets = allTickets
                        .Where(ticket => (status is null || status == ticket.Status) &&
                                        (priority is null || priority == ticket.Priority))
                        .OrderBy(ticket => ticket.Id);
        }

        return new OperationResult<IEnumerable<Ticket>>
        {
            Data = tickets
        };
    }

    public async Task<OperationResult<Ticket>> AddTicketAsync(ITicketValidatable dto)
    {

        var validation = ValidateTicket(dto);
        if (!validation.IsSuccess)
            return validation;

        var ticket = new Ticket()
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Priority = dto.Priority,
            AssignedTo = dto.AssignedTo
        };

        validation.Data = ticket;

        await _repository.AddAsync(ticket);

        return validation;
    }

    public async Task<OperationResult<Ticket>> DeleteTicketAsync(int id)
    {
        var result = await GetTicketByIdAsync(id);
        if (!result.IsSuccess)
            return result;

        var ticket = result.Data!;

        if (ticket.Status == TicketStatus.Completed)
        {
            return new OperationResult<Ticket>()
            {
                Status = ResultStatus.BusinessError,
                Message = "Completed tickets cannot be deleted.",
                Data = ticket,
            };
        }

        var isDeleted = await _repository.DeleteAsync(ticket);

        return new OperationResult<Ticket>()
        {
            Status = isDeleted ? ResultStatus.Success : ResultStatus.ValidationError,
            Message = isDeleted ? null : "Ticket could not be deleted.",
            Data = ticket
        };

    }

    public async Task<OperationResult<Ticket>> UpdateTicketAsync(int id, ITicketValidatable dto)
    {

        var validation = ValidateTicket(dto);
        if (!validation.IsSuccess)
            return validation;

        var result = await GetTicketByIdAsync(id);
        if (!result.IsSuccess)
            return result;

        var ticket = result.Data!;

        ticket.Title = dto.Title;
        ticket.Description = dto.Description;
        ticket.Priority = dto.Priority;
        ticket.AssignedTo = dto.AssignedTo;
        ticket.UpdatedAt = DateTime.Now;

        ticket.UpdateStatus(dto.Status);

        validation.Data = ticket;
        await _repository.UpdateAsync(ticket);

        return validation;
    }

    private OperationResult<Ticket> ValidateTicket(ITicketValidatable dto)
    {
        var result = new OperationResult<Ticket>();

        if (!Enum.IsDefined(typeof(TicketStatus), dto.Status))
        {
            result.Status = ResultStatus.ValidationError;
            result.Message = "Invalid ticket status.";
            return result;
        }

        if (!Enum.IsDefined(typeof(TicketPriority), dto.Priority))
        {
            result.Status = ResultStatus.ValidationError;
            result.Message = "Invalid ticket priority.";
            return result;
        }

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            result.Status = ResultStatus.ValidationError;
            result.Message = "Title is required.";
            return result;
        }

        if (string.IsNullOrWhiteSpace(dto.Description))
        {
            result.Status = ResultStatus.ValidationError;
            result.Message = "Description is required.";
            return result;
        }

        return result;
    }

}


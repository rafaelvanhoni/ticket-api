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

    public Task<Ticket?> GetTicketByIdAsync(int id)
    {
        return _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Ticket>> GetTicketsAsync(TicketStatus? status = null, TicketPriority? priority = null)
    {
        var tickets = await GetBaseTicketsAsync();

        if (status is null && priority is null)
            return tickets.OrderBy(ticket => ticket.Id);

        return tickets
            .Where(ticket => (status is null || status == ticket.Status) &&
                             (priority is null || priority == ticket.Priority))
            .OrderBy(ticket => ticket.Id);

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
        var ticket = await GetTicketByIdAsync(id);

        if (ticket is null)
        {
            return new OperationResult<Ticket>()
            {
                Status = ResultStatus.NotFound,
                Message = "Ticket not found.",
            };
        }

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

        var ticket = await GetTicketByIdAsync(id);

        if (ticket is null)
        {
            return new OperationResult<Ticket>()
            {
                Status = ResultStatus.NotFound,
                Message = "Ticket not found."
            };
        }

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


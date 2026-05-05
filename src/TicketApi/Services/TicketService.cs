
public class TicketService
{

    private readonly ITicketRepository _repository;

    public TicketService(ITicketRepository repository)
    {
        _repository = repository;
    }

    private async Task<IEnumerable<Ticket>> GetBaseTicketsAsync()
    {
        return await _repository.GetAllTicketsAsync();
    }

    public async Task<Ticket?> GetTicketByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Ticket>> GetTicketsAsync(TicketStatus? status = null, TicketPriority? priority = null)
    {
        var tickets = Task.FromResult(GetBaseTicketsAsync());

        if (status is null && priority is null)
            return tickets.OrderBy(ticket => ticket.Id);

        return await tickets
            .Where(ticket => (status is null || status == ticket.Status) &&
                             (priority is null || priority == ticket.Priority))
            .OrderBy(ticket => ticket.Id);

    }

    public OperationResult<Ticket> AddTicket(ITicketValidatable dto)
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

        _repository.Add(ticket);

        return validation;
    }

    public OperationResult<Ticket> DeleteTicket(int id)
    {
        var ticket = GetTicketById(id);

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

        var isDeleted = _repository.Delete(ticket);

        return new OperationResult<Ticket>()
        {
            Status = isDeleted ? ResultStatus.Success : ResultStatus.ValidationError,
            Message = isDeleted ? null : "Ticket could not be deleted.",
            Data = ticket
        };

    }

    public OperationResult<Ticket> UpdateTicket(int id, ITicketValidatable dto)
    {

        var validation = ValidateTicket(dto);
        if (!validation.IsSuccess)
            return validation;

        var ticket = GetTicketById(id);

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
        _repository.Update(ticket);

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


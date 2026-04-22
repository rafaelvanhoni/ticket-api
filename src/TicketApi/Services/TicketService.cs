
public class TicketService
{

    private readonly ITicketRepository _repository;

    public TicketService(ITicketRepository repository)
    {
        _repository = repository;
    }

    private IEnumerable<Ticket> GetBaseTickets()
    {
        return _repository.GetAllTickets();
    }

    public Ticket? GetTicketById(int id)
    {
        var tickets = GetBaseTickets();
        return tickets.FirstOrDefault(ticket => ticket.Id == id);
    }

    public IEnumerable<Ticket> GetTickets(TicketStatus? status = null, TicketPriority? priority = null)
    {
        var tickets = GetBaseTickets();

        if (status is null && priority is null)
            return tickets.OrderBy(ticket => ticket.Id);

        return tickets
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
                IsSuccess = false,
                Message = "Ticket not found."
            };
        }

        if (ticket.Status == TicketStatus.Completed)
        {

            return new OperationResult<Ticket>()
            {
                IsSuccess = false,
                Message = "Completed tickets cannot be deleted.",
                Data = ticket
            };

        }

        var isDeleted = _repository.Delete(ticket);

        return new OperationResult<Ticket>()
        {
            IsSuccess = isDeleted,
            Message = isDeleted ? "" : "Ticket could not be deleted.",
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
                IsSuccess = false,
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
        return validation;
    }

    public OperationResult<Ticket> ValidateTicket(ITicketValidatable dto)
    {
        var result = new OperationResult<Ticket>();

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            result.IsSuccess = false;
            result.Message = "Title is required.";
            return result;
        }

        if (string.IsNullOrWhiteSpace(dto.Description))
        {
            result.IsSuccess = false;
            result.Message = "Description is required.";
            return result;
        }

        return result;
    }

}


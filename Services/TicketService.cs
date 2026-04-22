
public class TicketService
{

    private readonly ITicketRepository _repository;

    public TicketService(ITicketRepository repository)
    {
        _repository = repository;
    }

    private IEnumerable<Ticket> ObterBaseTickets()
    {
        return _repository.ObterBaseTickets();
    }

    public Ticket? ObterTicketPorId(int id)
    {
        var tickets = ObterBaseTickets();
        return tickets.FirstOrDefault(ticket => ticket.Id == id);
    }

    public IEnumerable<Ticket> ObterTickets(TicketStatus? status = null, TicketPriority? priority = null)
    {
        var tickets = ObterBaseTickets();

        if (status is null && priority is null)
            return tickets.OrderBy(ticket => ticket.Id);

        return tickets
            .Where(ticket => (status is null || status == ticket.Status) &&
                             (priority is null || priority == ticket.Priority))
            .OrderBy(ticket => ticket.Id);

    }

    public int ContarEmAberto()
    {
        var tickets = ObterBaseTickets();
        return tickets.Count(ticket => ticket.Status == TicketStatus.Open);
    }

    public bool ExistePrioridadeAlta()
    {
        var tickets = ObterBaseTickets();
        return tickets.Any(ticket => ticket.Priority == TicketPriority.High);
    }

    public IEnumerable<Ticket> ObterOrdenadoPorDescricao()
    {
        var tickets = ObterBaseTickets();
        return tickets.OrderBy(ticket => ticket.Description);
    }

    public IEnumerable<Ticket> ObterOrdenadoPorDescricaoDecrescente()
    {
        var tickets = ObterBaseTickets();
        return tickets.OrderByDescending(ticket => ticket.Description);
    }

    public IEnumerable<TicketResumoDto> ObterTicketsResumidos()
    {
        var tickets = ObterBaseTickets();
        return tickets.Select(ticket => new TicketResumoDto(ticket.Id, ticket.Title, ticket.Status));
    }

    public IEnumerable<TicketResumoDto> ObterTitulosResumidosEmAberto()
    {
        var tickets = ObterBaseTickets();
        return tickets.Where(ticket => ticket.Status == TicketStatus.Open)
            .OrderBy(ticket => ticket.Title)
            .Select(ticket => new TicketResumoDto(ticket.Id, ticket.Title, ticket.Status));
    }

    public IEnumerable<TicketResumoDto> ObterTitulosResumidosOrdenados()
    {
        var tickets = ObterBaseTickets();
        return tickets.OrderBy(ticket => ticket.Status)
            .ThenBy(ticket => ticket.Title)
            .Select(ticket => new TicketResumoDto(ticket.Id, ticket.Title, ticket.Status));
    }

    public OperationResult<Ticket> AddTicket(ITicketValidatable dto)
    {

        var validation = ValidarCreateTicket(dto);
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
        var ticket = ObterTicketPorId(id);

        if (ticket is null)
        {
            return new OperationResult<Ticket>()
            {
                IsSuccess = false,
                Message = "Ticket not found."
            };
        }

        OperationResult<Ticket> resultado;

        if (ticket.Status == TicketStatus.Completed)
        {

            resultado = new OperationResult<Ticket>()
            {
                IsSuccess = false,
                Message = "Completed tickets cannot be deleted.",
                Data = ticket
            };

            return resultado;
        }

        var isDeleted = _repository.Delete(ticket);

        resultado = new OperationResult<Ticket>()
        {
            IsSuccess = isDeleted,
            Message = isDeleted ? "" : "Ticket could not be deleted.",
            Data = ticket
        };

        return resultado;
    }

    public OperationResult<Ticket> AtualizarTicket(int id, ITicketValidatable dto)
    {

        var validation = ValidarCreateTicket(dto);
        if (!validation.IsSuccess)
            return validation;

        var ticket = ObterTicketPorId(id);

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

        ticket.AtualizarStatus(dto.Status);

        validation.Data = ticket;
        return validation;
    }

    public OperationResult<Ticket> ValidarCreateTicket(ITicketValidatable dto)
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



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

    public Ticket? ObterTicketPorId(int codigo)
    {
        var tickets = ObterBaseTickets();
        return tickets.FirstOrDefault(ticket => ticket.Id == codigo);
    }

    public IEnumerable<Ticket> ObterTickets(TicketStatus? status = null, TicketPriority? prioridade = null)
    {
        var tickets = ObterBaseTickets();

        if (status is null && prioridade is null)
            return tickets.OrderBy(ticket => ticket.Id);

        return tickets
            .Where(ticket => (status is null || status == ticket.Status) &&
                             (prioridade is null || prioridade == ticket.Priority))
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

    public Ticket AdicionarTicket(CreateTicketDto dto)
    {

        var ticket = new Ticket()
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Priority = dto.Priority,
            AssignedTo = dto.AssignedTo
        };

        _repository.AdicionarTicket(ticket);

        return ticket;
    }
}


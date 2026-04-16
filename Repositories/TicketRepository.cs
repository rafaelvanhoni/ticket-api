public class TicketRepository : ITicketRepository
{
    private readonly List<Ticket> _tickets;
    private int _ultimoId = 1;

    public TicketRepository()
    {
        _tickets = new List<Ticket>();

        AdicionarTicket(new Ticket
        {
            Title = "Primeiro chamado",
            Description = "Primeiro chamado criado para teste",
            Status = TicketStatus.Open,
            AssignedTo = "João",
        });

        AdicionarTicket(new Ticket
        {
            Title = "Chamado MLA0301",
            Description = "Problema no MLA que está travando na aprovação",
            Status = TicketStatus.Closed,
            Priority = TicketPriority.High,
            AssignedTo = "José",
        });
        AdicionarTicket(new Ticket
        {
            Title = "Zerar valores",
            Description = "Zerar valores de um registro",
            Status = TicketStatus.Open,
            Priority = TicketPriority.Low,
            AssignedTo = "Rafael",
        });

        AdicionarTicket(new Ticket
        {
            Title = "Sistema ABC",
            Description = "Subir sistema ABC para produção",
            Status = TicketStatus.Closed,
            Priority = TicketPriority.High,
            AssignedTo = "Rafael",
        });
    }

    public void AdicionarTicket(Ticket ticket)
    {
        ticket.Id = _ultimoId++;
        ticket.CreatedAt = DateTime.Now;
        _tickets.Add(ticket);

    }

    public IEnumerable<Ticket> ObterBaseTickets()
    {
        return _tickets;
    }
}
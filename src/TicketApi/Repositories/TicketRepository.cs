public class TicketRepository : ITicketRepository
{
    private readonly List<Ticket> _tickets;
    private int _nextId = 1;

    public TicketRepository()
    {
        _tickets = new List<Ticket>();

        Add(new Ticket
        {
            Title = "Primeiro chamado",
            Description = "Primeiro chamado criado para teste",
            Status = TicketStatus.Open,
            AssignedTo = "João",
        });

        Add(new Ticket
        {
            Title = "Chamado MLA0301",
            Description = "Problema no MLA que está travando na aprovação",
            Status = TicketStatus.Closed,
            Priority = TicketPriority.High,
            AssignedTo = "José",
        });
        Add(new Ticket
        {
            Title = "Zerar valores",
            Description = "Zerar valores de um registro",
            Status = TicketStatus.Open,
            Priority = TicketPriority.Low,
            AssignedTo = "Rafael",
        });

        Add(new Ticket
        {
            Title = "Sistema ABC",
            Description = "Subir sistema ABC para produção",
            Status = TicketStatus.Closed,
            Priority = TicketPriority.High,
            AssignedTo = "Rafael",
        });
        Add(new Ticket
        {
            Title = "Melhorar o programa XYZ",
            Description = "Diversas melhorias no programa XYZ",
            Status = TicketStatus.Completed,
            Priority = TicketPriority.High,
            AssignedTo = "Joao"
        });
    }
    public IEnumerable<Ticket> GetAllTickets()
    {
        return _tickets;
    }

    public void Add(Ticket ticket)
    {
        ticket.Id = _nextId++;
        ticket.CreatedAt = DateTime.Now;
        _tickets.Add(ticket);

    }

    public bool Delete(Ticket ticket)
    {
        return _tickets.Remove(ticket);
    }

}
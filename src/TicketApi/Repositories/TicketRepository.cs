public class TicketRepository : ITicketRepository
{
    private readonly List<Ticket> _tickets;
    private int _nextId = 1;

    public TicketRepository()
    {
        _tickets = new List<Ticket>();

        AddAsync(new Ticket
        {
            Title = "Primeiro chamado",
            Description = "Primeiro chamado criado para teste",
            Status = TicketStatus.Open,
            AssignedTo = "João",
        });

        AddAsync(new Ticket
        {
            Title = "Chamado MLA0301",
            Description = "Problema no MLA que está travando na aprovação",
            Status = TicketStatus.Closed,
            Priority = TicketPriority.High,
            AssignedTo = "José",
        });
        AddAsync(new Ticket
        {
            Title = "Zerar valores",
            Description = "Zerar valores de um registro",
            Status = TicketStatus.Open,
            Priority = TicketPriority.Low,
            AssignedTo = "Rafael",
        });

        AddAsync(new Ticket
        {
            Title = "Sistema ABC",
            Description = "Subir sistema ABC para produção",
            Status = TicketStatus.Closed,
            Priority = TicketPriority.High,
            AssignedTo = "Rafael",
        });
        AddAsync(new Ticket
        {
            Title = "Melhorar o programa XYZ",
            Description = "Diversas melhorias no programa XYZ",
            Status = TicketStatus.Completed,
            Priority = TicketPriority.High,
            AssignedTo = "Joao"
        });
    }
    public Task<IEnumerable<Ticket>> GetAllTicketsAsync()
    {
        return Task.FromResult(_tickets.AsEnumerable());
    }

    public Task<Ticket?> GetByIdAsync(int id)
    {
        return Task.FromResult(_tickets.FirstOrDefault(ticket => ticket.Id == id));
    }

    public Task AddAsync(Ticket ticket)
    {
        ticket.Id = _nextId++;
        ticket.CreatedAt = DateTime.Now;
        _tickets.Add(ticket);
        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(Ticket ticket)
    {
        return Task.FromResult(_tickets.Remove(ticket));
    }

    public Task UpdateAsync(Ticket ticket)
    {
        return Task.CompletedTask;
    }
}
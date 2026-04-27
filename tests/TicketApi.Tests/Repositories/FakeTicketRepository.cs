
public class FakeTicketRepository : ITicketRepository
{

    private readonly List<Ticket> _tickets = new();
    private int _nextId = 1;

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

    public IEnumerable<Ticket> GetAllTickets()
    {
        return _tickets.ToList();
    }
}
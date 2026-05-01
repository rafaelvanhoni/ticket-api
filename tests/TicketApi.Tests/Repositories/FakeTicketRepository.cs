public class FakeTicketRepository : ITicketRepository
{

    private readonly List<Ticket> _tickets = new();
    private int _nextId = 1;

    public IEnumerable<Ticket> GetAllTickets() => _tickets.ToList();
    public Ticket? GetById(int id) => _tickets.FirstOrDefault(ticket => ticket.Id == id);

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

    public void Update(Ticket ticket)
    {
        //
    }
}
public class FakeTicketRepository : ITicketRepository
{

    private readonly List<Ticket> _tickets = new();
    private int _nextId = 1;

    public Task<IEnumerable<Ticket>> GetAllTicketsAsync()
    {
        return Task.FromResult(_tickets.AsEnumerable());
    }

    public Task<Ticket?> GetByIdAsync(int id) => Task.FromResult(_tickets.FirstOrDefault(ticket => ticket.Id == id));

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
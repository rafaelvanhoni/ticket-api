public class EfTicketRepository : ITicketRepository
{

    private readonly TicketDbContext _context;

    public EfTicketRepository(TicketDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Ticket> GetAllTickets() => _context.Tickets.ToList();

    public Ticket? GetById(int id) => _context.Tickets.FirstOrDefault(ticket => ticket.Id == id);

    public void Add(Ticket ticket)
    {
        ticket.CreatedAt = DateTime.Now;
        _context.Tickets.Add(ticket);
        _context.SaveChanges();
    }

    public bool Delete(Ticket ticket)
    {
        _context.Tickets.Remove(ticket);
        var result = _context.SaveChanges();
        return result > 0;
    }


    public void Update(Ticket ticket)
    {
        _context.Tickets.Update(ticket);
        _context.SaveChanges();
    }

}
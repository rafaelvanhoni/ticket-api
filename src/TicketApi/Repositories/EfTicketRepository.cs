using Microsoft.EntityFrameworkCore;

public class EfTicketRepository : ITicketRepository
{

    private readonly TicketDbContext _context;

    public EfTicketRepository(TicketDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
    {
        return await _context.Tickets.ToListAsync();
    }

    public async Task<Ticket?> GetByIdAsync(int id)
    {
        return await _context.Tickets.FirstOrDefaultAsync(ticket => ticket.Id == id);
    }

    public async Task AddAsync(Ticket ticket)
    {
        ticket.CreatedAt = DateTime.Now;
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Ticket ticket)
    {
        _context.Tickets.Remove(ticket);
        var result = await _context.SaveChangesAsync();
        return (result > 0);
    }

    public async Task UpdateAsync(Ticket ticket)
    {
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();
    }

}
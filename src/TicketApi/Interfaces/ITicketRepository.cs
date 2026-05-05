public interface ITicketRepository
{
    Task<IEnumerable<Ticket>> GetAllTicketsAsync();
    Task<Ticket?> GetByIdAsync(int id);
    Task AddAsync(Ticket ticket);
    Task<bool> DeleteAsync(Ticket ticket);
    Task UpdateAsync(Ticket ticket);
}

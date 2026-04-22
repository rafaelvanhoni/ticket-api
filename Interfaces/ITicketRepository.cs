public interface ITicketRepository
{
    IEnumerable<Ticket> GetAllTickets();
    void Add(Ticket ticket);
    bool Delete(Ticket ticket);
}

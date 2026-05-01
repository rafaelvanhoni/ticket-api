public interface ITicketRepository
{
    IEnumerable<Ticket> GetAllTickets();
    Ticket? GetById(int id);
    void Add(Ticket ticket);
    bool Delete(Ticket ticket);
    void Update(Ticket ticket);
}

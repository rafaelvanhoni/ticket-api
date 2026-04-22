public interface ITicketRepository
{
    IEnumerable<Ticket> ObterBaseTickets();
    void Add(Ticket ticket);
    bool Delete(Ticket ticket);
}

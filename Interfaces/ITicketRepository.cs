public interface ITicketRepository
{
    IEnumerable<Ticket> ObterBaseTickets();
    void AdicionarTicket(Ticket ticket);
}

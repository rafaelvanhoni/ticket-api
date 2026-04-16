public class TicketResumoDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public TicketStatus Status { get; set; }

    public TicketResumoDto(int id, string title, TicketStatus status)
    {
        this.Id = id;
        this.Title = title;
        this.Status = status;
    }
}
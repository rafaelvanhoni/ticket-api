public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public TicketPriority Priority { get; set; } = TicketPriority.Low;
    public string? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public bool UpdateStatus(TicketStatus status)
    {
        if (status == this.Status)
            return false;

        if (status == TicketStatus.Completed)
            this.CompletedAt = DateTime.Now;
        else
            this.CompletedAt = null;

        this.Status = status;
        return true;
    }

}
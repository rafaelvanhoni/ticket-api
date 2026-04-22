public class UpdateTicketDto : ITicketValidatable
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public TicketPriority Priority { get; set; } = TicketPriority.Low;
    public string? AssignedTo { get; set; }
}
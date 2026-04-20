public interface ITicketValidatable
{

    public string Title { get; set; }
    public string Description { get; set; }
    public TicketStatus Status { get; set; }
    public TicketPriority Priority { get; set; }
    public string? AssignedTo { get; set; }

}
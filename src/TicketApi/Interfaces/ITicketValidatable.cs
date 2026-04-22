public interface ITicketValidatable
{

    string Title { get; set; }
    string Description { get; set; }
    TicketStatus Status { get; set; }
    TicketPriority Priority { get; set; }
    string? AssignedTo { get; set; }

}
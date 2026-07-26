using System;

namespace AfroEvent.Models
{
    public class AgendaItemEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Foreign Key
        public Guid EventId { get; set; }
        public EventEntity? Event { get; set; }
    }
}

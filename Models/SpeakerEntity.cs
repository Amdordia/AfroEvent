using System;

namespace AfroEvent.Models
{
    public class SpeakerEntity
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;
        public string LinkedInUrl { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;

        // Foreign Key
        public Guid EventId { get; set; }
        public EventEntity? Event { get; set; }
    }
}

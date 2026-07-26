using System;
using System.Collections.Generic;

namespace AfroEvent.Models
{
    public class EventEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LocationAddress { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public decimal Price { get; set; }
        public int MaxCapacity { get; set; }
        public string CoverImageUrl { get; set; } = string.Empty;

        // Foreign keys
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public string OrganizerId { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<SpeakerEntity> Speakers { get; set; } = new List<SpeakerEntity>();
        public ICollection<AgendaItemEntity> AgendaItems { get; set; } = new List<AgendaItemEntity>();
    }
}

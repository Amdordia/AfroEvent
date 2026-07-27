using System;

namespace AfroEvent.Models
{
    public class TicketEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string QrCodeHash { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public bool IsPresent { get; set; }
        public DateTime? ScanDate { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        // Clé étrangère vers l'Événement
        public Guid EventId { get; set; }
        public EventEntity? Event { get; set; }

        // Clé étrangère vers le Participant (AppUser)
        public string ParticipantId { get; set; } = string.Empty;
    }
}

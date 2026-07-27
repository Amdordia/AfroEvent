using System;
using System.Collections.Generic;

namespace AfroEvent.ViewModels;

public class OrganizerDashboardViewModel
{
    public string OrganizerName { get; set; } = string.Empty;
    public decimal TotalRevenueFcfa { get; set; }
    public int TotalEvents { get; set; }
    public int TotalRegistrations { get; set; }
    public int TotalCheckIns { get; set; }
    public double AveragePresenceRate => TotalRegistrations > 0
        ? Math.Round((double)TotalCheckIns / TotalRegistrations * 100, 1)
        : 0;

    public List<OrganizerEventSummaryViewModel> RecentEvents { get; set; } = new();

    // Propriétés pour les graphiques dynamiques
    public List<string> CategoryLabels { get; set; } = new();
    public List<int> CategoryData { get; set; } = new();
    
    public List<string> EventLabels { get; set; } = new();
    public List<int> EventRegistrationData { get; set; } = new();
    public List<decimal> EventRevenueData { get; set; } = new();
}

public class OrganizerEventSummaryViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public string LocationAddress { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public int RegisteredCount { get; set; }
    public int CheckedInCount { get; set; }
    public decimal TicketPrice { get; set; }
    public decimal Revenue => RegisteredCount * TicketPrice;
    public string Status { get; set; } = "Publié"; // Publié, Brouillon, Terminé, Annulé
}

public class AttendeeViewModel
{
    public Guid TicketId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public bool IsPaid { get; set; }
    public bool IsPresent { get; set; }
    public DateTime? ScanDate { get; set; }
    public string QrCodeHash { get; set; } = string.Empty;
}

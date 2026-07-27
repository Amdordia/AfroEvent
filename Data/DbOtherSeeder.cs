using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AfroEvent.Models;

namespace AfroEvent.Data
{
    public static class DbOtherSeeder
    {
        public static async Task SeedOtherDataAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var context = serviceProvider.GetRequiredService<AfroEventDbContext>();


            var organizers = new List<AppUser>();
            for (int i = 1; i <= 5; i++)
            {
                var email = $"organisateur{i}@afroevent.com";
                var user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = $"Organisateur{i}",
                    LastName = $"Nom{i}",
                    OrganizationName = $"AfroOrg {i}",
                    OrganizerStatus = OrganizerStatus.Approuve,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, "Organizer@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Organisateur");
                }
                organizers.Add(user);
            }


            var participants = new List<AppUser>();
            for (int i = 1; i <= 5; i++)
            {
                var email = $"participant{i}@afroevent.com";
                var user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = $"Participant{i}",
                    LastName = $"Nom{i}",
                    OrganizerStatus = OrganizerStatus.NonApplicable,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, "Participant@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Participant");
                }
                participants.Add(user);
            }


            var eventsList = new List<EventEntity>
            {
                new EventEntity
                {
                    Title = "Innov'Afro Hackathon",
                    Description = "Un grand hackathon de 48 heures pour stimuler l'innovation en Afrique.",
                    StartDate = DateTime.UtcNow.AddDays(10),
                    EndDate = DateTime.UtcNow.AddDays(12),
                    LocationAddress = "Bamako, Mali",
                    Latitude = 12.6392,
                    Longitude = -8.0029,
                    Price = 0.00m,
                    MaxCapacity = 100,
                    CoverImageUrl = "https://images.unsplash.com/photo-1631350397792-8e0c2de5b637?q=80&w=870&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                    CategoryId = 1, // Hackathon
                    OrganizerId = organizers[0].Id
                },
                new EventEntity
                {
                    Title = "Vite & React Bootcamp",
                    Description = "Formation intensive et pratique sur le développement frontend moderne.",
                    StartDate = DateTime.UtcNow.AddDays(20),
                    EndDate = DateTime.UtcNow.AddDays(25),
                    LocationAddress = "Abidjan, Côte d'Ivoire",
                    Latitude = 5.3600,
                    Longitude = -4.0083,
                    Price = 15000.00m,
                    MaxCapacity = 50,
                    CoverImageUrl = "https://images.unsplash.com/photo-1528901166007-3784c7dd3653?q=80&w=870&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                    CategoryId = 2, // Bootcamp
                    OrganizerId = organizers[0].Id
                },
                new EventEntity
                {
                    Title = "Conférence FinTech Afrique",
                    Description = "Échanges passionnants sur le futur de la finance mobile et du Web3.",
                    StartDate = DateTime.UtcNow.AddDays(30),
                    EndDate = DateTime.UtcNow.AddDays(31),
                    LocationAddress = "Dakar, Sénégal",
                    Latitude = 14.7167,
                    Longitude = -17.4677,
                    Price = 25000.00m,
                    MaxCapacity = 200,
                    CoverImageUrl = "https://plus.unsplash.com/premium_photo-1679547202671-f9dbbf466db4?q=80&w=1032&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                    CategoryId = 3, // Conférence
                    OrganizerId = organizers[0].Id
                },
                new EventEntity
                {
                    Title = "Atelier Design Thinking",
                    Description = "Apprenez à concevoir des produits centrés sur l'expérience utilisateur.",
                    StartDate = DateTime.UtcNow.AddDays(40),
                    EndDate = DateTime.UtcNow.AddDays(40).AddHours(6),
                    LocationAddress = "Conakry, Guinée",
                    Latitude = 9.5370,
                    Longitude = -13.6773,
                    Price = 5000.00m,
                    MaxCapacity = 30,
                    CoverImageUrl = "https://plus.unsplash.com/premium_photo-1661713210744-f5be3c3491fe?q=80&w=870&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                    CategoryId = 4, // Workshop
                    OrganizerId = organizers[1].Id
                },
                new EventEntity
                {
                    Title = "AfroBeats Festival",
                    Description = "Le plus grand concert festif de musique urbaine de l'année.",
                    StartDate = DateTime.UtcNow.AddDays(50).AddHours(18),
                    EndDate = DateTime.UtcNow.AddDays(50).AddHours(23),
                    LocationAddress = "Lomé, Togo",
                    Latitude = 6.1375,
                    Longitude = 1.2125,
                    Price = 10000.00m,
                    MaxCapacity = 500,
                    CoverImageUrl = "https://images.unsplash.com/photo-1470229722913-7c0e2dbbafd3?q=80&w=870&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                    CategoryId = 5, // Concert
                    OrganizerId = organizers[1].Id
                }
            };

            context.Events.AddRange(eventsList);
            await context.SaveChangesAsync();


            var speakersList = new List<SpeakerEntity>
            {
                new SpeakerEntity
                {
                    FullName = "Dr. Aminata Diallo",
                    Biography = "Experte en Intelligence Artificielle et entrepreneure chevronnée.",
                    LinkedInUrl = "https://linkedin.com/in/aminatadiallo",
                    ProfileImageUrl = "/images/speakers/speaker1.jpg",
                    EventId = eventsList[0].Id
                },
                new SpeakerEntity
                {
                    FullName = "Jean-Pierre Kassi",
                    Biography = "Développeur Senior & formateur passionné par les technologies du Web.",
                    LinkedInUrl = "https://linkedin.com/in/jpkassi",
                    ProfileImageUrl = "/images/speakers/speaker2.jpg",
                    EventId = eventsList[1].Id
                },
                new SpeakerEntity
                {
                    FullName = "Seydou Keita",
                    Biography = "Directeur de l'innovation dans une grande banque mobile panafricaine.",
                    LinkedInUrl = "https://linkedin.com/in/seydoukeita",
                    ProfileImageUrl = "/images/speakers/speaker3.jpg",
                    EventId = eventsList[2].Id
                },
                new SpeakerEntity
                {
                    FullName = "Fatoumata Coulibaly",
                    Biography = "Product Designer UX/UI engagée pour le design inclusif.",
                    LinkedInUrl = "https://linkedin.com/in/fatoumata",
                    ProfileImageUrl = "/images/speakers/speaker4.jpg",
                    EventId = eventsList[3].Id
                },
                new SpeakerEntity
                {
                    FullName = "Master Soumy",
                    Biography = "Artiste engagé et producteur musical renommé.",
                    LinkedInUrl = "https://linkedin.com/in/mastersoumy",
                    ProfileImageUrl = "/images/speakers/speaker5.jpg",
                    EventId = eventsList[4].Id
                }
            };

            context.Speakers.AddRange(speakersList);


            var ticketsList = new List<TicketEntity>
            {
                new TicketEntity
                {
                    QrCodeHash = Guid.NewGuid().ToString(),
                    IsPaid = true,
                    IsPresent = true,
                    ScanDate = DateTime.UtcNow.AddHours(-1),
                    PurchaseDate = DateTime.UtcNow.AddDays(-2),
                    EventId = eventsList[0].Id,
                    ParticipantId = participants[0].Id
                },
                new TicketEntity
                {
                    QrCodeHash = Guid.NewGuid().ToString(),
                    IsPaid = true,
                    IsPresent = true,
                    ScanDate = DateTime.UtcNow.AddHours(-2),
                    PurchaseDate = DateTime.UtcNow.AddDays(-1),
                    EventId = eventsList[0].Id,
                    ParticipantId = participants[1].Id
                },
                new TicketEntity
                {
                    QrCodeHash = Guid.NewGuid().ToString(),
                    IsPaid = true,
                    IsPresent = false,
                    PurchaseDate = DateTime.UtcNow.AddDays(-3),
                    EventId = eventsList[1].Id,
                    ParticipantId = participants[2].Id
                },
                new TicketEntity
                {
                    QrCodeHash = Guid.NewGuid().ToString(),
                    IsPaid = true,
                    IsPresent = true,
                    ScanDate = DateTime.UtcNow.AddMinutes(-30),
                    PurchaseDate = DateTime.UtcNow.AddDays(-1),
                    EventId = eventsList[2].Id,
                    ParticipantId = participants[3].Id
                },
                new TicketEntity
                {
                    QrCodeHash = Guid.NewGuid().ToString(),
                    IsPaid = true,
                    IsPresent = false,
                    PurchaseDate = DateTime.UtcNow.AddDays(-2),
                    EventId = eventsList[3].Id,
                    ParticipantId = participants[4].Id
                }
            };

            context.Tickets.AddRange(ticketsList);
            await context.SaveChangesAsync();
        }
    }
}

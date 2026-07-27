using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using AfroEvent.Data;
using AfroEvent.Hubs;
using AfroEvent.Models;
using AfroEvent.Services.Interfaces;
using AfroEvent.ViewModels;

namespace AfroEvent.Services.Implementations
{
    /// <summary>
    /// Service métier pour la gestion complète du cycle de vie des événements.
    /// Source unique de vérité : la base de données EF Core.
    /// </summary>
    public class EventService : IEventService
    {
        private readonly AfroEventDbContext _context;
        private readonly IHubContext<EventHub> _hubContext;

        private const string DefaultCoverImage =
            "https://images.unsplash.com/photo-1515187029135-18ee286d815b?auto=format&fit=crop&w=1200&q=80";
        private const string DefaultSpeakerImage =
            "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=300&q=80";

        public EventService(AfroEventDbContext context, IHubContext<EventHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public List<EventFormViewModel> GetAllEvents()
        {
            return _context.Events
                .Include(e => e.Category)
                .Include(e => e.Speakers)
                .Include(e => e.AgendaItems)
                .OrderBy(e => e.StartDate)
                .AsEnumerable()
                .Select(MapToViewModel)
                .ToList();
        }

        public List<EventFormViewModel> GetUpcomingEvents(int count = 6)
        {
            return _context.Events
                .Include(e => e.Category)
                .Where(e => e.StartDate >= DateTime.Now)
                .OrderBy(e => e.StartDate)
                .Take(count)
                .AsEnumerable()
                .Select(MapToViewModel)
                .ToList();
        }

        public EventFormViewModel? GetEventById(Guid id)
        {
            var entity = _context.Events
                .Include(e => e.Category)
                .Include(e => e.Speakers)
                .Include(e => e.AgendaItems)
                .FirstOrDefault(e => e.Id == id);

            return entity == null ? null : MapToViewModel(entity);
        }

        public void CreateEvent(EventFormViewModel model, string organizerId)
        {
            var entity = new EventEntity
            {
                Id              = model.Id == Guid.Empty ? Guid.NewGuid() : model.Id,
                Title           = model.Title,
                Description     = model.Description ?? string.Empty,
                StartDate       = model.StartDate,
                EndDate         = model.EndDate,
                LocationAddress = model.LocationAddress ?? string.Empty,
                Latitude        = model.Latitude,
                Longitude       = model.Longitude,
                Price           = model.Price,
                MaxCapacity     = model.MaxCapacity,
                CoverImageUrl   = string.IsNullOrWhiteSpace(model.CoverImageUrl)
                                    ? DefaultCoverImage
                                    : model.CoverImageUrl,
                CategoryId      = model.CategoryId,
                OrganizerId     = organizerId
            };

            if (model.AgendaItems != null)
            {
                foreach (var item in model.AgendaItems)
                {
                    entity.AgendaItems.Add(new AgendaItemEntity
                    {
                        Title     = item.Title,
                        StartTime = item.StartTime,
                        EndTime   = item.EndTime
                    });
                }
            }

            if (model.Speakers != null)
            {
                foreach (var item in model.Speakers)
                {
                    entity.Speakers.Add(new SpeakerEntity
                    {
                        FullName        = item.FullName,
                        Biography       = item.Biography ?? string.Empty,
                        LinkedInUrl     = item.LinkedInUrl ?? string.Empty,
                        ProfileImageUrl = string.IsNullOrWhiteSpace(item.ProfileImageUrl)
                                            ? DefaultSpeakerImage
                                            : item.ProfileImageUrl
                    });
                }
            }

            _context.Events.Add(entity);
            _context.SaveChanges();
        }

        public void UpdateEvent(Guid id, EventFormViewModel model)
        {
            var entity = _context.Events
                .Include(e => e.Speakers)
                .Include(e => e.AgendaItems)
                .FirstOrDefault(e => e.Id == id);

            if (entity == null) return;

            entity.Title           = model.Title;
            entity.Description     = model.Description ?? string.Empty;
            entity.StartDate       = model.StartDate;
            entity.EndDate         = model.EndDate;
            entity.LocationAddress = model.LocationAddress ?? string.Empty;
            entity.Latitude        = model.Latitude;
            entity.Longitude       = model.Longitude;
            entity.Price           = model.Price;
            entity.MaxCapacity     = model.MaxCapacity;
            entity.CategoryId      = model.CategoryId;

            if (!string.IsNullOrWhiteSpace(model.CoverImageUrl))
                entity.CoverImageUrl = model.CoverImageUrl;

            // Remplacer les sous-entités
            _context.Speakers.RemoveRange(entity.Speakers);
            _context.AgendaItems.RemoveRange(entity.AgendaItems);

            if (model.AgendaItems != null)
            {
                foreach (var item in model.AgendaItems)
                {
                    entity.AgendaItems.Add(new AgendaItemEntity
                    {
                        Title     = item.Title,
                        StartTime = item.StartTime,
                        EndTime   = item.EndTime
                    });
                }
            }

            if (model.Speakers != null)
            {
                foreach (var item in model.Speakers)
                {
                    entity.Speakers.Add(new SpeakerEntity
                    {
                        FullName        = item.FullName,
                        Biography       = item.Biography ?? string.Empty,
                        LinkedInUrl     = item.LinkedInUrl ?? string.Empty,
                        ProfileImageUrl = string.IsNullOrWhiteSpace(item.ProfileImageUrl)
                                            ? DefaultSpeakerImage
                                            : item.ProfileImageUrl
                    });
                }
            }

            _context.SaveChanges();
        }

        public void DeleteEvent(Guid id)
        {
            var entity = _context.Events
                .Include(e => e.Speakers)
                .Include(e => e.AgendaItems)
                .FirstOrDefault(e => e.Id == id);

            if (entity == null) return;

            _context.Speakers.RemoveRange(entity.Speakers);
            _context.AgendaItems.RemoveRange(entity.AgendaItems);
            _context.Events.Remove(entity);
            _context.SaveChanges();
        }

        public int GetAvailablePlaces(Guid eventId)
        {
            var ev = _context.Events.FirstOrDefault(e => e.Id == eventId);
            if (ev == null) return 0;

            var reserved = _context.Tickets.Count(t => t.EventId == eventId && t.IsPaid);
            return Math.Max(0, ev.MaxCapacity - reserved);
        }

        public async Task ReservePlaceAsync(Guid id)
        {
            var available = GetAvailablePlaces(id);
            var ev = _context.Events.FirstOrDefault(e => e.Id == id);
            if (ev != null)
            {
                await _hubContext.Clients.All.SendAsync("ReceivePlacesUpdate",
                    id.ToString(), available, ev.MaxCapacity);
            }
        }

        private static EventFormViewModel MapToViewModel(EventEntity entity)
        {
            return new EventFormViewModel
            {
                Id              = entity.Id,
                Title           = entity.Title,
                Description     = entity.Description,
                StartDate       = entity.StartDate,
                EndDate         = entity.EndDate,
                LocationAddress = entity.LocationAddress,
                Latitude        = entity.Latitude,
                Longitude       = entity.Longitude,
                Price           = entity.Price,
                MaxCapacity     = entity.MaxCapacity,
                CategoryId      = entity.CategoryId,
                CategoryName    = entity.Category?.Name ?? "Général",
                CoverImageUrl   = entity.CoverImageUrl,
                OrganizerId     = entity.OrganizerId,
                AgendaItems     = entity.AgendaItems?.Select(ai => new AgendaItemViewModel
                {
                    Id        = ai.Id,
                    Title     = ai.Title,
                    StartTime = ai.StartTime,
                    EndTime   = ai.EndTime
                }).ToList() ?? new(),
                Speakers = entity.Speakers?.Select(s => new SpeakerViewModel
                {
                    Id              = s.Id,
                    FullName        = s.FullName,
                    Biography       = s.Biography,
                    LinkedInUrl     = s.LinkedInUrl,
                    ProfileImageUrl = s.ProfileImageUrl
                }).ToList() ?? new()
            };
        }
    }
}

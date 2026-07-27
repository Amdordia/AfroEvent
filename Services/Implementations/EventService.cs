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
    public class EventService : IEventService
    {
        private readonly AfroEventDbContext _context;
        private readonly IHubContext<EventHub> _hubContext;

        public EventService(AfroEventDbContext context, IHubContext<EventHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public List<EventFormViewModel> GetAllEvents()
        {
            var entities = _context.Events
                .Include(e => e.Category)
                .Include(e => e.Speakers)
                .Include(e => e.AgendaItems)
                .OrderBy(e => e.StartDate)
                .ToList();

            return entities.Select(MapToViewModel).ToList();
        }

        public EventFormViewModel GetEventById(Guid id)
        {
            var entity = _context.Events
                .Include(e => e.Category)
                .Include(e => e.Speakers)
                .Include(e => e.AgendaItems)
                .FirstOrDefault(e => e.Id == id);

            if (entity == null)
            {
                // Fallback to first or empty model
                var first = _context.Events
                    .Include(e => e.Category)
                    .Include(e => e.Speakers)
                    .Include(e => e.AgendaItems)
                    .FirstOrDefault();
                
                if (first != null)
                {
                    return MapToViewModel(first);
                }

                return new EventFormViewModel { Title = "Événement introuvable" };
            }

            return MapToViewModel(entity);
        }

        public void CreateEvent(EventFormViewModel model)
        {
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
            }

            var entity = new EventEntity
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description ?? string.Empty,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                LocationAddress = model.LocationAddress ?? string.Empty,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Price = model.Price,
                MaxCapacity = model.MaxCapacity,
                CoverImageUrl = model.CoverImageUrl ?? "https://images.unsplash.com/photo-1515187029135-18ee286d815b?auto=format&fit=crop&w=1200&q=80",
                CategoryId = model.CategoryId
            };

            if (model.AgendaItems != null)
            {
                foreach (var item in model.AgendaItems)
                {
                    entity.AgendaItems.Add(new AgendaItemEntity
                    {
                        Title = item.Title,
                        StartTime = item.StartTime,
                        EndTime = item.EndTime
                    });
                }
            }

            if (model.Speakers != null)
            {
                foreach (var item in model.Speakers)
                {
                    entity.Speakers.Add(new SpeakerEntity
                    {
                        FullName = item.FullName,
                        Biography = item.Biography ?? string.Empty,
                        LinkedInUrl = item.LinkedInUrl ?? string.Empty,
                        ProfileImageUrl = item.ProfileImageUrl ?? "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=300&q=80"
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

            if (entity != null)
            {
                entity.Title = model.Title;
                entity.Description = model.Description ?? string.Empty;
                entity.StartDate = model.StartDate;
                entity.EndDate = model.EndDate;
                entity.LocationAddress = model.LocationAddress ?? string.Empty;
                entity.Latitude = model.Latitude;
                entity.Longitude = model.Longitude;
                entity.Price = model.Price;
                entity.MaxCapacity = model.MaxCapacity;
                entity.CategoryId = model.CategoryId;
                if (!string.IsNullOrEmpty(model.CoverImageUrl))
                {
                    entity.CoverImageUrl = model.CoverImageUrl;
                }

                // Clear old sub-items and add updated ones
                _context.Speakers.RemoveRange(entity.Speakers);
                _context.AgendaItems.RemoveRange(entity.AgendaItems);

                if (model.AgendaItems != null)
                {
                    foreach (var item in model.AgendaItems)
                    {
                        entity.AgendaItems.Add(new AgendaItemEntity
                        {
                            Title = item.Title,
                            StartTime = item.StartTime,
                            EndTime = item.EndTime
                        });
                    }
                }

                if (model.Speakers != null)
                {
                    foreach (var item in model.Speakers)
                    {
                        entity.Speakers.Add(new SpeakerEntity
                        {
                            FullName = item.FullName,
                            Biography = item.Biography ?? string.Empty,
                            LinkedInUrl = item.LinkedInUrl ?? string.Empty,
                            ProfileImageUrl = item.ProfileImageUrl ?? "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=300&q=80"
                        });
                    }
                }

                _context.SaveChanges();
            }
        }

        public async Task ReservePlaceAsync(Guid id)
        {
            var entity = _context.Events.FirstOrDefault(e => e.Id == id);
            if (entity != null)
            {
                // Count current active tickets for this event
                var reserved = _context.Tickets.Count(t => t.EventId == id && t.IsPaid);
                var available = Math.Max(0, entity.MaxCapacity - reserved);

                // Send live update via SignalR
                await _hubContext.Clients.All.SendAsync("ReceivePlacesUpdate", id.ToString(), available, entity.MaxCapacity);
            }
        }

        private static EventFormViewModel MapToViewModel(EventEntity entity)
        {
            return new EventFormViewModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                LocationAddress = entity.LocationAddress,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                Price = entity.Price,
                MaxCapacity = entity.MaxCapacity,
                CategoryId = entity.CategoryId,
                CategoryName = entity.Category?.Name ?? "Général",
                CoverImageUrl = entity.CoverImageUrl,
                AgendaItems = entity.AgendaItems.Select(ai => new AgendaItemViewModel
                {
                    Id = ai.Id,
                    Title = ai.Title,
                    StartTime = ai.StartTime,
                    EndTime = ai.EndTime
                }).ToList(),
                Speakers = entity.Speakers.Select(s => new SpeakerViewModel
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    Biography = s.Biography,
                    LinkedInUrl = s.LinkedInUrl,
                    ProfileImageUrl = s.ProfileImageUrl
                }).ToList()
            };
        }
    }
}

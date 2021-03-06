﻿using System.Collections.Generic;
using System.Linq;
using LetsMeet.DA.Dto;
using LetsMeet.DA.Interfaces;
using AutoMapper;
using LetsMeet.DA.Models;
using Microsoft.EntityFrameworkCore;

namespace LetsMeet.DA.Repositories
{
    public class FindEventsRepository : IFindEventsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public FindEventsRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<EventDto> GetAll()
        { 
            var result = _context.Events.ToList();
            return _mapper.Map<List<EventDto>>(result);
        }

        public IEnumerable<EventDto> GetByTitle(string title)
        {
            var events = _context.Events.Where(x => x.Title.Contains(title)).ToList();
            var result = _mapper.Map<IEnumerable<EventDto>>(events);
            return result;
        }

        public string GetEventDescription(int id)
        {
            var eventInDb = _context.Events.SingleOrDefault(n => n.Id == id);
            var result = eventInDb.Description;
            return result;
        }

        public List<EventWithHostNameDto> GetEventsWithHostNames()
        {
            var eventsInDB = _context.Events.Include(u => u.User).ToList();
            var result = _mapper.Map<List<EventWithHostNameDto>>(eventsInDB);
            return result;
        }

        public List<EventWithHostNameDto> GetEventsWithHostNames(string title)
        {
            var wantedEvents = _context.Events.Where(n => n.Title.Contains(title)).Include(u => u.User).ToList();
            var result = _mapper.Map<List<EventWithHostNameDto>>(wantedEvents);
            return result;
        }

        public EventWithHostNameDto GetEventWithHostName(int id)
        {
            var eventInDb = _context.Events.Where(n => n.Id == id).Include(u => u.User).ToList();
            var result = _mapper.Map<List<EventWithHostNameDto>>(eventInDb);
            return result.First();
        }

        public int GetNumberEventParticipants(int id)
        {
            int result = _context.Participants.Count(n => n.EventId == id);
            return result;
        }


        public void UpdateEvent(EventDto updated, string email)
        {
            var eventObject =_mapper.Map<Event>(updated);
            var eventInDb = _context.Events.Single(n => n.Id == eventObject.Id);
            var user = _context.Users.Single(u => u.Email == email);
            if(eventInDb.HostId == user.Id)
            {
                _context.Entry(eventInDb).CurrentValues.SetValues(eventObject);
                _context.SaveChanges();
            }
        }

        public void AddEvent(EventDto newEvent, string email)
        {
            var eventObject = _mapper.Map<Event>(newEvent);
            var user = _context.Users.Single(u => u.Email == email);
            eventObject.HostId = user.Id;
            _context.Events.Add(eventObject);
            _context.SaveChanges();
        }

        public void DeleteEvent(int id, string email)
        {
            var eventInDb = _context.Events.SingleOrDefault(n => n.Id == id);
            var user = _context.Users.Single(u => u.Email == email);
          
            if (eventInDb.HostId == user.Id)
            {
                var clearParticipants = _context.Participants.Where(e => e.EventId == eventInDb.Id);
                _context.Participants.RemoveRange(clearParticipants);

                _context.Events.Remove(eventInDb);
                _context.SaveChanges();
            }
        }

        public List<EventWithHostNameDto> GetMostPopularEvents()
        {
            var popularEvents = _context.Events.Include(p => p.Participants)
                .Include(u => u.User)
                .OrderByDescending(e => e.Participants.Count)
                .Take(3).ToList();
            var result = _mapper.Map<List<EventWithHostNameDto>>(popularEvents);
            return result;
        }

        public IEnumerable<string> GetUsersAssignedToEvent(int id)
        {
            var result = _context.Participants
                .Where(p => p.EventId == id)
                .Include(u => u.User)
                .Select(s => s.User.UserName);
            return result;
        }

        public void JoinToEvent(int id, string email)
        {
            var user = _context.Users.Single(u => u.Email == email);
            var selectedEvent = _context.Events.Single(e => e.Id == id);
            var participant = new Participant
            {
                Event = selectedEvent,
                EventId = selectedEvent.Id,
                User = user,
                UserId = user.Id
            };
            _context.Participants.Add(participant);
            _context.SaveChanges();
        }

        public bool IsAssignedToEvent(int id, string email)
        {
            var eventInDb = _context.Events.SingleOrDefault(n => n.Id == id);
            var user = _context.Users.Single(u => u.Email == email);

            var participant = _context.Participants.SingleOrDefault(n => n.EventId == id && n.UserId == user.Id);
            if (participant != null)
                return true;
            return false;
        }

        public void LeaveEvent(int id, string email)
        {
            var eventInDb = _context.Events.SingleOrDefault(n => n.Id == id);
            var user = _context.Users.Single(u => u.Email == email);

            var participant = _context.Participants.SingleOrDefault(n => n.UserId == user.Id && n.EventId == id);
            if(participant != null)
            {
                _context.Participants.Remove(participant);
                _context.SaveChanges();
            }
        }

        public List<EventWithHostNameDto> GetMyCreatedEvents(string email)
        {
            var user = _context.Users.Single(u => u.Email == email);
            var myEvents = _context.Events.Where(n => n.HostId == user.Id);
            var result = _mapper.Map<List<EventWithHostNameDto>>(myEvents);
            return result;
        }

        public List<EventWithHostNameDto> GetEventsAssignedToLoggedUser(string email)
        {
            var user = _context.Users.Single(u => u.Email == email);
            var assignedEvents = new List<Event>();
            var result = new List<Event>();

            var assignedParticipants = _context.Participants
                .Include(e => e.Event)
                .Where(p => p.UserId == user.Id)
                .Include(u => u.User)
                .ToList();
            assignedParticipants.ForEach(n => assignedEvents.Add(n.Event));
            var AllEventsFromDb = _context.Events.Include(u => u.User).ToList();
            foreach(var item in AllEventsFromDb)
            {
                foreach(var item2 in assignedEvents)
                {
                    if (item.Id == item2.Id)
                        result.Add(item);
                }
            }
            return _mapper.Map<List<EventWithHostNameDto>>(result);
        }
    }
}
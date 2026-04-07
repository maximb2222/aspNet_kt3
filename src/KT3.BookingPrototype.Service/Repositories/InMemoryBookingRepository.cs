using KT3.BookingPrototype.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KT3.BookingPrototype.Service.Repositories;

public class InMemoryBookingRepository : IBookingRepository
{
    private readonly object _syncRoot = new();
    private readonly List<Resource> _resources = new();
    private readonly List<Booking> _bookings = new();
    private int _nextResourceId = 1;
    private int _nextBookingId = 1;

    public InMemoryBookingRepository()
    {
        SeedResources();
    }

    public IReadOnlyList<Resource> GetResources()
    {
        lock (_syncRoot)
        {
            return _resources.ToList();
        }
    }

    public Resource? GetResourceById(int id)
    {
        lock (_syncRoot)
        {
            return _resources.FirstOrDefault(r => r.Id == id);
        }
    }

    public Resource AddResource(Resource resource)
    {
        lock (_syncRoot)
        {
            resource.Id = _nextResourceId++;
            _resources.Add(resource);
            return resource;
        }
    }

    public IReadOnlyList<Booking> GetBookings()
    {
        lock (_syncRoot)
        {
            return _bookings.ToList();
        }
    }

    public Booking AddBooking(Booking booking)
    {
        lock (_syncRoot)
        {
            booking.Id = _nextBookingId++;
            _bookings.Add(booking);
            return booking;
        }
    }

    public Booking? GetBookingById(int id)
    {
        lock (_syncRoot)
        {
            return _bookings.FirstOrDefault(b => b.Id == id);
        }
    }

    private void SeedResources()
    {
        _resources.Add(new Resource
        {
            Id = _nextResourceId++,
            Name = "Conference Room A",
            Type = "Room",
            Capacity = 10,
            BasePricePerHour = 30m,
            Amenities = new List<string> { "Projector", "Whiteboard", "WiFi" },
            Availability = BuildDefaultSchedule("Weekdays 08:00-20:00")
        });

        _resources.Add(new Resource
        {
            Id = _nextResourceId++,
            Name = "Conference Room B",
            Type = "Room",
            Capacity = 25,
            BasePricePerHour = 50m,
            Amenities = new List<string> { "Projector", "TV", "WiFi", "Coffee" },
            Availability = BuildDefaultSchedule("Weekdays 08:00-20:00")
        });
    }

    private static Schedule BuildDefaultSchedule(string notes)
    {
        var today = DateTime.UtcNow.Date;
        var slots = new List<TimeSlot>();

        for (var day = 0; day < 14; day++)
        {
            var current = today.AddDays(day);
            slots.Add(new TimeSlot
            {
                StartUtc = current.AddHours(8),
                EndUtc = current.AddHours(20)
            });
        }

        return new Schedule
        {
            Notes = notes,
            Slots = slots
        };
    }
}

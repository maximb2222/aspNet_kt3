using CoreWCF;
using KT3.BookingPrototype.Core.Contracts;
using KT3.BookingPrototype.Core.DTOs;
using KT3.BookingPrototype.Core.Models;
using KT3.BookingPrototype.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KT3.BookingPrototype.Service.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;

    public BookingService(IBookingRepository repository)
    {
        _repository = repository;
    }

    public List<Resource> SearchAvailableResources(SearchCriteria criteria)
    {
        AuthGuard.EnsureAuthorized();
        criteria ??= new SearchCriteria();

        var resources = _repository.GetResources().AsEnumerable();

        if (criteria.MinimumCapacity > 0)
        {
            resources = resources.Where(r => r.Capacity >= criteria.MinimumCapacity);
        }

        if (!string.IsNullOrWhiteSpace(criteria.ResourceType))
        {
            resources = resources.Where(r => string.Equals(r.Type, criteria.ResourceType, StringComparison.OrdinalIgnoreCase));
        }

        if (criteria.RequiredAmenities.Count > 0)
        {
            resources = resources.Where(r => criteria.RequiredAmenities.All(req =>
                r.Amenities.Any(actual => string.Equals(actual, req, StringComparison.OrdinalIgnoreCase))));
        }

        if (criteria.DesiredFromUtc < criteria.DesiredToUtc)
        {
            resources = resources.Where(r =>
                IsWithinAvailability(r.Availability, criteria.DesiredFromUtc, criteria.DesiredToUtc) &&
                !HasBookingConflict(r.Id, criteria.DesiredFromUtc, criteria.DesiredToUtc));
        }

        return resources.ToList();
    }

    public Booking ReserveResource(ReservationRequest request)
    {
        AuthGuard.EnsureAuthorized();

        if (request is null)
        {
            throw new FaultException("Request cannot be null.");
        }

        if (request.FromUtc >= request.ToUtc)
        {
            throw new FaultException("Invalid booking period.");
        }

        if (request.Customer is null || string.IsNullOrWhiteSpace(request.Customer.Email))
        {
            throw new FaultException("Customer information is required.");
        }

        var resource = _repository.GetResourceById(request.ResourceId);
        if (resource is null)
        {
            throw new FaultException("Resource not found.");
        }

        if (request.ParticipantsCount > resource.Capacity)
        {
            throw new FaultException("Too many participants for selected resource.");
        }

        if (!IsWithinAvailability(resource.Availability, request.FromUtc, request.ToUtc))
        {
            throw new FaultException("Resource is unavailable in requested period.");
        }

        if (HasBookingConflict(resource.Id, request.FromUtc, request.ToUtc))
        {
            throw new FaultException("Requested period is already booked.");
        }

        var hours = Math.Max(1, (int)Math.Ceiling((request.ToUtc - request.FromUtc).TotalHours));
        var totalPrice = resource.BasePricePerHour * hours;

        var booking = new Booking
        {
            ResourceId = resource.Id,
            ResourceName = resource.Name,
            CustomerName = $"{request.Customer.FirstName} {request.Customer.LastName}".Trim(),
            CustomerEmail = request.Customer.Email,
            FromUtc = request.FromUtc,
            ToUtc = request.ToUtc,
            Price = totalPrice,
            Status = BookingStatus.Active,
            CreatedAtUtc = DateTime.UtcNow
        };

        return _repository.AddBooking(booking);
    }

    public bool CancelBooking(int bookingId)
    {
        AuthGuard.EnsureAuthorized();

        var booking = _repository.GetBookingById(bookingId);
        if (booking is null)
        {
            return false;
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            return false;
        }

        booking.Status = BookingStatus.Cancelled;
        return true;
    }

    public Resource AddResource(ResourceDetails details)
    {
        AuthGuard.EnsureAuthorized();

        if (details is null)
        {
            throw new FaultException("Resource details are required.");
        }

        if (string.IsNullOrWhiteSpace(details.Name))
        {
            throw new FaultException("Resource name is required.");
        }

        if (details.Capacity <= 0)
        {
            throw new FaultException("Capacity must be greater than zero.");
        }

        var resource = new Resource
        {
            Name = details.Name,
            Type = string.IsNullOrWhiteSpace(details.Type) ? "Room" : details.Type,
            Capacity = details.Capacity,
            BasePricePerHour = details.BasePricePerHour <= 0 ? 1 : details.BasePricePerHour,
            Amenities = details.Amenities ?? new List<string>(),
            Availability = details.InitialSchedule ?? new Schedule()
        };

        return _repository.AddResource(resource);
    }

    public bool UpdateResourceAvailability(int resourceId, Schedule schedule)
    {
        AuthGuard.EnsureAuthorized();

        if (schedule is null || schedule.Slots.Count == 0)
        {
            return false;
        }

        var resource = _repository.GetResourceById(resourceId);
        if (resource is null)
        {
            return false;
        }

        resource.Availability = schedule;
        return true;
    }

    public RevenueReport GetRevenueReport(DateTime from, DateTime to)
    {
        AuthGuard.EnsureAuthorized();

        if (to <= from)
        {
            throw new FaultException("Invalid report period.");
        }

        var resourcesById = _repository.GetResources().ToDictionary(r => r.Id);

        var bookings = _repository.GetBookings()
            .Where(b => b.Status == BookingStatus.Active)
            .Where(b => b.FromUtc >= from && b.ToUtc <= to)
            .ToList();

        var byResource = bookings
            .GroupBy(b => b.ResourceId)
            .Select(group =>
            {
                resourcesById.TryGetValue(group.Key, out var resource);

                return new ResourceRevenueItem
                {
                    ResourceId = group.Key,
                    ResourceName = resource?.Name ?? "Unknown",
                    Revenue = group.Sum(x => x.Price),
                    BookingCount = group.Count()
                };
            })
            .OrderByDescending(x => x.Revenue)
            .ToList();

        return new RevenueReport
        {
            FromUtc = from,
            ToUtc = to,
            TotalRevenue = bookings.Sum(x => x.Price),
            TotalBookings = bookings.Count,
            ByResource = byResource
        };
    }

    private bool HasBookingConflict(int resourceId, DateTime fromUtc, DateTime toUtc)
    {
        return _repository.GetBookings()
            .Where(b => b.ResourceId == resourceId)
            .Where(b => b.Status == BookingStatus.Active)
            .Any(b => Overlaps(b.FromUtc, b.ToUtc, fromUtc, toUtc));
    }

    private static bool IsWithinAvailability(Schedule schedule, DateTime fromUtc, DateTime toUtc)
    {
        if (schedule is null || schedule.Slots.Count == 0)
        {
            return false;
        }

        return schedule.Slots.Any(slot => slot.StartUtc <= fromUtc && slot.EndUtc >= toUtc);
    }

    private static bool Overlaps(DateTime fromA, DateTime toA, DateTime fromB, DateTime toB)
    {
        return fromA < toB && fromB < toA;
    }
}

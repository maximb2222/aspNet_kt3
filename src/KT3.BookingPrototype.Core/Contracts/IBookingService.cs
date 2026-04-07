using KT3.BookingPrototype.Core.DTOs;
using KT3.BookingPrototype.Core.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace KT3.BookingPrototype.Core.Contracts;

/// <summary>
/// Main booking service contract.
/// </summary>
[ServiceContract]
public interface IBookingService
{
    /// <summary>
    /// Searches resources that match criteria and are free in requested time range.
    /// </summary>
    [OperationContract]
    List<Resource> SearchAvailableResources(SearchCriteria criteria);

    /// <summary>
    /// Creates a new booking.
    /// </summary>
    [OperationContract]
    Booking ReserveResource(ReservationRequest request);

    /// <summary>
    /// Cancels an existing booking by id.
    /// </summary>
    [OperationContract]
    bool CancelBooking(int bookingId);

    /// <summary>
    /// Adds a new resource.
    /// </summary>
    [OperationContract]
    Resource AddResource(ResourceDetails details);

    /// <summary>
    /// Replaces availability schedule for a resource.
    /// </summary>
    [OperationContract]
    bool UpdateResourceAvailability(int resourceId, Schedule schedule);

    /// <summary>
    /// Returns revenue analytics for a date range.
    /// </summary>
    [OperationContract]
    RevenueReport GetRevenueReport(DateTime from, DateTime to);
}

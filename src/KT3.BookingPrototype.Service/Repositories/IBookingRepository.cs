using KT3.BookingPrototype.Core.Models;
using System.Collections.Generic;

namespace KT3.BookingPrototype.Service.Repositories;

public interface IBookingRepository
{
    IReadOnlyList<Resource> GetResources();

    Resource? GetResourceById(int id);

    Resource AddResource(Resource resource);

    IReadOnlyList<Booking> GetBookings();

    Booking AddBooking(Booking booking);

    Booking? GetBookingById(int id);
}

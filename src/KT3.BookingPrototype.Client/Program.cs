using KT3.BookingPrototype.Core.Contracts;
using KT3.BookingPrototype.Core.DTOs;
using KT3.BookingPrototype.Core.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel;

const string endpointAddress = "http://localhost:8080/BookingService.svc";

var binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
{
    MaxReceivedMessageSize = 1024 * 1024
};

var factory = new ChannelFactory<IBookingService>(binding, new EndpointAddress(endpointAddress));
IBookingService channel = factory.CreateChannel();

try
{
    using var scope = new OperationContextScope((IContextChannel)channel);
    OperationContext.Current!.OutgoingMessageHeaders.Add(
        System.ServiceModel.Channels.MessageHeader.CreateHeader(AuthHeader.Name, AuthHeader.Namespace, AuthHeader.DemoApiKey));

    Console.WriteLine("=== KT3 Booking Demo Client ===");
    Console.WriteLine($"Endpoint: {endpointAddress}");
    Console.WriteLine($"API key: {AuthHeader.DemoApiKey}");
    Console.WriteLine();

    var bookingDay = DateTime.UtcNow.Date.AddDays(1);
    var fromUtc = bookingDay.AddHours(10);
    var toUtc = bookingDay.AddHours(12);

    var newResource = channel.AddResource(new ResourceDetails
    {
        Name = "Training Room C",
        Type = "Room",
        Capacity = 15,
        BasePricePerHour = 40,
        Amenities = new List<string> { "Projector", "WiFi", "Whiteboard" },
        InitialSchedule = new Schedule
        {
            Notes = "Temporary schedule",
            Slots = new List<TimeSlot>
            {
                new() { StartUtc = bookingDay.AddHours(8), EndUtc = bookingDay.AddHours(20) }
            }
        }
    });

    Console.WriteLine($"1) AddResource -> Id={newResource.Id}, Name={newResource.Name}");

    var updated = channel.UpdateResourceAvailability(newResource.Id, new Schedule
    {
        Notes = "Updated schedule for demo",
        Slots = new List<TimeSlot>
        {
            new() { StartUtc = bookingDay.AddHours(8), EndUtc = bookingDay.AddHours(22) }
        }
    });

    Console.WriteLine($"2) UpdateResourceAvailability -> {updated}");

    var found = channel.SearchAvailableResources(new SearchCriteria
    {
        DesiredFromUtc = fromUtc,
        DesiredToUtc = toUtc,
        MinimumCapacity = 8,
        ResourceType = "Room",
        RequiredAmenities = new List<string> { "WiFi" }
    });

    Console.WriteLine($"3) SearchAvailableResources -> Found={found.Count}");

    var booking = channel.ReserveResource(new ReservationRequest
    {
        ResourceId = newResource.Id,
        FromUtc = fromUtc,
        ToUtc = toUtc,
        ParticipantsCount = 8,
        Customer = new CustomerInfo
        {
            FirstName = "Max",
            LastName = "Student",
            Email = "max@example.com",
            Phone = "+10000000000"
        }
    });

    Console.WriteLine($"4) ReserveResource -> BookingId={booking.Id}, Price={booking.Price}");

    var reportBeforeCancel = channel.GetRevenueReport(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(30));
    Console.WriteLine($"5) GetRevenueReport(before cancel) -> TotalRevenue={reportBeforeCancel.TotalRevenue}, TotalBookings={reportBeforeCancel.TotalBookings}");

    var cancelled = channel.CancelBooking(booking.Id);
    Console.WriteLine($"6) CancelBooking -> {cancelled}");

    var reportAfterCancel = channel.GetRevenueReport(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(30));
    Console.WriteLine($"7) GetRevenueReport(after cancel) -> TotalRevenue={reportAfterCancel.TotalRevenue}, TotalBookings={reportAfterCancel.TotalBookings}");

    Console.WriteLine();
    Console.WriteLine("Demo completed successfully.");
}
catch (EndpointNotFoundException)
{
    Console.WriteLine("Service is not running. Start KT3.BookingPrototype.Service first.");
}
catch (FaultException fault)
{
    Console.WriteLine($"Service fault: {fault.Message}");
}
finally
{
    var clientChannel = (IClientChannel)channel;
    if (clientChannel.State == CommunicationState.Faulted)
    {
        clientChannel.Abort();
        factory.Abort();
    }
    else
    {
        clientChannel.Close();
        factory.Close();
    }
}

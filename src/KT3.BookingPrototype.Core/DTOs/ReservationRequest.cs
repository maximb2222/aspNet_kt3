using System;
using System.Runtime.Serialization;

namespace KT3.BookingPrototype.Core.DTOs;

[DataContract]
public class ReservationRequest
{
    [DataMember(Order = 1)]
    public int ResourceId { get; set; }

    [DataMember(Order = 2)]
    public CustomerInfo Customer { get; set; } = new();

    [DataMember(Order = 3)]
    public DateTime FromUtc { get; set; }

    [DataMember(Order = 4)]
    public DateTime ToUtc { get; set; }

    [DataMember(Order = 5)]
    public int ParticipantsCount { get; set; }
}

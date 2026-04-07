using System;
using System.Runtime.Serialization;

namespace KT3.BookingPrototype.Core.Models;

[DataContract]
public class Booking
{
    [DataMember(Order = 1)]
    public int Id { get; set; }

    [DataMember(Order = 2)]
    public int ResourceId { get; set; }

    [DataMember(Order = 3)]
    public string ResourceName { get; set; } = string.Empty;

    [DataMember(Order = 4)]
    public string CustomerName { get; set; } = string.Empty;

    [DataMember(Order = 5)]
    public string CustomerEmail { get; set; } = string.Empty;

    [DataMember(Order = 6)]
    public DateTime FromUtc { get; set; }

    [DataMember(Order = 7)]
    public DateTime ToUtc { get; set; }

    [DataMember(Order = 8)]
    public decimal Price { get; set; }

    [DataMember(Order = 9)]
    public BookingStatus Status { get; set; }

    [DataMember(Order = 10)]
    public DateTime CreatedAtUtc { get; set; }
}

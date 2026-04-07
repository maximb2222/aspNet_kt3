using System;
using System.Runtime.Serialization;

namespace KT3.BookingPrototype.Core.Models;

[DataContract]
public class TimeSlot
{
    [DataMember(Order = 1)]
    public DateTime StartUtc { get; set; }

    [DataMember(Order = 2)]
    public DateTime EndUtc { get; set; }
}

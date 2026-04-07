using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KT3.BookingPrototype.Core.Models;

[DataContract]
public class Schedule
{
    [DataMember(Order = 1)]
    public List<TimeSlot> Slots { get; set; } = new();

    [DataMember(Order = 2)]
    public string Notes { get; set; } = string.Empty;
}

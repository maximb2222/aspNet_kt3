using KT3.BookingPrototype.Core.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KT3.BookingPrototype.Core.DTOs;

[DataContract]
public class ResourceDetails
{
    [DataMember(Order = 1)]
    public string Name { get; set; } = string.Empty;

    [DataMember(Order = 2)]
    public string Type { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    public int Capacity { get; set; }

    [DataMember(Order = 4)]
    public decimal BasePricePerHour { get; set; }

    [DataMember(Order = 5)]
    public List<string> Amenities { get; set; } = new();

    [DataMember(Order = 6)]
    public Schedule InitialSchedule { get; set; } = new();
}

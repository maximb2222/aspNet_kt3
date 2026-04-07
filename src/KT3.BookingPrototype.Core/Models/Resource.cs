using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KT3.BookingPrototype.Core.Models;

[DataContract]
public class Resource
{
    [DataMember(Order = 1)]
    public int Id { get; set; }

    [DataMember(Order = 2)]
    public string Name { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    public string Type { get; set; } = string.Empty;

    [DataMember(Order = 4)]
    public int Capacity { get; set; }

    [DataMember(Order = 5)]
    public decimal BasePricePerHour { get; set; }

    [DataMember(Order = 6)]
    public List<string> Amenities { get; set; } = new();

    [DataMember(Order = 7)]
    public Schedule Availability { get; set; } = new();
}

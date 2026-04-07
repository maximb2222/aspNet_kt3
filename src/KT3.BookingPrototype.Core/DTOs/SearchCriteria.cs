using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KT3.BookingPrototype.Core.DTOs;

[DataContract]
public class SearchCriteria
{
    [DataMember(Order = 1)]
    public DateTime DesiredFromUtc { get; set; }

    [DataMember(Order = 2)]
    public DateTime DesiredToUtc { get; set; }

    [DataMember(Order = 3)]
    public int MinimumCapacity { get; set; }

    [DataMember(Order = 4)]
    public string ResourceType { get; set; } = string.Empty;

    [DataMember(Order = 5)]
    public List<string> RequiredAmenities { get; set; } = new();
}

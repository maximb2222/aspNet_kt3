using System.Runtime.Serialization;

namespace KT3.BookingPrototype.Core.Models;

[DataContract]
public class ResourceRevenueItem
{
    [DataMember(Order = 1)]
    public int ResourceId { get; set; }

    [DataMember(Order = 2)]
    public string ResourceName { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    public decimal Revenue { get; set; }

    [DataMember(Order = 4)]
    public int BookingCount { get; set; }
}

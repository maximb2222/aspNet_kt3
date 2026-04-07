using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KT3.BookingPrototype.Core.Models;

[DataContract]
public class RevenueReport
{
    [DataMember(Order = 1)]
    public DateTime FromUtc { get; set; }

    [DataMember(Order = 2)]
    public DateTime ToUtc { get; set; }

    [DataMember(Order = 3)]
    public decimal TotalRevenue { get; set; }

    [DataMember(Order = 4)]
    public int TotalBookings { get; set; }

    [DataMember(Order = 5)]
    public List<ResourceRevenueItem> ByResource { get; set; } = new();
}

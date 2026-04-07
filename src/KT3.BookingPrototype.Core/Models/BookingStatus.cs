using System.Runtime.Serialization;

namespace KT3.BookingPrototype.Core.Models;

[DataContract]
public enum BookingStatus
{
    [EnumMember]
    Active = 1,

    [EnumMember]
    Cancelled = 2
}

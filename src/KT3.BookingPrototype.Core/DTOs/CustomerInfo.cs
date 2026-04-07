using System.Runtime.Serialization;

namespace KT3.BookingPrototype.Core.DTOs;

[DataContract]
public class CustomerInfo
{
    [DataMember(Order = 1)]
    public string FirstName { get; set; } = string.Empty;

    [DataMember(Order = 2)]
    public string LastName { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    public string Email { get; set; } = string.Empty;

    [DataMember(Order = 4)]
    public string Phone { get; set; } = string.Empty;
}

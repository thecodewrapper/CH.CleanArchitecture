using System;
using CH.Data.Abstractions;

namespace CH.CleanArchitecture.Infrastructure.Models
{
    public class AddressEntity : DataEntityBase<Guid>
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
    }
}

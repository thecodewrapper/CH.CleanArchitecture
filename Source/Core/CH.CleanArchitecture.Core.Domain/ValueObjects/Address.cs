namespace CH.CleanArchitecture.Core.Domain
{
    public class Address
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }

        public Address(string line1, string line2, string city, string postCode, string country) {
            Line1 = line1;
            Line2 = line2;
            City = city;
            Postcode = postCode;
            Country = country;
        }
    }
}

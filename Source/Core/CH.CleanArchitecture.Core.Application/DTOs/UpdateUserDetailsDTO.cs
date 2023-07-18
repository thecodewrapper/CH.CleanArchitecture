namespace CH.CleanArchitecture.Core.Application.DTOs
{
    /// <summary>
    /// Represents a request to update a user's details
    /// </summary>
    public class UpdateUserDetailsDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PrimaryPhone { get; set; }
        public string SecondaryPhone { get; set; }
    }
}

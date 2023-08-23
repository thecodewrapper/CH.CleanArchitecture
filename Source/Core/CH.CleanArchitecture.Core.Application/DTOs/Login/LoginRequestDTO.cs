namespace CH.CleanArchitecture.Core.Application.DTOs
{
    public class LoginRequestDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; } //TODO[CH]: Consider removing this?
    }
}

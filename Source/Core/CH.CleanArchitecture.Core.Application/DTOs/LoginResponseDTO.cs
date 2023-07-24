namespace CH.CleanArchitecture.Core.Application.DTOs
{
    public class LoginResponseDTO
    {
        public string User { get; set; }
        public bool Success { get; set; }
        public bool Requires2FA { get; set; }
        public bool IsLockedOut { get; set; }
    }
}

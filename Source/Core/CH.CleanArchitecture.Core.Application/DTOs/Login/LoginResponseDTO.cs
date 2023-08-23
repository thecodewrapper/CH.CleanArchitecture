namespace CH.CleanArchitecture.Core.Application.DTOs
{
    public class LoginResponseDTO
    {
        public string User { get; set; }
        public bool Success { get; set; }
        public bool Requires2FA { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsNotAllowed { get; set; }

        public override string ToString() {
            return $"User: {User}, Success: {Success}, Requires2FA: {Requires2FA}, IsLockedOut: {IsLockedOut}, IsNotAllowed: {IsNotAllowed}";
        }
    }
}

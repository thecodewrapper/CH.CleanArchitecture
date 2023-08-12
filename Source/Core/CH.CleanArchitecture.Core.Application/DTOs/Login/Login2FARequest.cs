namespace CH.CleanArchitecture.Core.Application.DTOs
{
    public class Login2FARequest
    {
        public string Code { get; set; }
        public bool IsPersisted { get; set; }
        public bool RememberClient { get; set; }
    }
}
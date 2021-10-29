namespace CH.CleanArchitecture.Core.Application.DTOs
{
    public class ApplicationConfigurationDTO
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public bool IsEncrypted { get; set; }
    }
}

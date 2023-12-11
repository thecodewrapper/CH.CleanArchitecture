namespace CH.CleanArchitecture.Infrastructure.Options
{
    internal class EmailSenderOptions
    {
        /// <summary>
        /// Indicates whether to use SendGrid as an email sender
        /// </summary>
        public bool UseSendGrid { get; set; } = false;
    }
}

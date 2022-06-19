namespace CH.CleanArchitecture.Presentation.EventStoreWeb.Models
{
    public class RetroactiveEventModel : EventModelBase
    {
        /// <summary>
        /// The sequence of the retroactive event
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// Indicates whether to ignore or apply this event
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}

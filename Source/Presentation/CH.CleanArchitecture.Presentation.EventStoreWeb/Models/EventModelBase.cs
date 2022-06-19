namespace CH.CleanArchitecture.Presentation.EventStoreWeb.Models
{
    public class EventModelBase
    {
        /// <summary>
        /// The assembly type name of the retroactive event
        /// </summary>
        public string AssemblyTypeName { get; set; }

        /// <summary>
        /// The payload of the retroactive event
        /// </summary>
        public string Data { get; set; }
    }
}

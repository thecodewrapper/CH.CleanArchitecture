namespace CH.CleanArchitecture.Core.Domain
{
    /// <summary>
    /// User Created domain event
    /// </summary>
    public class UserCreatedEvent : DomainEventBase<string>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Needed for serialization
        /// </summary>
        internal UserCreatedEvent() {
        }

        public UserCreatedEvent(string username, string email, string name) {
            Username = username;
            Email = email;
            Name = name;
        }

        public UserCreatedEvent(string aggregateId, int aggregateVersion, string username, string email, string name)
            : base(aggregateId, aggregateVersion) {
            Username = username;
            Email = email;
            Name = name;
        }

        public override IDomainEvent<string> WithAggregate(string aggregateId, int aggregateVersion) {
            return new UserCreatedEvent(aggregateId, aggregateVersion, Username, Email, Name);
        }
    }
}
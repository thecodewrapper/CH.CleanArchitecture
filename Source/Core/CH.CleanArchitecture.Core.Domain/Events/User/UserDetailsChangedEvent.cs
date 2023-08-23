using CH.Domain.Abstractions;

namespace CH.CleanArchitecture.Core.Domain.Events.User
{
    public class UserDetailsChangedEvent : DomainEventBase<string>
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        /// <summary>
        /// Needed for serialization
        /// </summary>
        internal UserDetailsChangedEvent() {
        }

        public UserDetailsChangedEvent(string email, string phoneNumber, string name, string surname) {
            Email = email;
            PhoneNumber = phoneNumber;
            Name = name;
            Surname = surname;
        }

        public UserDetailsChangedEvent(string aggregateId, int aggregateVersion, string email, string phoneNumber, string name, string surname)
            : base(aggregateId, aggregateVersion) {
            Email = email;
            PhoneNumber = phoneNumber;
            Name = name;
            Surname = surname;
        }

        public override IDomainEvent<string> WithAggregate(string aggregateId, int aggregateVersion) {
            return new UserDetailsChangedEvent(aggregateId, aggregateVersion, Email, PhoneNumber, Name, Surname);
        }
    }
}

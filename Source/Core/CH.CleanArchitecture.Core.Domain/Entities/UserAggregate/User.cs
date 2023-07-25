using System.Collections.Generic;
using Ardalis.GuardClauses;
using CH.CleanArchitecture.Core.Domain.Events.User;
using CH.Domain.Abstractions;

namespace CH.CleanArchitecture.Core.Domain.Entities.UserAggregate
{
    /// <summary>
    /// The User aggregate, representing a user.
    /// Notes: Use domain events when there is a particular reason to emit those events in other places.
    /// Also use this approach when you are implementing Event Sourcing.
    /// </summary>
    public class User : AggregateRootBase<string>, IDomainEventHandler<UserCreatedEvent>, IDomainEventHandler<UserDetailsChangedEvent>
    {
        public string Username { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public Address Address { get; private set; }
        public string Email { get; private set; }
        public IReadOnlyCollection<RoleEnum> Roles { get; private set; }
        public bool IsActive { get; private set; }

        private User() {

        }

        public User(string username, string email, string name, string surname) {
            Guard.Against.NullOrWhiteSpace(username, nameof(username));
            Guard.Against.NullOrWhiteSpace(email, nameof(email));
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.NullOrWhiteSpace(surname, nameof(surname));

            RaiseEvent(new UserCreatedEvent(username, email, name, surname));
        }

        void IDomainEventHandler<UserCreatedEvent>.Apply(UserCreatedEvent @event) {
            Username = @event.Username;
            Name = @event.Name;
            Email = @event.Email;
            Surname = @event.Surname;
        }

        public void ChangeDetails(string email, string phoneNumber, string name, string surname) {
            RaiseEvent(new UserDetailsChangedEvent(email, phoneNumber, name, surname));
        }

        void IDomainEventHandler<UserDetailsChangedEvent>.Apply(UserDetailsChangedEvent @event) {
            if (!string.IsNullOrEmpty(Email) && Email != @event.Email) {
                Email = @event.Email;
            }

            if (!string.IsNullOrEmpty(Name) && Name != @event.Name) {
                Name = @event.Name;
            }

            if (!string.IsNullOrEmpty(Surname) && Surname != @event.Surname) {
                Surname = @event.Surname;
            }

            if (!string.IsNullOrEmpty(@event.PhoneNumber))
                PhoneNumber = new PhoneNumber(@event.PhoneNumber);
        }

        public void ChangeAddress(string line1, string line2, string city, string postCode, string country) {
            Address = new Address(line1, line2, city, postCode, country);
        }

        public void Activate() {
            IsActive = true;
        }

        public void Deactivate() {
            IsActive = false;
        }
    }
}

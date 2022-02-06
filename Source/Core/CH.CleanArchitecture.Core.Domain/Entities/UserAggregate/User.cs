using System.Collections.Generic;
using Ardalis.GuardClauses;

namespace CH.CleanArchitecture.Core.Domain.Entities.UserAggregate
{
    public class User : AggregateRootBase<string>, IDomainEventHandler<UserCreatedEvent>
    {
        public string Username { get; private set; }
        public string Name { get; private set; }
        public string PrimaryPhoneNumber { get; private set; }
        public string SecondaryPhoneNumber { get; private set; }
        public string Email { get; private set; }
        public IReadOnlyCollection<RoleEnum> Roles { get; private set; }
        public bool IsActive { get; private set; }

        private User() {

        }

        public User(string username, string email, string name) {
            Guard.Against.NullOrWhiteSpace(username, nameof(username));
            Guard.Against.NullOrWhiteSpace(email, nameof(email));
            Guard.Against.NullOrWhiteSpace(name, nameof(name));

            RaiseEvent(new UserCreatedEvent(username, email, name));
        }

        void IDomainEventHandler<UserCreatedEvent>.Apply(UserCreatedEvent @event) {
            Username = @event.Username;
            Name = @event.Name;
            Email = @event.Email;
        }
    }
}

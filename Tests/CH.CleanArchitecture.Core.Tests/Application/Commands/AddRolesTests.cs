using System.Collections.Generic;
using CH.CleanArchitecture.Core.Application.Commands;
using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Tests;
using FluentAssertions;
using Xunit;

namespace CH.CleanArchitecture.Core.Tests.Application.Commands
{
    public class AddRolesTests : TestBase
    {
        public AddRolesTests() {

        }
        [Fact]
        public async void AddRoles_WhenRoleIsValidAndNotAssigned_ShouldSucceed() {
            var command = new AddRolesCommand("basicUser", new List<string>
                {
                    RoleEnum.SuperAdmin.ToString()
                });

            var result = await ServiceBus.SendAsync(command);

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public async void AddRoles_WhenRoleIsInvalid_ShouldFail() {
            var command = new AddRolesCommand("basicUser", new List<string>
                {
                    "InvalidRole"
                });

            var result = await ServiceBus.SendAsync(command);

            result.IsSuccessful.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Error == $"Role InvalidRole is invalid.");
        }
    }
}

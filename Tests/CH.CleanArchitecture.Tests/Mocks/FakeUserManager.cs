using CH.CleanArchitecture.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using IdentityDbContext = CH.CleanArchitecture.Infrastructure.DbContexts.IdentityDbContext;

namespace CH.CleanArchitecture.Tests
{
    public class FakeUserManager : UserManager<ApplicationUser>
    {
        public FakeUserManager(IdentityDbContext context)
            : base(new UserStore<ApplicationUser>(context),
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<ApplicationUser>>().Object,
                Array.Empty<IUserValidator<ApplicationUser>>(),
                Array.Empty<IPasswordValidator<ApplicationUser>>(),
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<ApplicationUser>>>().Object) { }
    }
}

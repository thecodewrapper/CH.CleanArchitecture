using CH.CleanArchitecture.Infrastructure.DbContexts;
using CH.CleanArchitecture.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CH.CleanArchitecture.Tests
{
    public class FakeSignInManager : SignInManager<ApplicationUser>
    {
        public FakeSignInManager(IdentityDbContext context)
            : base(new FakeUserManager(context),
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<ApplicationUser>>().Object) { }
    }
}

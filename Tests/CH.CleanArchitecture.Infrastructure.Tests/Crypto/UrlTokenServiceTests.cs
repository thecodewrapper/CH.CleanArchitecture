using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Tests;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CH.CleanArchitecture.Infrastructure.Tests.Crypto
{
    public class UrlTokenServiceTests : TestBase
    {
        private readonly IUrlTokenService _service;

        public UrlTokenServiceTests() {
            _service = ServiceProvider.GetService<IUrlTokenService>();
        }

        [Fact]
        public void CreateSafeUrlToken_Returns_String_Token_NotNull() {
            string payload = "Test";
            string token = _service.CreateSafeUrlToken(payload);

            Assert.NotNull(token);
        }

        [Fact]
        public void ReadSafeUrlToken_When_Created_Using_CreateSafeUrlToken_Value_Matches() {
            string payload = "Test";
            string token = _service.CreateSafeUrlToken(payload);
            string readPayload = _service.ReadSafeUrlToken<string>(token);

            Assert.Equal(payload, readPayload);
        }
    }
}

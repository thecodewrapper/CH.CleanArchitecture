using System.IdentityModel.Tokens.Jwt;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Tests;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CH.CleanArchitecture.Infrastructure.Tests.Crypto
{
    public class JwtServiceTests : TestBase
    {
        private readonly IJWTService _service;

        public JwtServiceTests() {
            _service = ServiceProvider.GetService<IJWTService>();
        }

        [Fact]
        public void CreateJWT_Creates_A_Valid_JWT_String() {
            string payload = "test";
            string jwt = _service.CreateJWT(payload, 10);

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwt);

            Assert.NotNull(token);
        }

        [Fact]
        public void ReadJWT_Returns_Payload_String_Matches() {
            string payload = "test";
            string jwt = _service.CreateJWT(payload, 10);
            string readPayload = _service.ReadJWT<string>(jwt);

            Assert.Equal(payload, readPayload);
        }
    }
}
